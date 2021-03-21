using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot
{
    private float _angle = 0;
    private Vector3 _createAngle = Vector3.zero;
    private System.Action<Vector3,Vector3> _bulletCreate;

    public void OnStart(System.Action<Vector3,Vector3>createAction)
    {
        _angle = 360f/Data.Bullet;
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
        //初期の生成位置をずらす
        _createAngle.y += _angle * 0.5f;
    }
}
