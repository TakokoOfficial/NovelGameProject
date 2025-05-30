using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPusher : MonoBehaviour
{
    [Header("ここに発生させるmessageSetを設定")]
    public int useMessageSet;
    [Header("ここにmessageSetの発生に必要なフラグの番号を設定")]
    public int[] flagTypes;

    [Header("ここにTextManagerをアタッチ")]
    public TextManager textManager;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < flagTypes.Length; i++){
            if(!textManager.flags[flagTypes[i]].isFlag)
                return;
        }

        if(textManager.messageCoroutine == null){
            textManager.messageStart(useMessageSet);
            Destroy(this);
        }
    }
}
