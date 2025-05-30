using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoMenu : MonoBehaviour
{
    [Header("現在表示中のメモ")]
    public int MemoTypeNow;

    [Header("ここにTextManagerをアタッチ")]
    public TextManager textManager;

    [Header("アイデアのあれこれを設定できるよ")]
    public MemoData[] memoData;

    [System.Serializable]
    public class MemoData{
        public string name;

        [Header("ここにアイデアのメモのゲームオブジェクトをアタッチ")]
        public GameObject gameObject;

        [Header("参照するTextManagerのフラグの番号")]
        public int FragNumber;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MemoShow(){
        for(int i = 0; i < memoData.Length; i++){
            memoData[i].gameObject.SetActive(false);
        }
        if(textManager.flags[memoData[MemoTypeNow].FragNumber].isFlag){
            memoData[MemoTypeNow].gameObject.SetActive(true);
        }
    }

    public void MemoNext(){
        int nextMemo = -1;
        for(int i = MemoTypeNow + 1; i < memoData.Length; i++){
            if(textManager.flags[memoData[i].FragNumber].isFlag){
                nextMemo = i;
                break;
            }
        }

        if(nextMemo == -1){ //MemoTypeNow以降の表示可能なメモが無かった場合
            int backMemo = MemoTypeNow;
            for(int i = 0; i < memoData.Length; i++){
                if(textManager.flags[memoData[i].FragNumber].isFlag){
                    backMemo = i;
                    break;
                }
            }
            MemoTypeNow = backMemo;

        }
        else{ //MemoTypeNow以降に表示可能なメモがあった場合
            MemoTypeNow = nextMemo;
        }

        MemoShow();
    }



    public void MemoBack(){
        int nextMemo = -1;
        for(int i = MemoTypeNow - 1; i >= 0; i--){
            if(textManager.flags[memoData[i].FragNumber].isFlag){
                nextMemo = i;
                break;
            }
        }

        if(nextMemo == -1){ //MemoTypeNow以前に表示可能なメモが無かった場合
            int backMemo = MemoTypeNow;
            for(int i = memoData.Length - 1; i > 0; i--){
                if(textManager.flags[memoData[i].FragNumber].isFlag){
                    backMemo = i;
                    break;
                }
            }
            MemoTypeNow = backMemo;

        }
        else{ //MemoTypeNow以前に表示可能なメモがあった場合
            MemoTypeNow = nextMemo;
        }

        MemoShow();
    }
}
