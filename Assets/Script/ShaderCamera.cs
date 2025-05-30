using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]

public class ShaderCamera : MonoBehaviour
{
    public Material[] materials;
    public string[] materialsInfo;
    public Text ShaderText;
    public int nowMaterial;

    public void Start(){

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (materials[nowMaterial] != null)
        {
            Graphics.Blit(src, dest, materials[nowMaterial]);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void NextMaterial(){
        nowMaterial++;
        if(nowMaterial == materials.Length){
            nowMaterial = 0;
        }
        ShaderText.text = "" + materialsInfo[nowMaterial];
    }
}
