using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [Header("ここにスタート時に消しておきたいゲームオブジェクトをアタッチ")]
    public GameObject[] willTrueGameObjects;
    [Header("ここにスタート後に消したいゲームオブジェクトをアタッチ")]
    public GameObject[] willDestroyGameObjects;
    // Start is called before the first frame update


    void Awake(){
        for (int i = 0; i < willTrueGameObjects.Length; i++){
            willTrueGameObjects[i].SetActive(false);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFunction(){
        for (int i = 0; i < willTrueGameObjects.Length; i++){
            willTrueGameObjects[i].SetActive(true);
        }
        for (int i = 0; i < willDestroyGameObjects.Length; i++){
            Destroy(willDestroyGameObjects[i]);
        }
    }
}
