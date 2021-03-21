using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using System;

public class Bullet 
{
    //弾のプール
    public List<(GameObject obj, Transform transform)> _bulletsList = new List<(GameObject, Transform)>();
    //弾のエンティティプレハブ
    private Entity _bulletPrefab;
    //カメラの範囲指定
    private Rect rect = new Rect(0, 0, 1, 1);

    //生成する角度
    private Vector3 _createAngle = Vector3.zero;
    //隣の弾との角度差
    private float _angle = default;
    //時間
    private float _time = 0f;

    private Shot _shotScr = new Shot();

    public void OnStart()
    {
        _shotScr.OnStart(CreateOrientedBullet);
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
            CreateEntityBullet(pos, rot);
        }
    }

    /// <summary>弾を生成する </summary>
    private void CreateObjectBullet(Vector3 pos, Vector3 rot)
    {
        //プールの使用設定によってプールを使用する。
        if (Data.IsPool)
        {
            //Listの中から未使用のオブジェクトを見つける
            for (int i = 0; i < _bulletsList.Count; i++)
            {
                if (_bulletsList[i].obj.activeSelf) continue;

                _bulletsList[i].transform.position = pos;
                _bulletsList[i].transform.eulerAngles = rot;
                _bulletsList[i].obj.SetActive(true);
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
        _bulletsList.Add(Obj);
    }

    /// <summary>弾を消す・プールを行う </summary>
    private void DestroyObjectBullet(int i)
    {
        //プールを使用してオブジェクトを消すかどうか。
        if (Data.IsPool)
        {
            //値の初期化と、プール
            _bulletsList[i].obj.SetActive(false);
            _bulletsList[i].transform.eulerAngles = Vector3.zero;
        }
        else
        {
            //プールを使用しないで、オブジェクトを消す。
            UnityEngine.Object.Destroy(_bulletsList[i].obj);
            _bulletsList.Remove(_bulletsList[i]);
        }
    }

    public void AllBulletControl()
    {
        for(int i=0;i<_bulletsList.Count;i++)
        {
            //弾が存在するかどうか
            if (!_bulletsList[i].obj.activeSelf) continue;

            //画面外に弾があるか調べる
            if (OutScreen(_bulletsList[i].transform.position,SerializeGameData.GameData.gameCam)) DestroyObjectBullet(i);

            //弾を動かす。
            _bulletsList[i].transform.position = BulletMove(_bulletsList[i].transform);
        }
    }

    //画面外に存在するか調べる。
    public bool OutScreen(Vector3 pos,Camera cam)
    {
        Vector3 screenPos = cam.WorldToViewportPoint(pos);
        return !rect.Contains(screenPos);
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

    /// <summary>弾のエンティティプレハブを生成する </summary>
    public void CreateEntityBullet(Vector3 pos, Vector3 rot)
    {
        //エンティティを新規生成する。
        Entity bullet = Data.EntityManager.Instantiate(_bulletPrefab);

        //生成位置と生成角度の設定
        Data.EntityManager.SetComponentData(bullet, new Translation { Value = pos });
        Data.EntityManager.SetComponentData(bullet, new Rotation { Value = quaternion.Euler(rot) });
    }

    /// <summary>弾のエンティティを消す </summary>
    public void DestroyEntityBullet(Entity entity)
    {
        Data.EntityManager.DestroyEntity(entity);
    }

    /// <summary>弾のエンティティプレハブを生成する。</summary>
    public void CreateBulletEntityPrefab()
    {
        World defaultWorld = World.DefaultGameObjectInjectionWorld;
        Data.EntityManager = defaultWorld.EntityManager;

        // GameObjectプレハブからEntityプレハブへの変換
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        _bulletPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(SerializeDotsData.DotsData.bulletObj, settings);
        Data.EntityManager.AddComponent<BulletTag>(_bulletPrefab);
    }
}
