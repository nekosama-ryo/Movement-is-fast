using UnityEngine;
using UnityEngine.UI;

/// <summary>オブジェクト指向のSerializeで取得したいデータ</summary>
public class SerializeTitleData : MonoBehaviour
{
    //外部からデータを取得出来る
    public static SerializeTitleData TitleData { get; private set; } = default;
    public void SetSerializeData(SerializeTitleData data)
    {
        TitleData = data;
    }

    //ゲーム開始画面
    [Header("タイトル画面のオブジェクトTransformを入れる")] public Transform[] titleObjTransforms  = default;
    [Header("AnyButtonのテキスト")] public Text anyButtonText = default;
    //セレクト画面
    [Header("選択画面のオブジェクトTransformを入れる")] public Transform[] selectObjTransforms = default;

    [Header("オブジェクト指向テキスト")] public Text[] oriented = default;
    [Header("プールテキスト")] public Text selectPoolText = default;
    [Header("非同期テキスト")] public Text selectAsyncText = default;
    [Header("弾数テキスト")] public Text selectBulletCountText = default;
    [Header("無敵モード")] public Text[] Invincibility = default;
    [Header("ゲームスタートテキスト")] public Text selectGameStartText = default;
}
