using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>オブジェクト指向のプレイヤーの挙動</summary>
public class ObjectPlayer
{
    //現在のプレイヤーの速度
    private Vector3 _playerSpeed = Vector3.zero;
    //プレイヤーの位置
    private Vector3 _playerPos = Vector3.zero;
    private Vector3 _playerRot = Vector3.zero;

    //時間管理
    private float _height = 0;
    private float _width = 0;

    //カメラの範囲指定
    private Rect rect = new Rect(0, 0, 1, 1);

    public void OnStart()
    {
        //プレイヤーの情報を取得
        _playerPos = SerializeGameData.GameData.playerTransform.position;
        _playerRot = SerializeGameData.GameData.playerTransform.eulerAngles;
    }

    public void OnUpdate()
    {
        //プレイヤーの情報を変更
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

            //ゲームマネージャーが停止したらキャンセル処理を行う
            if (Data.IsGameManager) continue;
            ct.ThrowIfCancellationRequested();
        }
    }

    /// <summary>プレイヤーがキーに対応した動きを行う</summary>
    private void PlayerMove()
    {
        //入力キーに応じてプレイヤーのスピードを求める
        _playerSpeed = PlayerSpeed(_playerSpeed, Data.PlayerMaxSpeed, Data.PlayerLowSpeed, ref _height, ref _width);

        //位置にスピードを足す。
        _playerPos += _playerSpeed;
        //移動方向に応じて機体を傾ける
        _playerRot = SetPlayerRot(_playerSpeed, Data.PlayerIncline);
    }

    /// <summary>移動量に応じて傾きを返す</summary>
    public Vector3 SetPlayerRot(Vector3 speed,float incline)
    {
        //戻り値用
        Vector3 rot = Vector3.zero;

        //傾きを求める
        rot.x = Mathf.Clamp( speed.z * incline,-incline,incline);
        rot.z = Mathf.Clamp(speed.x * -incline, -incline, incline);

        return rot;
    }

    /// <summary>スピードを返す</summary>
    public Vector3 PlayerSpeed(Vector3 speed, float maxSpeed, float lowSpeed,ref float height, ref float width)
    {
        //それぞれのキーに応じて、移動する。
        speed.z = GetKeyMove(Data.UP, Data.Down, speed.z, 0, maxSpeed, ref height);
        speed.x = GetKeyMove(Data.Right, Data.Left, speed.x, 0, maxSpeed, ref width);

        //特定のキーが押されたらゆっくりと移動する。
        if (Input.GetKey(Data.BButton)) speed /= lowSpeed;

        return speed;
    }

    /// <summary>設定されたキーに対してスピードを返す</summary>
    private float GetKeyMove(KeyCode key, KeyCode mirrorKey, float speed, float min, float max, ref float time)
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
