using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    [Header("現在表示中のデータ")]
    public int dataNow;
    [Header("ここにギャラリーのImageを")]
    public Image galleryImage;
    [Header("ここにギャラリーのTextを")]
    public Text galleryText;
    [Header("ここにギャラリーの名前を表すTextを")]
    public Text galleryNameText;
    [Header("ギャラリーのデータを入力")]
    public GalleryData[] galleryData;

    [System.Serializable]
    public class GalleryData{
        public string title;
        public Sprite sprite;
        
        [TextArea]
        public string text;
    }


    // Start is called before the first frame update
    void Start()
    {
        GalleryShow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GalleryShow(){
        galleryImage.sprite = galleryData[dataNow].sprite;
        galleryText.text = galleryData[dataNow].text;
        galleryNameText.text = galleryData[dataNow].title;
    }

    public void GalleryNext(int howNext){
        dataNow += howNext;
        if(dataNow < 0){
            dataNow = galleryData.Length - 1;
        }

        if(dataNow >= galleryData.Length){
            dataNow = 0;
        }

        GalleryShow();
    }




}
