using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceScript : MonoBehaviour
{
    [Header("選択肢の番号")]
    public int ChoiceNumber;

    [Header("ここにTextManagerが入る")]
    public TextManager textManager;

    // Start is called before the first frame update
    void Start()
    {
        textManager = GameObject.FindGameObjectWithTag("TextManager").GetComponent<TextManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChoiceClick(){
        textManager.choiceSet(ChoiceNumber);
    }
}
