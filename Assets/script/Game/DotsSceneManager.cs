using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/// <summary> データ指向のゲームシーンの挙動</summary>
public class DotsSceneManager : SystemBase
{
    //プレイヤーの挙動計算
    private ObjectPlayer _playerScr = new ObjectPlayer();
    //弾の生成方法を取得
    private Bullet _bulletScr = new Bullet();
    //弾の発射
    private Shot _shotScr = new Shot();

    //現在のプレイヤーの速度
    private Vector3 _playerSpeed = Vector3.zero;
    //プレイヤーの移動時間管理
    private float _height = 0;
    private float _width = 0;

    protected override void OnStartRunning()
    {
        //エンティティプレハブを生成する。
        _bulletScr.CreateBulletEntityPrefab();
        //発射の初期処理
        _shotScr.OnStart(_bulletScr.CreateDotsBullet);
    }

    protected override void OnUpdate()
    {
        //ゲームマネージャー停止時にエンティティをリセットすることでOnStartRunningを強制的に呼び出す。
        if (!Data.IsGameManager)
        {
            EntityManager.DestroyEntity(EntityManager.UniversalQuery);
        }

        //Dotsシーンだったら以降の処理を行う。
        if (Data.SceneNumber != Data.DotsSceneNumber) return;

        //画面外の弾を消す
        Entities.WithAll<BulletTag>().WithStructuralChanges().ForEach((Entity entity,in LocalToWorld translation) =>
        {
            //画面外にいるかどうか
            if (_bulletScr.OutScreen(translation.Position, SerializeDotsData.DotsData.gameCam))
            {
                //エンティティの破棄
                Data.EntityManager.DestroyEntity(entity);
            }
        }).Run();

        //弾を真っすぐ飛ばす
        Entities.WithAll<BulletTag>().ForEach((ref Translation translation, in LocalToWorld localToWorld) =>
        {
            //正面に向けて移動
            translation.Value += localToWorld.Forward * Data.BulletSpeed * Data.BulletDotsSpeed;
            //マルチスレッド
        }).Schedule();

        //プレイヤーの情報を算出
        Vector3 playerTranslation = _playerSpeed = _playerScr.PlayerSpeed(_playerSpeed, Data.PlayerMaxSpeed, Data.PlayerLowSpeed, ref _height, ref _width);
        Vector3 playerRotetion = _playerScr.SetPlayerRot(_playerSpeed, Data.PlayerDotsIncline);
        Vector3 playerposition = Vector3.zero;

        //プレイヤーの挙動
        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Rotation rotation) =>
        {
            //移動
            translation.Value.x += playerTranslation.x;
            translation.Value.z += playerTranslation.z;

            //回転
            rotation.Value = quaternion.Euler(playerRotetion);

            //プレイヤーの位置情報を保存
            playerposition = translation.Value;
        }).Run();

        //ボタンが押されたら、プレイヤーの位置から弾を撃つ
        if (Input.GetKeyDown(Data.AButton))
        {
            //弾の生成
            _shotScr.Omniazimuth(playerposition);
        }

        //エンジンパーティクルの挙動
        Entities.WithAll<PlayerPaticleTag>().ForEach((ref Translation translation, ref Rotation rotation) =>
        {
            //位置調整
            translation.Value.x += playerTranslation.x;
            translation.Value.z += playerTranslation.z;

            //回転調整
            rotation.Value = quaternion.Euler(playerRotetion + Data.PlayerParticleRot);
        }).Run();
    }
}
