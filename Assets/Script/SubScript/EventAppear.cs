using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAppear : MonoBehaviour
{
    [Header("ここにゲームオブジェクトの出現に必要なフラグの番号を設定")]
    public int[] flagTypes;

    [Header("ここに出現させるゲームオブジェクトを設定")]
    public GameObject[] appearGameObjects;

    [Header("ここにTextManagerをアタッチ")]
    public TextManager textManager;

    void Update()
    {
        for(int i = 0; i < flagTypes.Length; i++){
            if(!textManager.flags[flagTypes[i]].isFlag)
                return;
        }

        for(int i = 0; i < appearGameObjects.Length; i++){
            appearGameObjects[i].SetActive(true);
        }
        Destroy(this);
    }
}
