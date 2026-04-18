public enum FishingState
{
    Waiting,        // 待機中（何もしていない）
    Casting,        // 竿を投げた状態
    FishOnHook,     // 魚がかかった状態（！アイコン表示）
    Catching,       // 魚を釣り上げ中
    Success,        // 釣り成功
    Failed,         // 釣り失敗
    GameOver,       // ゲームオーバー
    ViewingBook     // 魚図鑑閲覧モード
}