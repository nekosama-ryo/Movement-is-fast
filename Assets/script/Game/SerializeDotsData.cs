using UnityEngine;

/// <summary>データ指向のSerializeで取得したいデータ</summary>
public class SerializeDotsData : MonoBehaviour
{
    //外部からデータを取得出来る
    public static SerializeDotsData DotsData { get; private set; } = default;
    public void SetSerializeData(SerializeDotsData data)
    {
        DotsData = data;
    }

    [Header("弾のプレハブ")] public GameObject bulletObj = default;
    [Header("俯瞰カメラ")] public Camera gameCam = default;
}
