using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorChanger : MonoBehaviour
{
    [Header("ここにカーソルを合わせると反応するTextをアタッチ")]
    public Text texts;
    [Header("ここにカーソルを合わせた時のTextの色を設定")]
    public Color onColor;
    [Header("ここにカーソルを離した時のTextの色を設定")]
    public Color removeColor;

    private Color oldColor;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){
        RemoveTextColor();
    }

    public void OnTextColor(){
        if(oldColor != onColor)
            SEPlayer.instance.SE(4,1);

        texts.color = onColor;
        oldColor = texts.color;
    }

    public void RemoveTextColor(){
        texts.color = removeColor;
        oldColor = texts.color;
    }
}
