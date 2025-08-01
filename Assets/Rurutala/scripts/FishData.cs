using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Fishing Game/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("基本情報")]
    public string fishName;
    public GameObject fishModel; // 3Dモデルの参照
    public int fishSpeciesId; // 魚種ID（同じ種類の魚は同じID）

    [Header("釣りの設定")]
    [Range(0.5f, 5.0f)]
    public float escapeTime = 2.0f; // 魚が逃げるまでの時間

    [Range(0.01f, 1.0f)]
    public float spawnRate = 0.1f; // 出現確率

    [Header("レア魚設定")]
    public bool isRare = false;
}