using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class CreateBullet
{
    //稼働中の弾
    private List<(GameObject obj, Transform transform)> _bullets = default;

    //弾速
    private float _bulletSpeed = 10f;
    //カメラの範囲指定
    private Rect rect = new Rect(0, 0, 1, 1);

    //生成する角度
    private Vector3 _createAngle = Vector3.zero;
    //隣の弾との角度差
    private float _angle = default;
    //時間
    private float _time = 0f;

    public void OnStart()
    {
        _bullets= new List<(GameObject, Transform)>(Data._maxBullet * 2);
        //弾数が０なら生成を行わない
        if (Data.Bullet != 0)
        {
            _angle = 360.0f / Data.Bullet;
        }
    }
    public async UniTask OnAsyncStart(CancellationToken ct,Transform trs,float time)
    {
        //ゲームマネージャーが止まるまで非同期で動かす。
        while (true)
        {
            //通常の処理を非同期で行う。
            OnUpdate(trs.position,time);
            //1フレーム待機
            await UniTask.Delay(1);
            if (Data.IsGameManager) continue;
            ct.ThrowIfCancellationRequested();
        }
    }

    public void OnUpdate(Vector3 pos,float time)
    {
            BulletMove();
            ObjectBulletShoot(pos, time);
    }



    /// <summary>指定時間ごとに弾を撃つ </summary>
    private void BulletMove()
    {
        //一時的に使用する変数
        Vector3 outpos;

        //Listの要素を探索
        for (int i=0; i < _bullets.Count;i++)
        {
            //弾が存在するかどうか
            if (!_bullets[i].obj.activeSelf) continue;

            //画面外の位置を設定
            outpos = SerializeGameData.GameData.gameCam.WorldToViewportPoint(_bullets[i].transform.position);

            //弾を動かす。
            _bullets[i].transform.position = BulletMove(_bullets[i].transform);

            //カメラの範囲外ならオブジェクトを消す
            if (rect.Contains(outpos)) continue;
            //オブジェクトを消す処理
            ObjectDestroy(i);
        }
    }

    /// <summary>オブジェクト指向の弾発射 </summary>
    private void ObjectBulletShoot(Vector3 pos, float time)
    {
        _time += Time.deltaTime;

        //指定時間ごとに生成を行う
        if (_time < time) return;

        //弾の発射(別の発射方法が出来れば、ここにSwitch(Enum)を置く)
        ShootAll(pos);
        //カウントのリセット
        _time = 0;
    }

    /// <summary>全方位に弾を生成する </summary>
    private void ShootAll(Vector3 pos)
    {
        for (int i = 0; i < Data.Bullet; i++)
        {
            //弾を生成。
            ObjectCreate(pos, _createAngle);
            //次回の生成角度を変える。
            _createAngle.y += _angle;
        }
        //初期の生成位置をずらす
        _createAngle.y += _angle * 0.5f;
    }

    /// <summary>弾を生成する </summary>
    private void ObjectCreate(Vector3 pos, Vector3 rot)
    {
        //プールの使用設定によってプールを使用する。
        if (Data.IsPool)
        {
            //Listの中から未使用のオブジェクトを見つける
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].obj.activeSelf) continue;

                _bullets[i].transform.position = pos;
                _bullets[i].transform.eulerAngles = rot;
                _bullets[i].obj.SetActive(true);
                return;
            }
        }

        //新規生成
        GameObject obj = Object.Instantiate(SerializeGameData.GameData.bullet);
        ( GameObject obj, Transform trs) Obj = (obj,obj.transform);

        //生成したオブジェクトのパラメータを設定。
        Obj.trs.position = pos;
        Obj.trs.eulerAngles = rot;

        //稼働中のオブジェクトとして保存
        _bullets.Add(Obj);
    }

    /// <summary>弾を消す・プールを行う </summary>
    private void ObjectDestroy(int i)
    {
        //プールを使用してオブジェクトを消すかどうか。
        if (Data.IsPool)
        {
            //値の初期化と、プール
            _bullets[i].obj.SetActive(false);
            _bullets[i].transform.eulerAngles = Vector3.zero;
        }
        else
        {
            //プールを使用しないで、オブジェクトを消す。
            Object.Destroy(_bullets[i].obj);
            _bullets.Remove(_bullets[i]);
        }
    }

    /// <summary>弾を直線に飛ばす </summary>
    public Vector3 BulletMove(Transform transform)
    {
        return transform.position + transform.forward * Time.deltaTime * _bulletSpeed;
    }
}
