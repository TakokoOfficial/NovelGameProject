using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//音楽を管理するスクリプト

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer ist;

    [Header("音楽のマスター音量")]
    [Range(0f, 1f)]
    public float Volume;

    [Header("現在流している音楽の番号")]
    public int BGMType = 0;
    [Header("現在流しているミュージックリストの番号")]
    public int MusicListType = -1;

    [Header("音楽をここに")]
    public AudioClip[] BGMs;

    [Header("各音楽の音量設定をする")]
    [Range(0f, 1f)]
    public float[] BGMVolume;

    [Header("ミュージックリストを設定する")]
    public MusicSet[] musicSet;



    [Header("ここにAudioSourceをアタッチ")]
    public AudioSource AudioSource;



    [SerializeField]
    //各音楽の長さがここに格納される
    float[] BGMsLength;

    //現在の音楽の再生時間
    float BGMTime;

    [SerializeField]
    //音楽をフェードアウトさせたりフェードインするために使う
    float BGMFade;

    Coroutine[] musicListCoroutine;


    [System.Serializable]
    public class MusicSet
    {
        public int[] music;
    }


    void Awake()
    {
        ist = this;
        MusicLenghtSet();
        musicListCoroutine = new Coroutine[9];

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //各音楽の長さを格納する
    void MusicLenghtSet()
    {
        BGMsLength = new float[BGMs.Length];
        for (int i = 0; i < BGMs.Length; i++)
        {
            BGMsLength[i] = BGMs[i].length + 10;
        }
    }




    //音楽を流す。１つ目の引数はどの音楽を流すか
    public void Playing(int type)
    {
        //音楽を再生
        if(type < BGMsLength.Length && BGMs[type] != null)
        {
            AudioSource.clip = BGMs[type];
            AudioSource.volume = Volume * BGMVolume[type];
            AudioSource.Play();
            BGMType = type;
        }

    }

    IEnumerator MusicFade(float fadeTime)
    {
        if (fadeTime == 0)
            yield break;

        BGMFade = 0;

        float BeforeBGMVolume = AudioSource.volume;

        if (fadeTime > 0) //フェードアウトの場合
        {
            //フェードアウト中
            while (BGMFade < fadeTime)
            {
                yield return null;
                BGMFade += Time.deltaTime;
                AudioSource.volume = BeforeBGMVolume * (1 - BGMFade / fadeTime);
            }
            AudioSource.volume = 0;
            yield break;
        }
        else //フェードインの場合
        {
            //フェードイン中
            while (BGMFade < -fadeTime)
            {
                yield return null;
                BGMFade += Time.deltaTime;
                AudioSource.volume = BGMVolume[BGMType] * Volume * (BGMFade / -fadeTime);
            }
            AudioSource.volume = BGMVolume[BGMType] * Volume;
            yield break;
        }
    }


    //音楽を停止させる
    public void StopMusic(float fadeOut)
    {
        for(int i = 0; i < musicListCoroutine.Length; i++){
            if(musicListCoroutine[i] != null){
                StopCoroutine(musicListCoroutine[i]);
                musicListCoroutine[i] = null;
            }
        }
        
        musicListCoroutine[0] = StartCoroutine(MusicFade(fadeOut));
    }
    


    //ミュージックリストからランダムに音楽を流し始める
    public void StartMusicList(int type)
    {
        for(int i = 0; i < musicListCoroutine.Length; i++){
            if(musicListCoroutine[i] != null){
                StopCoroutine(musicListCoroutine[i]);
                musicListCoroutine[i] = null;
            }
        }


        musicListCoroutine[1] = StartCoroutine(FadeOutAndMusicList(type));
    }


    IEnumerator FadeOutAndMusicList(int type)
    {
        if (type < 0 || type >= musicSet.Length)
        {
            yield break;
        }

        // フェードアウト処理
        musicListCoroutine[2] = StartCoroutine(MusicFade(2f)); // 2秒フェードアウト（調整可）
        yield return new WaitForSeconds(2f);

        // ミュージックリストの種類を更新
        MusicListType = type;
        
        while (MusicListType == type)
        {
            int[] musicList = musicSet[type].music;
            if (musicList.Length == 0)
            {
                yield break;
            }

            int randomIndex = Random.Range(0, musicList.Length);
            int selectedMusic = musicList[randomIndex];

            // 4秒フェードインしながら再生
            musicListCoroutine[3] = StartCoroutine(MusicFade(-2f));
            Playing(selectedMusic); 

            // 次の曲まで待機
            yield return new WaitForSeconds(BGMsLength[selectedMusic]);
        }
    }




    //UIのSliderから音量設定するときに使う
    public void VolumeSetting(float v)
    {
        Volume = v;
        AudioSource.volume = v * BGMVolume[BGMType];
    }
}
