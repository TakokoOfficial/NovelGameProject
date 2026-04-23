using UnityEngine;
using UnityEngine.UI;

public class FishingGameUIManager : MonoBehaviour
{
    // 釣り竿アニメーション、！アイコン、餌数、スコアなどのUI参照をInspectorで設定
    // 例: public GameObject exclamationIcon;
    // 例: public TMPro.TextMeshProUGUI scoreText;

    // GameButtonの参照
    [SerializeField,Tooltip("竿を投げる（左クリック可）")] 
    private GameObject _castButton;
    [SerializeField,Tooltip("魚を釣り上げる（左クリック可）")]
    private GameObject _hookButton;
    [SerializeField,Tooltip("釣りに戻る（左クリック可）")]
    private GameObject _backtoFishButton;
    
    // UI参照
    public GameObject exclamationIcon;
    public Text averageReactionTimeText;
    public Text fishNameText;
    public Text fishReactionTimeText;

    // 餌アイコン（Image）をInspectorで3つアサイン
    public Image[] baitIcons; // 配列で3つ分

    // 継続確認ダイアログUI
    public GameObject continueDialog;
    public Button okButton;
    public Button ngButton;

    // 魚図鑑UI
    public GameObject InGameUI;
    public GameObject fishBookUI;

    //
    public void SetExclamationIconActive(bool isActive)
    {
        if (exclamationIcon != null)
            exclamationIcon.SetActive(isActive);
    }

    // 餌アイコンの表示/非表示
    public void SetBaitIcons(int remainingBait)
    {
        if (baitIcons != null && baitIcons.Length > 0)
        {
            for (int i = 0; i < baitIcons.Length; i++)
            {
                baitIcons[i].enabled = (i < remainingBait);
            }
        }
    }

    // 平均反応時間の表示・非表示
    public void SetAverageReactionTime(float averageReactionTime)
    {
        if (averageReactionTimeText != null)
        {
            averageReactionTimeText.gameObject.SetActive(true);
            if (averageReactionTime > 0)
                averageReactionTimeText.text = $"今回の平均反応時間: {averageReactionTime:F3}秒";
            else
                averageReactionTimeText.text = "今回の平均反応時間: --";
        }
    }
    public void HideAverageReactionTime()
    {
        if (averageReactionTimeText != null)
            averageReactionTimeText.gameObject.SetActive(false);
    }

    // 釣りあげた魚の名前の表示・非表示
    public void SetFishName(string fishName)
    {
        if (fishNameText != null)
        {
            fishNameText.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(fishName))
                fishNameText.text = $"釣れた魚: {fishName}";
            else
                fishNameText.text = "釣れた魚: --";
        }
    }
    public void HideFishName()
    {
        if (fishNameText != null)
            fishNameText.gameObject.SetActive(false);
    }

    // 釣りあげた時の反応速度の表示・非表示
    public void SetFishReactionTime(float reactionTime)
    {
        if (fishReactionTimeText != null)
        {
            fishReactionTimeText.gameObject.SetActive(true);
            if (reactionTime > 0)
                fishReactionTimeText.text = $"反応時間: {reactionTime:F3}秒";
            else
                fishReactionTimeText.text = "反応時間: --";
        }
    }
    public void HideFishReactionTime()
    {
        if (fishReactionTimeText != null)
            fishReactionTimeText.gameObject.SetActive(false);
    }

    public void ShowContinueDialog(bool show)
    {
        if (continueDialog != null)
            continueDialog.SetActive(show);
    }

    public void SetContinueDialogListeners(UnityEngine.Events.UnityAction onOk, UnityEngine.Events.UnityAction onNg)
    {
        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(onOk);
        }
        if (ngButton != null)
        {
            ngButton.onClick.RemoveAllListeners();
            ngButton.onClick.AddListener(onNg);
        }
    }

    public void ShowFishBook(bool show)
    {
        if (fishBookUI != null)
        {
            InGameUI.SetActive(!show); // 魚図鑑表示時は通常UIを非表示
            fishBookUI.SetActive(show);
        }
    }
    
    public void setCastButtonActive(bool isActive)
    {
        if (_castButton != null)
            _castButton.SetActive(isActive);
    }
    
    public void setHookButtonActive(bool isActive)
    {
        if (_hookButton != null)
            _hookButton.SetActive(isActive);
    }
    
    public void setBacktoFishButtonActive(bool isActive)
    {
        if (_backtoFishButton != null)
            _backtoFishButton.SetActive(isActive);
    }
}
