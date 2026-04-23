using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOnTheWater : MonoBehaviour
{
    [SerializeField]
    public FishingGameManager _fishingGameManager;
    
    public void RingOnTheWaterEffect()
    {
        if (_fishingGameManager != null)
        {
            _fishingGameManager.EnableRingOnTheWaterEffect();
            _fishingGameManager.SetFishHookTime();
        }
        else
        {
            Debug.LogWarning("FishingGameManagerがアサインされていません。");
        }
    }
    
    
}
