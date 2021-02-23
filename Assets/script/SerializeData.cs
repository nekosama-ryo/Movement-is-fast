﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerializeData : MonoBehaviour
{
    public static SerializeData Data { get; private set; } = default;
    public void SetSerializeData(SerializeData data)
    {
        Data = data;
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
