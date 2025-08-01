using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishBooksManager : MonoBehaviour
{
    [Header("UI要素")]
    public Button leftButton;
    public Button rightButton;
    public Text fishNameText; // 魚の名前を表示するテキスト
    public Text fishBestTimeText;
    
    [Header("魚のデータ")]
    public FishData[] allFishData; // すべての魚データ
    public Material unknownFishMaterial; // 未釣得魚用の黒いマテリアル
    public GameObject rareEffect; // レア魚用のエフェクト
    
    [Header("設定")]
    public int preloadCount = 1; // 前後何体まで事前生成するか
    
    private int currentFishId = 0;
    private Dictionary<int, GameObject> fishInstances = new Dictionary<int, GameObject>();
    private List<int> availableFishIds = new List<int>();
    
    
    private void Start()
    {
        InitializeAvailableFish();
        SetupButtons();
        ShowCurrentFish();
        PreloadAdjacentFish();
    }

    private void OnEnable()
    {
        ShowCurrentFish();
        PreloadAdjacentFish();
    }

    private void OnDisable()
    {
        // 魚情報を非表示
        HideFishName();
        HideFishBestTime();

        // 生成したインスタンスを削除
        foreach (var kvp in fishInstances)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        fishInstances.Clear();
    }
    
    private void InitializeAvailableFish()
    {
        availableFishIds.Clear();
        
        // PlayerPrefsに登録されている魚IDを収集
        for (int i = 0; i < allFishData.Length; i++)
        {
            if (allFishData[i] != null)
            {
                availableFishIds.Add(i);
            }
        }
        
        // 最初の魚IDを設定
        if (availableFishIds.Count > 0)
        {
            currentFishId = availableFishIds[0];
        }
    }
    
    private void SetupButtons()
    {
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(ShowPreviousFish);
        }
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(ShowNextFish);
        }
    }
    
    private void ShowPreviousFish()
    {
        int currentIndex = availableFishIds.IndexOf(currentFishId);
        if (currentIndex > 0)
        {
            currentFishId = availableFishIds[currentIndex - 1];
            ShowCurrentFish();
            PreloadAdjacentFish();
        }
    }
    
    private void ShowNextFish()
    {
        int currentIndex = availableFishIds.IndexOf(currentFishId);
        if (currentIndex < availableFishIds.Count - 1)
        {
            currentFishId = availableFishIds[currentIndex + 1];
            ShowCurrentFish();
            PreloadAdjacentFish();
        }
    }
    
    private void ShowCurrentFish()
    {
        // 現在表示中の魚を非表示
        HideAllFish();

        // 現在の魚を表示
        GameObject fishInstance = GetOrCreateFishInstance(currentFishId);
        if (fishInstance != null)
        {
            // レア状態を確認
            bool isRare = PlayerPrefs.GetInt($"Fish_{currentFishId}_Rare", 0) == 1;
        
            // レアエフェクトの表示制御
            if (rareEffect != null)
            {
                rareEffect.SetActive(isRare);
            }
        
            fishInstance.SetActive(true);
            UpdateFishInfo(currentFishId);
        }
    }
    
    private void UpdateFishInfo(int fishId)
    {
        if (fishId < 0 || fishId >= allFishData.Length || allFishData[fishId] == null)
            return;

        FishData fishData = allFishData[fishId];
        int caughtCount = PlayerPrefs.GetInt($"Fish_{fishId}_Count", 0);

        if (caughtCount > 0)
        {
            // 釣得済みの場合は名前と最高記録を表示
            SetFishName(fishData.fishName);
            float bestTime = PlayerPrefs.GetFloat($"Fish_{fishId}_BestTime", float.MaxValue);
            SetFishBestTime(bestTime);
        }
        else
        {
            // 未釣得の場合は「？？？」を表示
            SetFishName("？？？");
            HideFishBestTime();
        }
    }
    
    private void SetFishName(string fishName)
    {
        if (fishNameText != null)
        {
            fishNameText.gameObject.SetActive(true);
            fishNameText.text = fishName;
        }
    }

    private void SetFishBestTime(float bestTime)
    {
        if (fishBestTimeText != null)
        {
            fishBestTimeText.gameObject.SetActive(true);
            fishBestTimeText.text = $"最高記録: {bestTime:F3}秒";
        }
    }

    private void HideFishBestTime()
    {
        if (fishBestTimeText != null)
        {
            fishBestTimeText.gameObject.SetActive(false);
        }
    }

    private void HideFishName()
    {
        if (fishNameText != null)
        {
            fishNameText.gameObject.SetActive(false);
        }
    }
    
    private void PreloadAdjacentFish()
    {
        int currentIndex = availableFishIds.IndexOf(currentFishId);
        
        // 前後の魚を事前生成
        for (int offset = -preloadCount; offset <= preloadCount; offset++)
        {
            if (offset == 0) continue; // 現在の魚はスキップ
            
            int targetIndex = currentIndex + offset;
            if (targetIndex >= 0 && targetIndex < availableFishIds.Count)
            {
                int fishId = availableFishIds[targetIndex];
                GetOrCreateFishInstance(fishId);
            }
        }
    }
    
    private GameObject GetOrCreateFishInstance(int fishId)
    {
        if (fishInstances.ContainsKey(fishId))
        {
            return fishInstances[fishId];
        }
        
        return CreateFishInstance(fishId);
    }
    
    private GameObject CreateFishInstance(int fishId)
    {
        if (fishId < 0 || fishId >= allFishData.Length || allFishData[fishId] == null)
        {
            return null;
        }
        
        FishData fishData = allFishData[fishId];
        GameObject fishInstance = null;
        
        // 魚モデルを生成
        if (fishData.fishModel != null)
        {
            fishInstance = Instantiate(fishData.fishModel);
            
            // FishLookスクリプトを追加
            if (fishInstance.GetComponent<FishLook>() == null)
            {
                fishInstance.AddComponent<FishLook>();
            }
            
            // 釣得状況を確認
            int caughtCount = PlayerPrefs.GetInt($"Fish_{fishId}_Count", 0);
            bool isRare = PlayerPrefs.GetInt($"Fish_{fishId}_Rare", 0) == 1;
            
            if (caughtCount == 0)
            {
                // 未釣得の場合は黒いマテリアルを適用
                ApplyUnknownMaterial(fishInstance);
            }
            
            // 最初は非表示にしておく
            fishInstance.SetActive(false);
        }
        
        // インスタンスを登録
        if (fishInstance != null)
        {
            fishInstances[fishId] = fishInstance;
        }
        
        return fishInstance;
    }
    
    private void ApplyUnknownMaterial(GameObject fishInstance)
    {
        if (unknownFishMaterial == null) return;
        
        Renderer[] renderers = fishInstance.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = unknownFishMaterial;
            }
            renderer.materials = materials;
        }
    }
    
    private void HideAllFish()
    {
        foreach (var kvp in fishInstances)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetActive(false);
            }
        }
    }
    
    private void OnDestroy()
    {
        // 生成したインスタンスを削除
        foreach (var kvp in fishInstances)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        fishInstances.Clear();
    }
}