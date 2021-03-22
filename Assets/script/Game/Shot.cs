using UnityEngine;

/// <summary>弾の発射の挙動</summary>
public class Shot
{
    //生成時にずらす角度
    private float _angle = 0;
    //弾の生成処理
    private System.Action<Vector3,Vector3> _bulletCreate=default;

    public void OnStart(System.Action<Vector3,Vector3>createAction)
    {
        //角度を求める
        _angle = 360f/Data.Bullet;
        //弾の生成方法を取得する
        _bulletCreate = createAction;
    }

    /// <summary>全方位に弾を生成する </summary>
    public void Omniazimuth(Vector3 pos)
    {
        Vector3 createAngle = Vector3.zero;
        for (int i = 0; i < Data.Bullet; i++)
        {
            //弾を生成する。
            _bulletCreate(pos, createAngle);

            //次回の生成角度を変える。
            createAngle.y += _angle;
        }
    }
}
