using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeDotsData : MonoBehaviour
{
    public static SerializeDotsData DotsData { get; private set; } = default;
    public void SetSerializeData(SerializeDotsData data)
    {
        DotsData = data;
    }

    [Header("弾のプレハブ")] public GameObject bulletObj = default;
    [Header("俯瞰カメラ")] public Camera gameCam = default;
}
