using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>弾の処理 </summary>
public class Bullet 
{
    //弾のプール
    public List<(GameObject obj, Transform transform)> _bulletsPool = default;
    //弾のエンティティプレハブ
    private Entity _bulletEntityPrefab=default;
    //カメラの範囲指定
    private Rect _rect = new Rect(0, 0, 1, 1);
    //発射方法
    private Shot _shotScr = new Shot();

    public void OnStart()
    {
        //弾の生成方法を設定
        _shotScr.OnStart(CreateOrientedBullet);
        //プールの初期化
        _bulletsPool = new List<(GameObject, Transform)>();
    }

    /// <summary>指向情報を元に弾を生成する。 </summary>
    public void CreateOrientedBullet(Vector3 pos, Vector3 rot)
    {
        if (Data.IsOrientedObject)
        {
            //オブジェクト指向の弾生成
            CreateObjectBullet(pos, rot);
        }
        else
        {
            //データ指向の弾生成
            CreateDotsBullet(pos, rot);
        }
    }

    /// <summary>弾を生成する </summary>
    private void CreateObjectBullet(Vector3 pos, Vector3 rot)
    {
        //プールの使用設定によってプールを使用する。
        if (Data.IsPool)
        {
            //Listの中から未使用のオブジェクトを見つける
            for (int i = 0; i < _bulletsPool.Count; i++)
            {
                if (_bulletsPool[i].obj.activeSelf) continue;

                _bulletsPool[i].transform.position = pos;
                _bulletsPool[i].transform.eulerAngles = rot;
                _bulletsPool[i].obj.SetActive(true);
                return;
            }
        }

        //新規生成
        GameObject obj = UnityEngine.Object.Instantiate(SerializeGameData.GameData.bullet,pos,quaternion.Euler(rot));
        (GameObject obj, Transform trs) Obj = (obj, obj.transform);

        //生成したオブジェクトのパラメータを設定。
        Obj.trs.position = pos;
        Obj.trs.eulerAngles = rot;

        //稼働中のオブジェクトとして保存
        _bulletsPool.Add(Obj);
    }

    /// <summary>弾を消す・プールを行う </summary>
    private void DestroyObjectBullet(int i)
    {
        //プールを使用してオブジェクトを消すかどうか。
        if (Data.IsPool)
        {
            //値の初期化と、プール
            _bulletsPool[i].obj.SetActive(false);
            _bulletsPool[i].transform.eulerAngles = Vector3.zero;
        }
        else
        {
            //プールを使用しないで、オブジェクトを消す。
            UnityEngine.Object.Destroy(_bulletsPool[i].obj);
            _bulletsPool.Remove(_bulletsPool[i]);
        }
    }

    /// <summary>弾を直線に動かし、画面外の場合消す </summary>
    public void ObjectBulletMove()
    {
        for(int i=0;i<_bulletsPool.Count;i++)
        {
            //弾が存在するかどうか
            if (!_bulletsPool[i].obj.activeSelf) continue;

            //画面外に弾があるか調べる
            if (OutScreen(_bulletsPool[i].transform.position,SerializeGameData.GameData.gameCam)) DestroyObjectBullet(i);

            //弾を動かす。
            _bulletsPool[i].transform.position = BulletMove(_bulletsPool[i].transform);
        }
    }

    /// <summary>弾が画面外にあるか調べる </summary>
    public bool OutScreen(Vector3 pos,Camera cam)
    {
        //弾の位置を調べる
        Vector3 screenPos = cam.WorldToViewportPoint(pos);
        //画面外かどうか返す
        return !_rect.Contains(screenPos);
    }

    /// <summary>オブジェクト指向の弾発射 </summary>
    public void ObjectBulletShoot(Vector3 pos, float time)
    {
        //ボタンが押されたら、弾を発射する
        if(Input.GetKeyDown(Data.AButton))
        {
            //弾の生成
            _shotScr.Omniazimuth(pos);
        }
    }

    /// <summary>弾を直線に飛ばす </summary>
    private Vector3 BulletMove(Transform transform)
    {
        return transform.position + transform.forward * Data.BulletSpeed;
    }

    //ここからデータ指向の弾操作

    /// <summary>データ指向の弾生成（プレハブが生成済みであること） </summary>
    public void CreateDotsBullet(Vector3 pos, Vector3 rot)
    {
        //エンティティを新規生成する。
        Entity bullet = Data.EntityManager.Instantiate(_bulletEntityPrefab);

        //生成位置と生成角度の設定
        Data.EntityManager.SetComponentData(bullet, new Translation { Value = pos });
        Data.EntityManager.SetComponentData(bullet, new Rotation { Value = quaternion.Euler(rot) });
    }

    /// <summary>弾のエンティティプレハブを生成する。</summary>
    public void CreateBulletEntityPrefab()
    {
        World defaultWorld = World.DefaultGameObjectInjectionWorld;
        Data.EntityManager = defaultWorld.EntityManager;

        // GameObjectプレハブからEntityプレハブへの変換
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        _bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(SerializeDotsData.DotsData.bulletObj, settings);
        Data.EntityManager.AddComponent<BulletTag>(_bulletEntityPrefab);
    }
}
