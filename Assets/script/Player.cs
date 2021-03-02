using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using System;

public class ObjectPlayer
{
    //現在のプレイヤーの速度
    private Vector3 _playerSpeed = Vector3.zero;
    //プレイヤーの位置
    public Vector3 _playerPos = Vector3.zero;
    public Vector3 _playerRot = Vector3.zero;
    //プレイヤーの機体の傾き具合
    private int _incline = 30;
    //最高速度
    private float _playerSpeedMax = 0.3f;
    //時間管理
    private float _height = 0;
    private float _width = 0;
    //プレイヤーの低速移動
    private float _lowSpeed = 2.5f;

    public void OnStart()
    {
        _playerPos = SerializeGameData.GameData.playerTransform.position;
        _playerRot = SerializeGameData.GameData.playerTransform.eulerAngles;
    }

    public void OnUpdate()
    {
        //プレイヤーの位置を変更
        SerializeGameData.GameData.playerTransform.position = _playerPos;
        SerializeGameData.GameData.playerTransform.eulerAngles = _playerRot;
        //移動処理
        PlayerMove();
    }

    public async UniTask OnAsyncStart(CancellationToken ct)
    {
        //ゲームマネージャーが止まるまで非同期で動かす。
        while (true)
        {
            //通常の処理を非同期で行う。
            OnUpdate();
            //1フレーム待機
            await UniTask.Delay(1);
            if (Data.IsGameManager) continue;
            ct.ThrowIfCancellationRequested();
        }
    }

    /// <summary>プレイヤーがキーに対応した動きを行う</summary>
    public void PlayerMove()
    {
        //それぞれのキーに応じて、移動する。
        _playerSpeed.z = GetKeyMove(Data.UP, Data.Down, _playerSpeed.z, 0, _playerSpeedMax, ref _height);
        _playerSpeed.x = GetKeyMove(Data.Right, Data.Left, _playerSpeed.x, 0, _playerSpeedMax, ref _width);

        if (Input.GetKey(Data.BButton)) _playerSpeed /= _lowSpeed;

        _playerRot.x = _playerSpeed.z * _incline;
        _playerRot.z = _playerSpeed.x * -_incline;

        //位置にスピードを足す。
        _playerPos += _playerSpeed;
    }
    /// <summary>設定されたキーに対してスピードを返す</summary>
    public float GetKeyMove(KeyCode key, KeyCode mirrorKey, float speed, float min, float max, ref float time)
    {
        //プラス方向の入力処理。キーを離しても、移動処理が続くように。
        if (Input.GetKey(key)) return Mathf.Lerp(speed, max, LerpCalculate(ref time));
        else if (!Input.GetKey(mirrorKey)) return Mathf.Lerp(speed, min, LerpCalculate(ref time));

        //マイナス方向の入力処理。キーを離しても、移動処理が続くように。
        if (Input.GetKey(mirrorKey)) return -Mathf.Lerp(speed, max, LerpCalculate(ref time));
        else if (!Input.GetKey(key)) return -Mathf.Lerp(speed * -1, min, LerpCalculate(ref time));

        return speed;
    }

    /// <summary>慣性のつけたような数値を返す</summary>
    private float LerpCalculate(ref float time)
    {
        time += Time.deltaTime;
        float lerptime = time * time * 0.1f;
        return Mathf.Min(lerptime / 1, 1);
    }
}
