using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>ゲーム上で使用するデータや処理を保持するクラス</summary>
static public class Data
{
    //ゲームマネージャーの起動状況
    static public bool IsGameManager = false;
    //エンティティマネージャー
    static public EntityManager EntityManager = new EntityManager();

    //キー情報
    static public readonly KeyCode UP = KeyCode.UpArrow;
    static public readonly KeyCode Down = KeyCode.DownArrow;
    static public readonly KeyCode Right = KeyCode.RightArrow;
    static public readonly KeyCode Left = KeyCode.LeftArrow;

    static public readonly KeyCode AButton = KeyCode.Space;
    static public readonly KeyCode BButton = KeyCode.LeftShift;

    //タイトル画面のパラメータ
    public const float AnyButtonColorSpeed = 0.002f; //AnyButtonのテキストの透明度を切り替える速度。
    public const float AnyButtonReplacedSpeed = 0.1f;//AnyButtonの画面切り替え時に透明度を切り替える速度
    public const float AnyButtonReplacedTime = 0.3f;//ボタンが押された際にAnyButtonが何秒後に切り替わるか
    public const float TitleObjDestroyTime = 0.3f;//ゲーム開始画面にあるオブジェクトが消えるまでの時間
    public const float SelectInstantiateTime = 0.5f;//次の画面が生成されるまでの時間

    public const int SelectPointMax = 5;//選択画面の参照位置の最大値

    public const float SelectOffTransparence = 0.6f;//非選択時の透明度
    public const float SelectOnTransparence = 1f;//選択時の透明度
    static public Color SelectOffColor = new Color(0.3f, 0.3f, 0.3f);//非選択時の色
    public const int SumBullet = 100;//選択時に弾を一度に足す量


    //プレイヤーパラメータ
    public const int PlayerIncline = 30;//プレイヤーの機体の傾き具合
    public const float PlayerDotsIncline = 0.5f;//Dots使用時のプレイヤーの機体の傾き具合
    public const float PlayerMaxSpeed = 0.3f;//最高速度
    public const float PlayerLowSpeed = 2.5f;//プレイヤーの低速時の割る量
    static public readonly Vector3 PlayerParticleRot = new Vector3(-3.1f, 0f, 0f);//プレイヤーのパーティクルのずれ

    //弾のパラメータ
    public const float BulletSpeed = 0.05f;//弾速
    public const float BulletCreateFrame = 2;//生成間隔
    public const float BulletDotsSpeed = 0.49f;//Dots時の弾速調整
    private static int MaxBullet { get; set; } = 1000;//弾の最大値

    //FPSパラメータ
    static public int FpsMaxCount { get; private set; } = 200;//平均を算出する最大回数

    //設定パラメータ（ゲーム内で設定される）
    static public bool IsOrientedObject = true;
    static public bool IsPool = true;
    static public bool IsAsync = true;
    static private int BulletNumber = 0;//現在の弾数
    static public int Bullet
    {
        get { return BulletNumber; }
        set
        {
            if (value < 0)
            {
                value = MaxBullet;
            }
            if (value > MaxBullet)
            {
                value = 0;
            }
            BulletNumber = value;
        }
    }//弾のプロパティ
    static public bool IsInvincibilily = false;

    //現在のシーン情報
    static public int SceneNumber { get; private set; } = 0;

    //それぞれのシーンの番号
    public const int TitleSceneNumber = 0;//タイトル
    public const int GameSceneNumber = 1;//オブジェクト指向のゲームシーン
    public const int DotsSceneNumber = 2;//データ指向のゲームシーン

    //それぞれのデータタグ名
    public const string TitleDataTagName = "TitleData";
    public const string GameDataTagName = "GameData";
    public const string DotsDataTagName = "DotsData";

    //処理
    /// <summary>初期化処理。最初期に呼び出す </summary>
    static public void StartSceneSetting()
    {
        SceneNumber = SceneManager.GetActiveScene().buildIndex;
    }
    /// <summary>シーンを設定する。 </summary>
    static public void SetScene(int SceneNum)
    {
        //ロード中は一度ゲームマネージャーを停止刺せる
        IsGameManager = false;
        //シーン遷移
        SceneNumber = SceneNum;
        SceneManager.LoadSceneAsync(SceneNum);
    }
}
