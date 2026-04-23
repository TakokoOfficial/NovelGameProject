using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationExitDestory : MonoBehaviour
{
    public GameObject targetObject; // 破壊するオブジェクト
    
    // アニメーションイベントから呼び出される関数
    public void DestroyGameObject()
    {
        Destroy(targetObject);
    }
}
