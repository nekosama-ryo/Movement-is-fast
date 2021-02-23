using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class CreateBullet
{
    //稼働中の弾
    private List<GameObject> _Bullets = new List<GameObject>(Data._maxBullet * 2);

    //弾速
    private float _bulletSpeed = 10f;
    //カメラの範囲指定
    private Rect rect = new Rect(0, 0, 1, 1);
    //カメラ情報
    private Camera _cam = default;
    //弾のオブジェクト
    private GameObject _original = default;
    //使用するタスクのキャンセル
    private CancellationTokenSource _cancell = new CancellationTokenSource(1);
    //一度のみ
    private bool one = true;

    //生成する角度
    private Vector3 _createAngle = Vector3.zero;
    //隣の弾との角度差
    private float _angle = default;
    //時間
    private float _time = 0f;

    //コンストラクタ
    public CreateBullet()
    {
        //弾数が０なら生成を行わない
        if (Data.Bullet != 0)
        {
            _angle = 360.0f / Data.Bullet;
        }
        //弾のprefabを取得
        _original = Resources.Load("Bullet") as GameObject;
        _cam = GameObject.Find("GameCamera").GetComponent<Camera>();
    }

    public void BulletAction(Vector3 pos, int time)
    {
        if (Data.IsAsync)
        {
            if (one)
            {
                AsyncBulletAction(pos, time, _cancell.Token).Forget();
                AsyncCreateBullet(_cancell.Token).Forget();
            }

        }
        else
        {
            BulletShoot();
            ObjectCreateBullet(pos, time / 100);
        }
    }

    /// <summary>指定時間ごとに弾を撃つ </summary>
    private void BulletShoot()
    {
        //一時的に使用する変数
        int i = 0;
        Vector3 outpos;

        //Listの要素を探索
        while (i < _Bullets.Count)
        {
            //弾が存在するかどうか
            if (_Bullets[i].activeSelf)
            {
                //画面外の位置を設定
                outpos = _cam.WorldToViewportPoint(_Bullets[i].transform.position);

                //カメラの範囲内かどうか
                if (rect.Contains(outpos))
                {
                    //弾を動かす。
                    _Bullets[i].transform.position = BulletMove(_Bullets[i].transform);
                    i++;
                }
                else
                {
                    //オブジェクトを消す処理
                    ObjectDestroy(i);
                }
            }
            else
            {
                i++;
            }
        }
    }
    //オブジェクト指向の弾発射
    private void ObjectCreateBullet(Vector3 pos, float time)
    {
        //指定時間ごとに生成を行う
        if (_time > time)
        {
            //弾の発射
            All(pos);

            //カウントのリセット
            _time = 0;
        }
        else
        {
            _time += Time.deltaTime;
        }
    }

    //全方位に弾を生成する
    private void All(Vector3 pos)
    {
        for (int i = 0; i < Data.Bullet; i++)
        {
            //弾を生成。
            ObjectCreate(pos, _createAngle, true);
            //次回の生成角度を変える。
            _createAngle.y += _angle;
        }
        //初期の生成位置をずらす
        _createAngle.y += _angle * 0.5f;
    }

    /// <summary>弾を生成する </summary>
    private void ObjectCreate(Vector3 pos, Vector3 rot, bool active)
    {
        //プールの使用設定によってプールを使用する。
        if (Data.IsPool)
        {
            //Listの中から未使用のオブジェクトを見つける
            foreach (GameObject Obj in _Bullets)
            {
                if (!Obj.activeSelf)
                {
                    Obj.transform.position = pos;
                    Obj.transform.eulerAngles = rot;
                    Obj.SetActive(true);
                    return;
                }
            }
        }
        //新規生成
        GameObject obj = Object.Instantiate(_original);

        //生成したオブジェクトのパラメータを設定。
        obj.transform.position = pos;
        obj.transform.eulerAngles = rot;
        obj.SetActive(active);

        //稼働中のオブジェクトとして保存
        _Bullets.Add(obj);
    }

    private void ObjectDestroy(int i)
    {
        //プールを使用してオブジェクトを消すかどうか。
        if (Data.IsPool)
        {
            //値の初期化と、プール
            _Bullets[i].SetActive(false);
            _Bullets[i].transform.eulerAngles = Vector3.zero;
        }
        else
        {
            //プールを使用しないで、オブジェクトを消す。
            Object.Destroy(_Bullets[i]);
            _Bullets.Remove(_Bullets[i]);
        }
    }

    public Vector3 BulletMove(Transform transform)
    {
        return transform.position + transform.forward * Time.deltaTime * _bulletSpeed;
    }

    //非同期で処理を行う。
    private async UniTask AsyncBulletAction(Vector3 pos, int time, CancellationToken ct = default)
    {
        while (true)
        {
            //弾を発射する。
            All(pos);
            //待機処理
            await UniTask.DelayFrame(time, cancellationToken: ct);
            Debug.Log(1);
        }

    }
    private async UniTask AsyncCreateBullet(CancellationToken ct = default)
    {
        while (true)
        {
            BulletShoot();
            await UniTask.DelayFrame(1, cancellationToken: ct);
        }
    }
}
