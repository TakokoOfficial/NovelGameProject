using UnityEngine;

/// <summary>
/// バケツに演出魚を入れる
/// GetFishが呼ばれると配列の上から順に魚が入る
/// </summary>
public class BucketFish : MonoBehaviour
{
    [SerializeField] private GameObject[] fishSlots;
    
    private int currentIndex = 0;

    private void Start()
    {
        // すべてのスロットを非アクティブにする
        foreach (var slot in fishSlots)
        {
            if (slot != null)
            {
                slot.SetActive(false);
            }
        }
        currentIndex = 0;
    }

    public void GetFish()
    {
        // 配列の範囲内かチェック
        if (currentIndex < fishSlots.Length && fishSlots[currentIndex] != null)
        {
            fishSlots[currentIndex].SetActive(true);
            currentIndex++;
        }
    }

    public void ResetBucket()
    {
        // すべてのスロットを非アクティブにする
        foreach (var slot in fishSlots)
        {
            if (slot != null)
            {
                slot.SetActive(false);
            }
        }
        currentIndex = 0;
    }
}