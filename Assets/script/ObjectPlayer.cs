using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayer
{
    //プレイヤー
    private GameObject _playerObj = default;
    //現在のプレイヤーの速度
    private Vector3 _playerSpeed = Vector3.zero;
    //プレイヤーの位置
    private Vector3 _playerPos = Vector3.zero;
    private Vector3 _playerRot = Vector3.zero;
    //プレイヤーの機体の傾き具合
    private int _incline = 30;
    //最高速度
    private float _playerSpeedMax = 0.3f;
    //時間管理
    private float _height = 0;
    private float _width = 0;

    public ObjectPlayer()
    {
        //_playerObj = GameObject.Find("Player");
        _playerPos = _playerObj.transform.position;
        _playerRot = _playerObj.transform.eulerAngles;
    }

    public void PlayerAction()
    {
        //プレイヤーの位置を変更
        _playerObj.transform.position = _playerPos;
        _playerObj.transform.eulerAngles =_playerRot;
        PlayerMove();
    }
    /// <summary>プレイヤーがキーに対応した動きを行う</summary>
    private void PlayerMove()
    {
        //それぞれのキーに応じて、移動する。
        _playerSpeed.z = GetKeyMove(Data.UP, Data.Down, _playerSpeed.z, 0, _playerSpeedMax,ref _height);
        _playerSpeed.x = GetKeyMove(Data.Right, Data.Left, _playerSpeed.x, 0, _playerSpeedMax,ref _width);

        _playerRot.x = _playerSpeed.z * _incline;
        _playerRot.z = _playerSpeed.x * -_incline;

        //位置にスピードを足す。
        _playerPos += _playerSpeed;

    }

    /// <summary>設定されたキーに対してスピードを返す</summary>
    private float GetKeyMove(KeyCode key, KeyCode mirrorKey, float speed, float min, float max,ref float time)
    {
        //プラス方向の入力処理
        if (Input.GetKey(key))
        {
            return Mathf.Lerp(speed, max,LerpCalculate(ref time));
        }
        else
        {
            if (!Input.GetKey(mirrorKey))
            {
                return Mathf.Lerp(speed, min, LerpCalculate(ref time));
            }
        }

        //マイナス方向の入力処理
        if (Input.GetKey(mirrorKey))
        {
            return -Mathf.Lerp(speed, max, LerpCalculate(ref time));
        }
        else
        {
            if (!Input.GetKey(key))
            {
                return -Mathf.Lerp(speed * -1, min, LerpCalculate(ref time));
            }
        }

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

public struct DataPlayer
{

}
