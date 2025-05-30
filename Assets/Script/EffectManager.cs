using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("ここにTitleのアニメーターをアタッチ")]
    public Animator TitleAnimator;

    [Header("ここに暗転時に使う黒い画像のアニメーターをアタッチ")]
    public Animator BlackOutAnim;


    public static EffectManager instance;

    void Awake(){
        instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LetsBlackOut(){
        BlackOutAnim.SetBool("BlackOut",true);
        StartCoroutine(ResetAnimator(BlackOutAnim,"BlackOut"));
    }

    public void LetsShowTitle(){
        TitleAnimator.SetBool("appear",true);
        StartCoroutine(ResetAnimator(TitleAnimator,"appear"));
    }

    IEnumerator ResetAnimator(Animator anim, string name){
        yield return null;
        anim.SetBool("" + name,false);
    }
}
