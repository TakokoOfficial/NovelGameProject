using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDeleter : MonoBehaviour
{
    [Header("ここにゲームオブジェクトの削除に必要なフラグの番号を設定")]
    public int[] flagTypes;

    [Header("ここにTextManagerをアタッチ")]
    public TextManager textManager;

    void Update()
    {
        for(int i = 0; i < flagTypes.Length; i++){
            if(!textManager.flags[flagTypes[i]].isFlag)
                return;
        }

        Destroy(gameObject);
    }
}
