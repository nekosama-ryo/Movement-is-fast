using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerializeGameData : MonoBehaviour
{
    public static SerializeGameData GameData { get; private set; } = default;
    public void SetSerializeData(SerializeGameData data)
    {
        GameData = data;
    }

    [Header("プレイヤーのオブジェクト")] public Transform playerTransform = default;
    [Header("プレイヤーの当たり判定")] public Collision playerCol=default;

    [Header("ボスのオブジェクト")] public Transform bossTransform = default;

    [Header("弾のプレハブ")] public GameObject bullet = default;
    [Header("俯瞰カメラ")]public Camera gameCam = default;
}
