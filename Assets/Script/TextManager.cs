using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [TextArea]
    [Header("MessageSetの使い方")]
    public string MessageSetHowToUse = 
    "MessageSetの使い方\n" + 
    "コマンドでメッセージを操作できる\n" +
    "cm_set=nnn：nnn番目のmessageSetを読み込む\n" +
    "cm_stage=nnn_mmm：nnn番の場面にmmmの演出で転換する\n" + 
    "cm_end：MessageSetの再生を終了する\n" + 
    "cm_flagTrue=nnn：nnn番目のフラグを立てる\n" +
    "cm_flagFalse=nnn：nnn番目のフラグを折る\n" +
    "cm_flagIF=nnn_mmm：nnn番目のフラグが立っていたらmmm個のメッセージを飛ばす\n" +
    "cm_choice2=nnn_mmm：次の２つのメッセージを選択肢にする。１つめの選択をするとnnn個のメッセージを飛ばし、２つ目の選択をするとmmm個のメッセージを飛ばす。\n" + 
    "cm_choice3=nnn_mmm_ooo：（以下略）\n" + 
    "cm_itemGet=nnn：nnn番目のアイテムを手に入れる\n" + 
    "cm_itemUse=nnn：nnn番目のアイテムを消費する\n" + 
    "cm_itemIF=nnn_mmm：nnn番目のアイテムを持っていたらmmm個のメッセージを飛ばす\n" + 
    "cm_wait=nnn：nnnフレーム待機する\n" + 
    "cm_clear：テキストをクリアする\n" +
    "cm_next=nnn：nnnフレーム経ったら強制的に次のメッセージを読み込む\n" +
    "cm_goalIn=nnn：nnn番目の目標を表示する\n" +
    "cm_goalOut：目標を消し飛ばす\n" +
    "cm_count=nnn_mmm：nnn秒のカウントダウンを始め、終わり次第mmmのmessageSetを再生する\n" +
    "cm_stopCount：カウントダウンを停止する\n" + 
    "cm_SpecialShow=nnn：nnnつ目のスペシャルなメッセージを表示する\n" + 
    "cm_SpecialClear：スペシャルなメッセージを非表示にする\n" +
    "cm_SpecialChange=nnn：メッセージを表示する場所をnnn - 1番目のものに変更する。nnn = 0だと普段の場所\n" +
    "cm_window=nnn：nnn番目のWindowsゲームオブジェクトを出現させる\n" + 

    "cm_gameClear：エンディング後フラグ用。別の場所でこのスクリプトを流用する場合はこれを除外すること\n" + 


    "cm_SE=nnn_mmm：nnn番目の効果音をmmmくらいのピッチで再生する\n" + 
    "cm_music=nnn：nnn番目の音楽リストを再生する\n" + 
    "cm_stopMusic：音楽を停止する\n" + 
    "";

    [Header("登場人物の名前を入力")]
    public string[] humanNames;
    [Header("登場人物の喋る効果音の番号を入力")]
    public int[] humanSE;

    [Header("各セリフセットを入力")]
    public MessageSet[] messageSet;


    [System.Serializable]
    public class MessageSet{
        public string SetTitle;

        [Header("メッセージのデータを入力")]
        public MessageData[] messageDatas;
    }

    [System.Serializable]
    public class MessageData{
        public string message;
        public int name;
    }

    [Header("各フラグを入力")]
    public Flags[] flags;

    [System.Serializable]
    public class Flags{
        public string flagName;
        public bool isFlag;
    }


    [Header("カーソルを合わせた際に表示する物の名前を入力")]
    public GoodsText[] goodsText;

    [System.Serializable]
    public class GoodsText{
        public string goodsName;
        [Header("未探索時の表示テキスト")]
        public string nullName;
        [Header("Goodsのフラグ")]
        public bool isFlag;
    }


    [Header("アイテムのデータを入力")]
    public ItemData[] itemData;

    [System.Serializable]
    public class ItemData{
        public string name;
        public Sprite image;
        public string info;
    }

    [Header("特殊なメッセージを表示するためのテキストをアタッチ")]
    public SpecialText[] specialText;

    [System.Serializable]
    public class SpecialText{
        public string title;
        [Header("特殊なメッセージの親オブジェクトをアタッチ")]
        public GameObject textParent;
        [Header("特殊なメッセージのセリフを表示するTextをアタッチ")]
        public Text message;
        [Header("特殊なメッセージの名前を表示するTextをアタッチ")]
        public Text name;
        [Header("特殊なメッセージのデフォルトの内容を入力")]
        public string defaultInput;
    }

    [Header("TextManagerのSetActiveで出現できるオブジェクト達をアタッチ")]
    public GameObject[] Windows; 

    [Header("目標を入力")]
    public string[] goals;

    [Header("テスト用の数字")]
    public int TestNumber;
    [Header("メッセージのめくる速度")]
    public float messageSpeed;
    [Header("現在のメッセージを再生する場所の種類。0であればいつもの場所。それ以外は特別な場所")]
    public int messagePlace;

    [Header("現在再生中のMessageSet")]
    public int SetNow;
    [Header("現在再生中のmessage")]
    public int messageNow;
    [Header("現在再生中のmessageSetコルーチン")]
    public Coroutine messageCoroutine;
    [Header("選択肢の入力を待機中か判定するbool")]
    public bool isChoicing = true;
    [Header("選択した選択肢")]
    public int beChose;
    [Header("現在所持中のアイテム")]
    public GameObject[] ItemGot;



    [Header("ここにセリフの入るTextをアタッチ")]
    public Text MessageText; 
    [Header("ここにナレーターのセリフの入るTextをアタッチ")]
    public Text NoOneMessageText; 
    [Header("ここにセリフの入るTextをアタッチ")]
    public Text NameText; 
    [Header("ここに現在地の入るTextをアタッチ")]
    public Text StageText; 
    [Header("ここに現在の目的が入るTextをアタッチ")]
    public Text GoalText; 
    [Header("ここにカウントダウンのTextをアタッチ")]
    public Text CountDownText; 

    [Header("ここに選択肢のおおもとのゲームオブジェクトをアタッチ")]
    public GameObject ChoiceParent;
    [Header("ここに子オブジェクトにTextのある選択肢用のプレハブをアタッチ")]
    public GameObject ChoicePrefab;
    [Header("ここに子オブジェクトにTextのある選択肢用のプレハブがアタッチされる親オブジェクトをアタッチ")]
    public Transform ChoicePrefabParent;

    [Header("ここに子オブジェクトにImageのあるアイテム用のプレハブをアタッチ")]
    public GameObject ItemPrefab;
    [Header("ここに子オブジェクトにImageのあるアイテム用のプレハブがアタッチされる親オブジェクトをアタッチ")]
    public Transform ItemPrefabParent;

    [Header("ここにBackGroundManagerをアタッチ")]
    public BackGroundManager backGroundManager; 
    [Header("ここにmessageSetを先に進めるためのコライダーが取り付けられたゲームオブジェクトをアタッチ")]
    public GameObject textNextColliderGo;











    int oldMessageNow;
    
    float countDownTime;
    int oldCountDown;

    int countDownMessageSet;

    int oldGoodsText = -1;

    Coroutine countDownCoroutine;

    //messageSetの表示中はnullでなくなる
    Coroutine messageShowCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        ItemGot = new GameObject[100];
        textNextColliderGo.SetActive(false);
        ChoiceParent.SetActive(false);

        messageStart(0);

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("TestMessage")]
    void TestMessage(){
        messageStart(TestNumber);
    }

    public void messageStart(int messageSetType){
        if(messageCoroutine == null){
            messageCoroutine = StartCoroutine(messagingCoroutine(messageSetType));
        }
        else{
            Debug.Log("２つのMessageSetを同時に再生することはできません");
        }
    }

    IEnumerator messagingCoroutine(int messageSetType){
        messageNow = 0;
        oldMessageNow = 0;
        textNextColliderGo.SetActive(true);



        //最後までmessageNowが行ったら終了する
        while(messageNow != messageSet[messageSetType].messageDatas.Length){


            //メッセージがコマンドかを判定する
            string commandString = messageSet[messageSetType].messageDatas[messageNow].message;


            if (commandString.StartsWith("cm")) //コマンドだった場合
            {


                //messageSet読み込みコマンドだった場合
                if(commandString.Length >= 6 && commandString.Substring(2, 4) == "_set"){
                    int whatMessageSetMove = ConvertStringToInt(commandString.Substring(7, 3));
                    StartCoroutine(MessageSetReload(whatMessageSetMove));
                    messageNow = messageSet[messageSetType].messageDatas.Length - 1;

                    MessageSetEnd();
                }


                //シーン遷移コマンドだった場合
                if(commandString.Length >= 8 && commandString.Substring(2, 6) == "_stage"){
                    int whatSceneMove = ConvertStringToInt(commandString.Substring(9, 3));
                    int howSceneMove = ConvertStringToInt(commandString.Substring(13, 3));
                    backGroundManager.BackGroundChange(whatSceneMove, howSceneMove);
                }


                //シーン終了コマンドだった場合
                if(commandString.Length >= 6 && commandString.Substring(2, 4) == "_end"){

                    messageNow = messageSet[messageSetType].messageDatas.Length - 1;
                }


                //フラグを立てるコマンドだった場合
                if(commandString.Length >= 11 && commandString.Substring(2, 9) == "_flagTrue"){

                    int trueFlagType = ConvertStringToInt(commandString.Substring(12, 3));
                    flags[trueFlagType].isFlag = true;
                }


                //フラグを折るコマンドだった場合
                if(commandString.Length >= 12 && commandString.Substring(2, 10) == "_flagFalse"){

                    int falseFlagType = ConvertStringToInt(commandString.Substring(13, 3));
                    flags[falseFlagType].isFlag = false;
                }


                //フラグを判定するコマンドだった場合
                if(commandString.Length >= 9 && commandString.Substring(2, 7) == "_flagIF"){

                    int whatFlagType = ConvertStringToInt(commandString.Substring(10, 3));
                    int howSkipMessage = ConvertStringToInt(commandString.Substring(14, 3));

                    if(flags[whatFlagType].isFlag){
                        messageNow += howSkipMessage;
                    }
                }


                //２つの選択肢だった場合
                if(commandString.Length >= 10 && commandString.Substring(2, 8) == "_choice2"){
                    int choice0SceneSkip = ConvertStringToInt(commandString.Substring(11, 3));
                    int choice1SceneSkip = ConvertStringToInt(commandString.Substring(15, 3));

                    string[] choice2 = 
                    {"" + messageSet[messageSetType].messageDatas[messageNow + 1].message,
                    "" + messageSet[messageSetType].messageDatas[messageNow + 2].message};

                    ShowChoice(choice2);

                    while(isChoicing){
                        yield return null;
                    }

                    switch(beChose){
                        case 0:
                            messageNow += choice0SceneSkip;
                            break;
                        case 1:
                            messageNow += choice1SceneSkip;
                            break;
                        default:
                            messageNow += choice0SceneSkip;
                            break;
                    }
                    isChoicing = true;
                }

                //３つの選択肢だった場合
                if(commandString.Length >= 10 && commandString.Substring(2, 8) == "_choice3"){

                    int choice0SceneSkip = ConvertStringToInt(commandString.Substring(11, 3));
                    int choice1SceneSkip = ConvertStringToInt(commandString.Substring(15, 3));
                    int choice2SceneSkip = ConvertStringToInt(commandString.Substring(19, 3));


                    string[] choice3 = 
                    {"" + messageSet[messageSetType].messageDatas[messageNow + 1].message,
                    "" + messageSet[messageSetType].messageDatas[messageNow + 2].message,
                    "" + messageSet[messageSetType].messageDatas[messageNow + 3].message,
                    };

                    ShowChoice(choice3);

                    while(isChoicing){
                        yield return null;
                    }

                    switch(beChose){
                        case 0:
                            messageNow += choice0SceneSkip;
                            break;
                        case 1:
                            messageNow += choice1SceneSkip;
                            break;
                        case 2:
                            messageNow += choice2SceneSkip;
                            break;
                        default:
                            messageNow += choice0SceneSkip;
                            break;
                    }
                    isChoicing = true;
                }

                //アイテムを手に入れる場合
                if(commandString.Length >= 10 && commandString.Substring(2, 8) == "_itemGet"){

                    int getItemType = ConvertStringToInt(commandString.Substring(11, 3));
                    ItemGet(getItemType);
                }

                //アイテムを消し飛ばす場合
                if(commandString.Length >= 10 && commandString.Substring(2, 8) == "_itemUse"){
                    int getItemType = ConvertStringToInt(commandString.Substring(11, 3));
                    ItemRemove(getItemType);
                }

                //アイテムを基に条件分岐する場合
                if(commandString.Length >= 9 && commandString.Substring(2, 7) == "_itemIF"){
                    int itemTypeIF = ConvertStringToInt(commandString.Substring(10, 3));
                    int howSceneSkip = ConvertStringToInt(commandString.Substring(14, 3));

                    if(ItemHave(itemTypeIF)){
                        messageNow += howSceneSkip;
                    }
                }

                //数フレーム待機する場合
                if(commandString.Length >= 7 && commandString.Substring(2, 5) == "_wait"){
                    int howFrameWait = ConvertStringToInt(commandString.Substring(8, 3));
                    float waitTime = (float)howFrameWait / 60;

                    StartCoroutine(MessageStop(waitTime));
                    yield return new WaitForSeconds(waitTime);
                }

                //メッセージをクリアする場合
                if(commandString.Length >= 8 && commandString.Substring(2, 6) == "_clear"){
                    if (messageShowCoroutine != null)
                    {
                        StopCoroutine(messageShowCoroutine);
                    }
                    MessageText.text = "";
                    NoOneMessageText.text = "";
                    NameText.text = "";

                }

                //次のメッセージを強制的に飛ばす場合
                if(commandString.Length >= 7 && commandString.Substring(2, 5) == "_next"){
                    int howFrameSkip = ConvertStringToInt(commandString.Substring(8, 3));
                    float waitTime = (float)howFrameSkip / 60;

                    if (messageShowCoroutine != null)
                    {
                        StopCoroutine(messageShowCoroutine);
                    }
                    messageShowCoroutine = StartCoroutine(MessageShow(messageSetType, messageNow + 1));

                    StartCoroutine(MessageStop(waitTime));
                    yield return new WaitForSeconds(waitTime);
                    messageNow++;
                }

                //目標を表示する場合
                if(commandString.Length >= 9 && commandString.Substring(2, 7) == "_goalIn"){
                    int whatTypeGoals = ConvertStringToInt(commandString.Substring(10, 3));
                    GoalText.text = "" + goals[whatTypeGoals];
                    
                }

                //目標を消し飛ばす場合
                if(commandString.Length >= 10 && commandString.Substring(2, 8) == "_goalOut"){
                    GoalText.text = "" ;
                }

                //カウントダウンを始める場合
                if(commandString.Length >= 8 && commandString.Substring(2, 6) == "_count"){
                    int howCountTime = ConvertStringToInt(commandString.Substring(9, 3));
                    int whatMessageSet = ConvertStringToInt(commandString.Substring(13, 3));

                    CountDownStart(howCountTime,whatMessageSet);
                }

                //カウントダウンを停止する場合
                if(commandString.Length >= 12 && commandString.Substring(2, 10) == "_stopCount"){
                    CountStop();
                }

                //スペシャルなテキストを表示する場合
                if(commandString.Length >= 14 && commandString.Substring(2, 12) == "_SpecialShow"){
                    int whatSpecialType = ConvertStringToInt(commandString.Substring(15, 3));
                    SpecialMessageShow(whatSpecialType);
                }

                //スペシャルなテキストを非表示にする場合
                if(commandString.Length >= 15 && commandString.Substring(2, 13) == "_SpecialClear"){
                    SpecialMessageClear();
                }

                //テキストを表示する場所を変更する場合
                if(commandString.Length >= 16 && commandString.Substring(2, 14) == "_SpecialChange"){
                    int whatPlaceChange = ConvertStringToInt(commandString.Substring(17, 3));
                    SpecialMessageChange(whatPlaceChange);
                }

                //Windowsゲームオブジェクトを出現させる場合
                if(commandString.Length >= 9 && commandString.Substring(2, 7) == "_window"){
                    int whatWindowShow = ConvertStringToInt(commandString.Substring(10, 3));
                    WindowShow(whatWindowShow);
                }


                //エンディングのフラグを立てる
                if(commandString.Length >= 12 && commandString.Substring(2, 10) == "_gameClear"){
                    GameManager.instance.GameCleared();
                    SceneManager.LoadScene("Title");
                }



                //音楽リスト再生コマンドだった場合
                if(commandString.Length >= 8 && commandString.Substring(2, 6) == "_music"){
                    int whatMusicList = ConvertStringToInt(commandString.Substring(9, 3));
                    MusicPlayer.ist.StartMusicList(whatMusicList);
                }
                
                //音楽リスト停止コマンドだった場合
                if(commandString.Length >= 12 && commandString.Substring(2, 10) == "_stopMusic"){
                    MusicPlayer.ist.StopMusic(1.5f);
                }

                //効果音コマンド場合
                if(commandString.Length >= 5 && commandString.Substring(2, 3) == "_SE"){
                    int whatTypeSE = ConvertStringToInt(commandString.Substring(6, 3));
                    int sePitch = ConvertStringToInt(commandString.Substring(10, 3));

                    float lastPitch = (float)sePitch / 100f;
                    SEPlayer.instance.SE(whatTypeSE, lastPitch);
                }


                messageNow++;
            }
            else{ //メッセージだった場合
                if (messageShowCoroutine != null)
                {
                    StopCoroutine(messageShowCoroutine);
                }
                messageShowCoroutine = StartCoroutine(MessageShow(messageSetType, messageNow));
                oldMessageNow = messageNow;


            }

            //messageNowが変更されたら再始動する
            while(oldMessageNow == messageNow){
                yield return null;
            }
        }

        //終了の処理
        MessageSetEnd();
    }

    //messageSet終了の処理
    void MessageSetEnd(){
        if (messageShowCoroutine != null)
        {
            StopCoroutine(messageShowCoroutine);
        }
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        messageCoroutine = null;

        textNextColliderGo.SetActive(false);
        MessageText.text = "";
        NoOneMessageText.text = "";
        NameText.text = "";
    }

    //messageSetをmessageSet内から呼ぶための処理。遅延を挟んでもう一度呼び出す
    IEnumerator MessageSetReload(int messageSet){
        yield return null;
        messageStart(messageSet);
    }

    //メッセージを次にする
    public void MessageNext(){
        messageNow++;
    }

    //メッセージをちょっとずつ表示する
    IEnumerator MessageShow(int messageSetType, int messageType){

        //セリフ関係初期化
        MessageText.text = "";
        NoOneMessageText.text = "";
        SpecialMessageReset();

        //名前を表示
        switch(messagePlace){
            case 0: //デフォルトの場所に表示
                NameText.text = "" + humanNames[messageSet[messageSetType].messageDatas[messageType].name];
                break;
            default: //スペシャルな場所に表示
                specialText[messagePlace - 1].name.text = "" + humanNames[messageSet[messageSetType].messageDatas[messageType].name];
                break;
        }

        //セリフを少しづつ表示
        string messaging = messageSet[messageSetType].messageDatas[messageType].message;        

        //スペシャルなメッセージを初期のものに戻す。
        if(messagePlace == 0){
            SpecialMessageReload();
        }

        foreach (char c in messaging)
        {
            if(messageSet[messageSetType].messageDatas[messageType].name != 0){
                switch(messagePlace){
                    case 0: //デフォルトの場所に表示
                        MessageText.text += "" + c;
                        break;
                    default: //スペシャルな場所に表示
                        specialText[messagePlace - 1].message.text += "" + c;
                        break;
                }
            }
            else{
                NoOneMessageText.text += c;
            }
            
            SEPlayer.instance.SE(humanSE[messageSet[messageSetType].messageDatas[messageType].name],1);
            yield return new WaitForSeconds(messageSpeed);
        }
        messageShowCoroutine = null;
    }

    //005とかの文字列を数字の5に変えたりする
    int ConvertStringToInt(string input)
    {
        return int.Parse(input);
    } 

    //選択肢を表示させる
    void ShowChoice(string[] choice){
        ChoiceParent.SetActive(true);

        //前に使った選択肢を消し飛ばす
        foreach(Transform transform in ChoicePrefabParent.GetComponentsInChildren<Transform>()){
            if(transform != ChoicePrefabParent.transform){
                Destroy(transform.gameObject);
            }
        }

        //選択肢を召喚する
        for(int i = 0; i < choice.Length; i++){
            GameObject instantiatedGO = Instantiate(ChoicePrefab);
            instantiatedGO.transform.SetParent(ChoicePrefabParent);
            instantiatedGO.transform.localScale = new Vector3(1,1,1);

            instantiatedGO.transform.position = Vector3.zero;
            instantiatedGO.transform.localPosition = Vector3.zero;

            Text text = instantiatedGO.GetComponentInChildren<Text>();
            text.text = choice[i];

            ChoiceScript choiceScript = instantiatedGO.GetComponentInChildren<ChoiceScript>();
            choiceScript.ChoiceNumber = i;

        }
    }

    //選択肢を選ぶ
    public void choiceSet(int choiceType){
        ChoiceParent.SetActive(false);
        //SEPlayer.instance.SE(2,1);

        beChose = choiceType;
        isChoicing = false;
    }

    //アイテムを入手する
    public void ItemGet(int itemType){

        for(int i = 0; i < ItemGot.Length; i++){
            if(ItemGot[i] == null){
                ItemGot[i] = Instantiate(ItemPrefab);
                ItemGot[i].transform.SetParent(ItemPrefabParent);
                ItemGot[i].transform.localScale = new Vector3(1,1,1);

                ItemGot[i].transform.position = Vector3.zero;
                ItemGot[i].transform.localPosition = Vector3.zero;

                Image image = ItemGot[i].GetComponentInChildren<Image>();
                image.sprite = itemData[itemType].image;

                ItemScript itemScript = ItemGot[i].GetComponentInChildren<ItemScript>();
                itemScript.ItemNumber = itemType;

                //SEPlayer.instance.SE(1,1);

                break;
            }
        }
    }

    //アイテムを消し飛ばす
    public void ItemRemove(int itemType){
        for(int i = 0; i < ItemGot.Length; i++){
            if(ItemGot[i] != null){
                ItemScript itemScript = ItemGot[i].GetComponentInChildren<ItemScript>();
                if(itemScript.ItemNumber == itemType){
                    Destroy(ItemGot[i]);
                    break;
                }
            }
        }

        int index = 0;
        GameObject[] newArray = new GameObject[ItemGot.Length];

        for (int i = 0; i < ItemGot.Length; i++)
        {
            if (ItemGot[i] != null)
            {
                newArray[index] = ItemGot[i];
                index++;
            }
        }

        ItemGot = newArray;


    }

    //アイテムを持っているか判定する
    public bool ItemHave(int itemType){
        for(int i = 0; i < ItemGot.Length; i++){
            if(ItemGot[i] != null){
                ItemScript itemScript = ItemGot[i].GetComponentInChildren<ItemScript>();
                if(itemScript.ItemNumber == itemType){
                    return true;
                }
            }
        }
        return false;
    }

    //アイテムの説明を表示する
    public void ItemInfoShow(int itemType){
        if(messageShowCoroutine != null)
            return;

        NoOneMessageText.text = "" + itemData[itemType].name + "：" + itemData[itemType].info;
    }

    //ここに渡すとnフレーム間テキストが進められなくなる
    IEnumerator MessageStop(float waitTime){
        textNextColliderGo.SetActive(false);
        yield return new WaitForSeconds(waitTime);
        textNextColliderGo.SetActive(true);

    }

    //time秒間のカウントダウンが始まり、messageSetTypeがカウントダウン後に実行される
    public void CountDownStart(float time, int messageType){
        //別のカウントダウンが実行中であれば停止させる
        CountStop();

        //カウントダウン開始
        countDownCoroutine = StartCoroutine(CountDowing(time, messageType)); 
    }

    //カウントダウンのコルーチン
    IEnumerator CountDowing(float time, int messageType){
        //カウントダウンの初期値設定
        countDownTime = time;
        countDownMessageSet = messageType;

        //カウントダウン中
        while(countDownTime > 0){
            countDownTime -= Time.deltaTime;
            yield return null;

            int nowCount = (int)countDownTime;
            if(nowCount != oldCountDown){
                CountDownText.text = "" + nowCount;
                oldCountDown = nowCount;
            }
        }

        //何かしらのmessageSetが再生中であれば終わるまで待機する
        while(messageCoroutine != null){
            yield return null;
        }

        //カウントダウン終了
        messageStart(messageType);
        CountStop();
    }

    //カウントダウンを停止させる
    public void CountStop(){
        CountDownText.text = "";
        if(countDownCoroutine != null){
            StopCoroutine(countDownCoroutine);
            countDownCoroutine = null;
        }
    }

    //specialMessageType番目のスペシャルなメッセージを表示する
    public void SpecialMessageShow(int specialMessageType){
        SpecialMessageClear();
        specialText[specialMessageType].textParent.SetActive(true);
    }

    //どこにメッセージを表示するかを変更する
    public void SpecialMessageChange(int specialMessageType){
        messagePlace = specialMessageType;
    }

    //スペシャルなメッセージを非表示にする。
    public void SpecialMessageClear(){
        for(int i = 0; i < specialText.Length; i++){
            specialText[i].textParent.SetActive(false); 
        }
    }

    //スペシャルなメッセージをすべてデフォルトなメッセージにする
    public void SpecialMessageReload(){
        for(int i = 0; i < specialText.Length; i++){
            specialText[i].message.text = "" + specialText[i].defaultInput;
        }
    }

    //スペシャルなメッセージをすべて無にする
    public void SpecialMessageReset(){
        for(int i = 0; i < specialText.Length; i++){
            specialText[i].message.text = "";
        }
    }

    //タイトルに戻る
    public void ReturnTitle(){
        SceneManager.LoadScene("Title");
    }

    //TextにGoodsTextを表示する
    public void GoodsTextView(int type){
        if(messageCoroutine != null || oldGoodsText == type)
            return;
        
        oldGoodsText = type;

        SEPlayer.instance.SE(5,1);

        if(goodsText[type].isFlag){
            NoOneMessageText.text = "" + goodsText[type].goodsName;
        }
        else{
            NoOneMessageText.text = "" + goodsText[type].nullName;
        }
    }

    //GoodsTextのフラグを立てる
    public void GoodsTextFlagTrue(int type){
        goodsText[type].isFlag = true;
    }

    //特別なウィンドウを出現させる
    public void WindowShow(int type){
        Windows[type].SetActive(true);
    }

}
