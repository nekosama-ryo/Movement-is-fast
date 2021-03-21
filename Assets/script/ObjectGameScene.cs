﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System;

public class ObjectGameScene
{

    //スクリプト
    private ObjectPlayer _playerScr = new ObjectPlayer();
    private ObjectBullet _bulletScr = new ObjectBullet();

    //使用するタスクのキャンセル
    private CancellationTokenSource _cancell = new CancellationTokenSource(1);
    public void OnStart()
    {
        //初期処理
        _playerScr.OnStart();
        _bulletScr.OnStart();

        //同期処理の場合は以降の処理を行わない。
        if (!Data.IsAsync) return;

        //プレイヤーを非同期で動かす処理
        _playerScr.OnAsyncStart(_cancell.Token).Forget();
        //弾を非同期で動かす処理
        _bulletScr.OnAsyncStart(_cancell.Token, SerializeGameData.GameData.playerTransform, Data.BulletCreateFrame).Forget();
    }

    public void Cancellation()
    {
        _cancell.Cancel();
    }

    public void OnUpdate()
    {
            //非同期処理の場合は以降の処理を行わない。
            if (Data.IsAsync) return;
            //プレイヤーを動かす処理
            _playerScr.OnUpdate();
            //弾を動かす処理
            _bulletScr.OnUpdate(SerializeGameData.GameData.playerTransform.position, Data.BulletCreateFrame);
    }
}