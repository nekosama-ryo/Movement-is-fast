using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene
{
    private GameObject _bossObj = default;

    //生成間隔
    private int _createFrame = 1000;
    //スクリプト
    private ObjectPlayer _playerScr = default;
    private CreateBullet _bulletScr = default;

    public GameScene()
    {
        //_bossObj = GameObject.Find("Boss");

        _playerScr = new ObjectPlayer();
        _bulletScr = new CreateBullet();
    }

    public void GameAction()
    {
        //オブジェクト指向
        if (Data.IsOrientedObject)
        {
            //プレイヤーを動かす処理
            _playerScr.PlayerAction();
            //弾の生成
            _bulletScr.BulletAction(_bossObj.transform.position, _createFrame);
        }
        //データ指向
        else
        {

        }
    }
}
