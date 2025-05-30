using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public static SEPlayer instance;

    [Header("マスター音量")]
    public float Volume = 1;

    [Header("効果音のデータを入力")]
    public SEData[] sEData;

    [System.Serializable]
    public class SEData{
        public AudioClip audioClip;
        public float volume = 1;
        public float interval = 0.05f;
        public float pitch = 1;
        public float delay = 0;

        [SerializeField]
        public float sEsTime = 0;
    }

    [Header("ここにAudioSourceを")]
    public AudioSource audioSource;


    private float oldVolume;

    private void Awake()
    {
        instance = this;
        /*
        for(int i = 0; i < sEData.Length; i++){
            sEData[i].sEsTime = sEData[i].audioClip.length;
        }
        */
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SECounting();
    }


    //効果音を再生する関数
    public void SE(int i, float pitch){
        if(sEData[i].sEsTime == 0)
        {
            StartCoroutine(SEPlay(i,pitch));
            sEData[i].sEsTime = sEData[i].interval;
        }
    }

    //効果音を再生する関数（ピッチを指定しないもの）
    public void SE(int i){
        if(sEData[i].sEsTime == 0)
        {
            StartCoroutine(SEPlay(i,1));
            sEData[i].sEsTime = sEData[i].interval;
        }
    }


    IEnumerator SEPlay(int i, float pitch)
    {
        yield return new WaitForSeconds(sEData[i].delay);
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = sEData[i].volume * Volume;
        audioSource.clip = sEData[i].audioClip;
        audioSource.spatialBlend = 0f;
        audioSource.pitch = sEData[i].pitch * pitch;
        audioSource.Play();
        Destroy(tempAudio, sEData[i].audioClip.length * 4 * (1 / sEData[i].pitch));
    }

    public void SEPlay(int i)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = sEData[i].volume * Volume;
        audioSource.clip = sEData[i].audioClip;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
        Destroy(tempAudio, sEData[i].audioClip.length * 4);
    }

    //効果音のインターバルを計算する関数
    void SECounting()
    {
        for(int i = 0; i < sEData.Length; i++)
        {
            if(sEData[i].sEsTime > 0)
            {
                sEData[i].sEsTime -= Time.deltaTime;
            }
            else
            {
                sEData[i].sEsTime = 0;
            }
        }
    }

    //効果音を変更する際に使う関数
    public void SEVolume(float i)
    {
        Volume = i;
    }

}
