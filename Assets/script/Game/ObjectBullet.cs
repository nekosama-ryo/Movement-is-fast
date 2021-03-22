using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>オブジェクト指向の弾の挙動</summary>
public class ObjectBullet
{
    //弾の生成処理
    private Bullet _bulletScr = new Bullet();

    public async UniTask OnAsyncStart(CancellationToken ct,Transform trs,float time)
    {
        //ゲームマネージャーが止まるまで非同期で動かす。
        while (true)
        {
            //通常の処理を非同期で行う。
            OnUpdate(trs.position,time);
            //1フレーム待機
            await UniTask.Delay(1);

            //ゲームマネージャーが停止したら、キャンセル処理を走らせる。
            if (Data.IsGameManager) continue;
            ct.ThrowIfCancellationRequested();
        }
    }

    public void OnStart()
    {
        //弾の生成方法を指定する。
        _bulletScr.OnStart();
    }

    public void OnUpdate(Vector3 pos,float time)
    {
        //弾の動作
        _bulletScr.ObjectBulletMove();
        //弾の発射・生成
        _bulletScr.ObjectBulletShoot(pos,time);
    }
}
