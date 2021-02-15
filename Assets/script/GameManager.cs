using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>全てのゲーム内の動作を管理する </summary>
public class GameManager : MonoBehaviour
{
    //タイトルシーンの現在の挙動を呼び出す。
    private TitleScene _titleScr=default;
    private CameraControl _camScr = default;

    //設定パラメータ
    public static bool IsOrientedObject = false;
    public static bool IsPool = false;
    public static bool Isasync = false;
    //現在の弾数
    private static int _bullet = 0;
    //弾の最大値
    private static int _maxBullet = 100;
    //弾のプロパティ
    public static int Bullet 
    {
        get { return _bullet; }
        set
        {
            if(value<0)
            {
                value = 0;
            }
            if(value>=_maxBullet)
            {
                value = _maxBullet;
            }
            _bullet = value;
        }
    }
    public static bool IsInvincibilily = false;

    private void Awake()
    {
    }

    void Start()
    {
        _titleScr = new TitleScene();
        _camScr = new CameraControl();
    }

    void Update()
    {
        _titleScr.TitleAction();
        _camScr.CameraAction();
    }
}
