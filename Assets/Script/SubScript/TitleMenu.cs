using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [Header("ここにメニュー画面をたくさんアタッチ")]
    public GameObject[] Menus;


    [Header("ここにカーソルを合わせると反応するTextをアタッチ")]
    public Text[] texts;
    [Header("ここにカーソルを合わせた時のTextの色を設定")]
    public Color onColor;
    [Header("ここにカーソルを離した時のTextの色を設定")]
    public Color removeColor;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuChange(int type){
        SEPlayer.instance.SE(8,1);

        for(int i = 0; i < Menus.Length; i++){
            Menus[i].SetActive(false);
        }
        Menus[type].SetActive(true);

        for(int i = 0; i < texts.Length; i++){
            RemoveTextColor(i);
        }
    }

    public void OnTextColor(int type){
        SEPlayer.instance.SE(4,1);
        texts[type].color = onColor;
    }

    public void RemoveTextColor(int type){
        texts[type].color = removeColor;
    }

    public void GameStart(){
        SceneManager.LoadScene("Game");
    }

}
