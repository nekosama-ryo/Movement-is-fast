using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>ゲーム上で使用するデータや処理を保持するクラス</summary>
static public class Data
{
    //ゲームマネージャーの起動状況
    static public bool IsGameManager = false;

    //キー情報
    static public readonly KeyCode UP = KeyCode.W;
    static public readonly KeyCode Down = KeyCode.S;
    static public readonly KeyCode Right = KeyCode.D;
    static public readonly KeyCode Left = KeyCode.A;

    static public readonly KeyCode AButton = KeyCode.J;
    static public readonly KeyCode BButton = KeyCode.K;

    //設定パラメータ
    public static bool IsOrientedObject = true;
    public static bool IsPool = true;
    public static bool IsAsync = true;
    private static int _bullet = 0;//現在の弾数
    public static int _maxBullet { get; private set; } = 1000;//弾の最大値
    public static int Bullet
    {
        get { return _bullet; }
        set
        {
            if (value < 0)
            {
                value = _maxBullet;
            }
            if (value > _maxBullet)
            {
                value = 0;
            }
            _bullet = value;
        }
    }//弾のプロパティ
    public static bool IsInvincibilily = false;

    //現在のシーン情報
    public static int SceneNumber { get; private set; } = 0;
    //それぞれのシーンの番号
    public const int TitleNumber = 0;
    public const int GameNumber = 1;

    /// <summary>初期化処理。最初期に呼び出す </summary>
    public static void StartSceneSetting()
    {
        SceneNumber = SceneManager.GetActiveScene().buildIndex;
    }
    /// <summary>シーンを設定する。 </summary>
    public static void SetScene(int SceneNum)
    {
        //ロード中は一度ゲームマネージャーを停止刺せる
        IsGameManager = false;
        //シーン遷移
        SceneNumber = SceneNum;
        SceneManager.LoadSceneAsync(SceneNum);
    }
}
