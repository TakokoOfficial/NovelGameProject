using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [Header("アイテムの番号")]
    public int ItemNumber;
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

    public void ItemClick(){
        textManager.ItemInfoShow(ItemNumber);
    }
}
