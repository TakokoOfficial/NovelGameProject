using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [Header("回転速度")]
    public float rotationSpeed = 1;
    
    // Update is called once per frame
    void Update()
    {
        // Z軸を中心に回転
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}

