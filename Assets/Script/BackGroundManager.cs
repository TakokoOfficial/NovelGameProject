using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundManager : MonoBehaviour
{
    [Header("ここに背景たちをアタッチ")]
    public GameObject[] BackGroundGOs;
    [Header("背景たちの名前を入力")]
    public string[] BackGroundGOsName;


    [Header("ここにInputをアタッチ")]
    public GameObject InputGO;
    [Header("背景の名前が入力されるTextをアタッチ")]
    public Text BackGroundGOsNameText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BackGroundChangeCoroutine(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //背景を
    public void BackGroundChange(int type, int way){
        StartCoroutine(BackGroundChangeCoroutine(type, way));
    }

    IEnumerator BackGroundChangeCoroutine(int type, int way){
        InputGO.SetActive(true);
        BackGroundGOsNameText.text = "";
        switch(way){
            //パッと反転
            case 0:
                ChangeBackGroundImage(type);
                break;
            //1秒間フェードアウト、1秒間フェードイン
            case 1:
                EffectManager.instance.LetsBlackOut();
                yield return new WaitForSeconds(1.5f);
                ChangeBackGroundImage(type);
                yield return new WaitForSeconds(1.5f);
                break;
            //1秒間フェードアウト、1秒間フェードイン、1秒経ってタイトル表示
            case 2:
                EffectManager.instance.LetsBlackOut();
                yield return new WaitForSeconds(1.5f);
                ChangeBackGroundImage(type);
                yield return new WaitForSeconds(2.5f);
                EffectManager.instance.LetsShowTitle();

                break;
        }

        BackGroundGOsNameText.text = "" + BackGroundGOsName[type];

        InputGO.SetActive(false);
        yield break;
    }

    public void ChangeBackGroundImage(int type){
        for(int i = 0; i < BackGroundGOs.Length; i++){
            BackGroundGOs[i].SetActive(false);
        }
        BackGroundGOs[type].SetActive(true);
    }


}
