using UnityEngine;

/// <summary>オブジェクト指向のSerializeで取得したいデータ</summary>
public class SerializeGameData : MonoBehaviour
{
    //外部からデータを取得出来る
    public static SerializeGameData GameData { get; private set; } = default;
    public void SetSerializeData(SerializeGameData data)
    {
        GameData = data;
    }

    [Header("プレイヤーのオブジェクト")] public Transform playerTransform = default;
    [Header("プレイヤーの当たり判定")] public Collision playerCol=default;

    [Header("弾のプレハブ")] public GameObject bullet = default;
    [Header("俯瞰カメラ")]public Camera gameCam = default;
}
