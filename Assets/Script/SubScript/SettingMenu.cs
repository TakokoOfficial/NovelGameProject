using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{

    [Header("ここにSettingのMenuをアタッチ")]
    public GameObject SettingMenuGO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MenuSettingMenuTandF(){
        SettingMenuGO.SetActive(!SettingMenuGO.activeSelf);
    }

    public void ReturnTitle(){
        SceneManager.LoadScene("Title");
    }
}
