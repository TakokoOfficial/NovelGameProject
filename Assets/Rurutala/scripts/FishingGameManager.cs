using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishingGameManager : MonoBehaviour
{
    [Header("ゲーム設定")]
    public int maxBaitCount = 3;

    [Header("タイミング設定 (竿を投げてから魚がかかるまでの待機時間)")]
    [Range(1.0f, 8.0f)]
    [Tooltip("魚がかかる最短時間（秒）")]
    public float minWaitTime = 2.0f;

    [Range(3.0f, 10.0f)]
    [Tooltip("魚がかかる最長時間（秒）")]
    public float maxWaitTime = 6.0f;

    [Header("魚のデータ")]
    public FishData[] availableFish;


    [Header("物置のカギ")]
    public FishData fish0Data;

    [Header("現在の状態")]
    public FishingState currentState = FishingState.Waiting;
    private int remainingBait;
    private List<float> reactionTimes = new List<float>();

    [Header("UI管理")]
    public FishingGameUIManager uiManager;
    public Button fishBookButton; // FishBookボタン
    public Button closeBookButton; // 図鑑閉じるボタン

    [Header("シーン遷移設定")]
    public string nextSceneName = "TitleScene";
    public Button returnSceneButton; // 戻るボタン（シーン遷移用）
    

    // プライベート変数
    private float waitStartTime;
    private float fishHookTime;
    private FishData currentFish;
    private bool canCatch = false;

    // CancellationTokenSource for managing async operations
    private CancellationTokenSource fishWaitCts;
    private CancellationTokenSource fishEscapeCts;
    private CancellationTokenSource returnToWaitingCts;

    // イベント
    public System.Action<FishingState> OnStateChanged;
    public System.Action<FishData, float> OnFishCaught;
    public System.Action OnGameOver;

    // 魚モデル用
    private GameObject lastFishModelInstance;

    // Fish_0が釣れたかどうかを管理する変数
    private bool isFish0Caught = false;

    private void Start()
    {
        remainingBait = maxBaitCount;
        DebugLog($"ゲーム開始 - 餌の数: {remainingBait}");
        OnStateChanged += HandleStateChangedUI;
        if (uiManager != null)
        {
            uiManager.SetBaitIcons(remainingBait);
            uiManager.SetExclamationIconActive(false);
            uiManager.HideFishName();
            uiManager.HideFishReactionTime();
            uiManager.HideAverageReactionTime();
        }
        if (fishBookButton != null)
        {
            fishBookButton.onClick.AddListener(OnFishBookButtonClicked);
        }
        if (closeBookButton != null)
        {
            closeBookButton.onClick.AddListener(OnCloseBookButtonClicked);
        }

        if (returnSceneButton != null)
        {
            returnSceneButton.onClick.AddListener(OnContinueNG);
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateFishingLogic();
    }

    private void OnDestroy()
    {
        // すべてのCancellationTokenをキャンセル
        CancelAllAsyncOperations();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
            {
                return;
            }
            OnLeftClick();
        }
    }
    private bool IsPointerOverUI()
    {
        // EventSystemが存在しない場合はfalseを返す
        if (EventSystem.current == null)
            return false;

        // モバイル対応も含めた判定
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
    private void OnLeftClick()
    {
        switch (currentState)
        {
            case FishingState.Waiting:
                StartCastingAsync().Forget();
                break;
            case FishingState.Casting:
                EarlyClick();
                break;
            case FishingState.FishOnHook:
                AttemptCatch();
                break;
            case FishingState.Success:
            case FishingState.Failed:
                // 成功・失敗時はクリックで待機状態に戻す＋魚モデル削除
                if (lastFishModelInstance != null)
                {
                    Destroy(lastFishModelInstance);
                    lastFishModelInstance = null;
                }
                ChangeState(FishingState.Waiting);
                // ここで餌が0なら結果表示
                if (remainingBait <= 0)
                {
                    DebugLog("ゲームオーバー！");
                    OnGameOver?.Invoke();
                    GameOverSequence();
                }
                break;
            case FishingState.ViewingBook:
                // 閲覧中は左クリックで何もしない（または必要に応じてページ送り等）
                break;
        }
    }

    private void OnFishBookButtonClicked()
    {
        if (currentState == FishingState.Waiting)
        {
            ChangeState(FishingState.ViewingBook);
            // 必要に応じて図鑑UI表示処理を呼ぶ
            if (uiManager != null) uiManager.ShowFishBook(true);
        }
    }
    private void OnCloseBookButtonClicked()
    {
        if (currentState == FishingState.ViewingBook)
        {
            ChangeState(FishingState.Waiting);
            if (uiManager != null) uiManager.ShowFishBook(false);
        }
    }

    private async UniTaskVoid StartCastingAsync()
    {
        if (remainingBait <= 0)
        {
            DebugLog("餌がありません！");
            return;
        }

        ChangeState(FishingState.Casting);
        waitStartTime = Time.time;

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        DebugLog($"竿を投げました。待機時間: {waitTime:F1}秒");

        // 前回の待機をキャンセル
        fishWaitCts?.Cancel();
        fishWaitCts = new CancellationTokenSource();

        try
        {
            await WaitForFishAsync(waitTime, fishWaitCts.Token);
        }
        catch (System.OperationCanceledException)
        {
            DebugLog("魚の待機がキャンセルされました");
        }
    }

    private async UniTask WaitForFishAsync(float waitTime, CancellationToken cancellationToken)
    {
        await UniTask.Delay((int)(waitTime * 1000), cancellationToken: cancellationToken);

        if (currentState == FishingState.Casting && !cancellationToken.IsCancellationRequested)
        {
            currentFish = SelectRandomFish();
            fishHookTime = Time.time;
            canCatch = true;
            ChangeState(FishingState.FishOnHook);

            DebugLog($"魚がかかりました！ {currentFish.fishName}");

            // 魚が逃げるまでの時間後に自動で失敗させる
            FishEscapeTimerAsync(currentFish.escapeTime).Forget();
        }
    }

    private async UniTaskVoid FishEscapeTimerAsync(float escapeTime)
    {
        // 前回のエスケープタイマーをキャンセル
        fishEscapeCts?.Cancel();
        fishEscapeCts = new CancellationTokenSource();

        try
        {
            await UniTask.Delay((int)(escapeTime * 1000), cancellationToken: fishEscapeCts.Token);

            if (currentState == FishingState.FishOnHook && !fishEscapeCts.Token.IsCancellationRequested)
            {
                FishEscaped();
            }
        }
        catch (System.OperationCanceledException)
        {
            DebugLog("魚のエスケープタイマーがキャンセルされました");
        }
    }

    private void EarlyClick()
    {
        // 魚の待機をキャンセル
        fishWaitCts?.Cancel();

        remainingBait--;
        DebugLog($"早すぎるクリック！餌を消費しました。残り餌: {remainingBait}");

        ChangeState(FishingState.Failed);

        // ReturnToWaitingAsyncは呼ばない
    }

    private void AttemptCatch()
    {
        if (!canCatch) return;
        fishEscapeCts?.Cancel();
        float reactionTime = Time.time - fishHookTime;
        reactionTimes.Add(reactionTime);
        remainingBait--;
        // 魚モデルの生成とアニメーション有効化
        if (currentFish != null && currentFish.fishModel != null)
        {
            if (lastFishModelInstance != null)
            {
                Destroy(lastFishModelInstance);
            }
            lastFishModelInstance = Instantiate(currentFish.fishModel);
            var animator = lastFishModelInstance.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Enable");
            }
        }
        // --- ここからPlayerPrefs保存処理 ---
        if (currentFish != null)
        {
            string baseKey = $"Fish_{currentFish.fishSpeciesId}";
            int count = PlayerPrefs.GetInt(baseKey + "_Count", 0) + 1;
            PlayerPrefs.SetInt(baseKey + "_Count", count);
            float prevBest = PlayerPrefs.GetFloat(baseKey + "_BestTime", float.MaxValue);
            if (reactionTime < prevBest)
            {
                PlayerPrefs.SetFloat(baseKey + "_BestTime", reactionTime);
            }
            if (currentFish.isRare)
            {
                PlayerPrefs.SetInt(baseKey + "_Rare", 1);
            }
            PlayerPrefs.Save();
            // Fish_0を釣ったらフラグをON
            if (currentFish.fishSpeciesId == 0)
            {
                isFish0Caught = true;
            }
        }
        // --- ここまでPlayerPrefs保存処理 ---
        ChangeState(FishingState.Catching);
        DebugLog($"反応時間: {reactionTime:F3}秒");
        OnFishCaught?.Invoke(currentFish, reactionTime);
        ChangeState(FishingState.Success);
        // ここではゲームオーバー判定しない
    }

    private void FishEscaped()
    {
        DebugLog($"{currentFish.fishName}が逃げました！");
        remainingBait--;
        canCatch = false;

        ChangeState(FishingState.Failed);

        // ReturnToWaitingAsyncは呼ばない
        // elseは不要
    }

    private void UpdateFishingLogic()
    {
        // 追加のロジックが必要な場合はここに記述
    }

    // 状態遷移を一元管理するメソッド
    private void ChangeState(FishingState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        DebugLog($"状態遷移: {newState}");
        OnStateChanged?.Invoke(newState);
        // 必要に応じてここでUI更新やサウンド再生なども追加可能
    }

    private void HandleStateChangedUI(FishingState state)
    {
        if (uiManager == null) return;
        switch (state)
        {
            case FishingState.Waiting:
                uiManager.SetExclamationIconActive(false);
                uiManager.HideFishName();
                uiManager.HideFishReactionTime();
                break;
            case FishingState.Casting:
                uiManager.SetExclamationIconActive(false);
                uiManager.HideFishName();
                uiManager.HideFishReactionTime();
                break;
            case FishingState.FishOnHook:
                uiManager.SetExclamationIconActive(true);
                uiManager.HideFishName();
                uiManager.HideFishReactionTime();
                break;
            case FishingState.Catching:
                uiManager.SetExclamationIconActive(false);
                break;
            case FishingState.Success:
                if (currentFish != null)
                {
                    uiManager.SetFishName(currentFish.fishName);
                }
                else
                {
                    uiManager.SetFishName("");
                }
                if (reactionTimes.Count > 0)
                {
                    uiManager.SetFishReactionTime(reactionTimes[reactionTimes.Count - 1]);
                }
                else
                {
                    uiManager.SetFishReactionTime(0);
                }
                uiManager.SetExclamationIconActive(false);
                break;
            case FishingState.Failed:
                uiManager.SetExclamationIconActive(false);
                uiManager.HideFishName();
                uiManager.HideFishReactionTime();
                break;
        }
        uiManager.SetBaitIcons(remainingBait);
    }

    public FishData SelectRandomFish()
    {
        // Fish_0のカウントが0なら必ずfish0Dataを返す
        if (PlayerPrefs.GetInt("Fish_0_Count", 0) == 0 && fish0Data != null)
        {
            return fish0Data;
        }
        // Fish_0が既に釣れていれば、availableFishからFish_0以外をランダム選択
        float totalRate = 0f;
        foreach (var fish in availableFish)
        {
            if (fish.fishSpeciesId == 0) continue;
            totalRate += fish.spawnRate;
        }
        float randomValue = Random.Range(0f, totalRate);
        float currentRate = 0f;
        foreach (var fish in availableFish)
        {
            if (fish.fishSpeciesId == 0) continue;
            currentRate += fish.spawnRate;
            if (randomValue <= currentRate)
            {
                return fish;
            }
        }
        // 万一全て除外された場合はavailableFish[0]を返す
        return availableFish[0];
    }

    // デバッグ用メソッド
    private void DebugLog(string message)
    {
        Debug.Log($"[FishingGame] {message}");
    }

    // テスト用メソッド
    public void ResetGame()
    {
        CancelAllAsyncOperations();
        remainingBait = maxBaitCount;
        reactionTimes.Clear();
        canCatch = false;
        ChangeState(FishingState.Waiting);
        DebugLog("ゲームリセット");
        if (uiManager != null)
        {
            uiManager.SetBaitIcons(remainingBait);
            uiManager.HideFishName();
            uiManager.HideFishReactionTime();
            uiManager.HideAverageReactionTime();
        }
        if (lastFishModelInstance != null)
        {
            Destroy(lastFishModelInstance);
            lastFishModelInstance = null;
        }
        // Fish_0の釣得フラグもリセット（必要に応じて）
        isFish0Caught = PlayerPrefs.GetInt("Fish_0_Count", 0) > 0;
    }

    // すべての非同期操作をキャンセル
    private void CancelAllAsyncOperations()
    {
        fishWaitCts?.Cancel();
        fishEscapeCts?.Cancel();
        returnToWaitingCts?.Cancel();
    }
    
    private void GameOverSequence()
    {
        if (uiManager != null)
        {
            float avg = reactionTimes.Count > 0 ? (float)System.Math.Round((float)reactionTimes.Average(), 3) : 0f;
            uiManager.SetAverageReactionTime(avg);
            uiManager.ShowContinueDialog(true);
            uiManager.SetContinueDialogListeners(OnContinueOK, OnContinueNG);
        }
    }

    private void OnContinueOK()
    {
        if (uiManager != null)
        {
            uiManager.ShowContinueDialog(false);
            uiManager.HideAverageReactionTime();
        }
        ResetGame();
    }

    private void OnContinueNG()
    {
        if (uiManager != null)
        {
            uiManager.ShowContinueDialog(false);
        }
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}