using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    [Header("現在のゲームの進行具合（ステージ）")]
    public int stage;
    [Header("ここにアイコンの名称が入力されるTextをアタッチ")]
    public Text iconText;
    [Header("ここにアイコンの詳細が入力されるTextをアタッチ")]
    public Text iconTextSub;

    [Header("ここにTextManagerをアタッチ")]
    public TextManager textManager;


    [Header("ここにマップアイコンの出現条件だったりを設定する")]
    public IconData[] iconData;

    [System.Serializable]
    public class IconData{
        [Header("アイコンの名前")]
        public string name;

        [Header("現在のアイコンの状態。-1だとアイコンは非表示になる")]
        public int situation = -1;

        [Header("アイコンをアタッチ")]
        public GameObject[] iconGOs;
        [Header("アイコンの状態を説明する文章を入力")]
        public string[] iconInfo;
    }

    [Header("ここでゲームの進行具合（ステージ）に合わせたアイコンの設定を行う")]
    public StageData[] stageData;

    [System.Serializable]
    public class StageData{
        [Header("ステージの各アイコンの初期設定")]
        public int[] firstIconSituation;

        [Header("ステージで更新されるアイコンの番号を入力")]
        public int[] changeIconNum;
        [Header("ステージで更新されるアイコンがどう変更されるかの番号を入力")]
        public int[] changeIconType;
        [Header("開放する条件となるフラグを設定")]
        public IconFrag[] iconFrag;
        [Header("変更されたか")]
        public bool beChanged;
    }


    [System.Serializable]
    public class IconFrag{
        public int[] flag;
    }

    // Start is called before the first frame update
    void Start()
    {
        StageReset();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < stageData.Length; i++){
            for(int j = 0; j < stageData[i].iconFrag.Length; j++){
                for(int k = 0; k < stageData[i].iconFrag[j].flag.Length; k++){
                    if(!textManager.flags[stageData[i].iconFrag[j].flag[k]].isFlag){
                        break;
                    }

                    if(k == stageData[i].iconFrag[j].flag.Length - 1 && !stageData[i].beChanged){ //フラグをすべて満たす場合
                        iconData[stageData[i].changeIconNum[j]].situation = stageData[i].changeIconType[j];
                        stageData[i].beChanged = true;
                        MapShow();
                    }
                }
            }
        }
    }

    //ステージを進めて、マップのアイコンをstageDataのものに更新する
    public void NextStage(){
        if(stage > stageData.Length - 1){
            return;
        }
        stage++;

        StageReset();

    }

    //現在のステージの初期状態に設定する
    public void StageReset(){
        for(int i = 0; i < iconData.Length; i++){
            iconData[i].situation = stageData[stage].firstIconSituation[i];
        }
    }

    //マップのアイコンを更新する
    public void MapShow(){
        for(int i = 0; i < iconData.Length; i++){
            for(int j = 0; j < iconData[i].iconGOs.Length; j++){
                iconData[i].iconGOs[j].SetActive(false);
            }
        }        
        for(int i = 0; i < iconData.Length; i++){
            if(iconData[i].situation != -1){
                iconData[i].iconGOs[iconData[i].situation].SetActive(true);
            }
        }
    }

    public void IconTextShow(int type){
        iconText.text = iconData[type].name;
        if(iconData[type].situation != -1){
            iconTextSub.text = iconData[type].iconInfo[iconData[type].situation];
        }
        else{
            iconTextSub.text = "";
        }
    }

}
