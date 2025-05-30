using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("ここにMusicPlayerを")]
    public MusicPlayer musicPlayer;
    [Header("ここにMusicPlayerのSliderを")]
    public Slider musicSlider;
    [Header("ここにSEPlayerを")]
    public SEPlayer sEPlayer;
    [Header("ここにSEPlayerのSliderを")]
    public Slider sESlider;

    [Header("ゲームクリア時に表示するゲームオブジェクトをアタッチ")]
    public GameObject[] gameClearObject;

    public static class GameScoreStatic{
        [Header("ゲームをクリアしたか")]
        public static bool isGameClear;

        [Header("音楽の音量")]
        public static float musicVolume = 1;
        [Header("効果音の音量")]
        public static float seVolume = 1;
    }

    public static GameManager instance;

    void Awake(){
        instance = this;

        int isCleared = PlayerPrefs.GetInt("isGameClear",0);
        if(isCleared == 1){
            GameScoreStatic.isGameClear = true;
        }
        else{
            GameScoreStatic.isGameClear = false;
        }

        GameClearObjectReload();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        musicVolumeChange(GameScoreStatic.musicVolume);
        seVolumeChange(GameScoreStatic.seVolume);

        GameClearObjectReload();

    }

    // Update is called once per frame
    void Update()
    {
        /*
        musicVolumeChange(GameScoreStatic.musicVolume);
        seVolumeChange(GameScoreStatic.seVolume);
        */
    }

    public void musicVolumeChange(float f){
        musicPlayer.VolumeSetting(f);
        musicSlider.value = f;
        GameScoreStatic.musicVolume = f;
    }
    public void seVolumeChange(float f){
        sEPlayer.SEVolume(f);
        sESlider.value = f;
        GameScoreStatic.seVolume = f;
    }

    [ContextMenu("GameClearObjectReload")]

    public void GameClearObjectReload(){
        for(int i = 0; i < gameClearObject.Length; i++){
            gameClearObject[i].SetActive(false);
        }

        if(!GameScoreStatic.isGameClear)
            return;

        for(int i = 0; i < gameClearObject.Length; i++){
            gameClearObject[i].SetActive(true);
        }
    }

    [ContextMenu("GameCleared")]
    public void GameCleared(){
        GameScoreStatic.isGameClear = true;
        PlayerPrefs.SetInt("isGameClear",1);
    }
}
