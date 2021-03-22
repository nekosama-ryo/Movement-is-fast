using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>全てのゲーム内の動作を管理する </summary>
public class GameManager : MonoBehaviour
{
    //シングルトン
    private static GameManager _gameManager = default;

    //それぞれのシーンのデータ情報
    private SerializeTitleData _titleData = default;
    private SerializeGameData _gameData = default;
    private SerializeDotsData _dotsData = default;

    //スクリプト
    private TitleScene _titleScr = new TitleScene();
    private SkyBoxControl _camScr;
    private ObjectGameScene _gameScr = new ObjectGameScene();

    private void Awake()
    {
        //シングルトン処理
        SingletonSetting();
        //データの初期処理。現在のシーン番号を保存する。
        Data.StartSceneSetting();
        //シーン読み込み時に呼び出す処理を追加することで、疑似Startメソッドを作成
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        //スカイボックスを動かす。
        _camScr = new SkyBoxControl();
    }

    private void Update()
    {
        //ゲームマネージャーの起動状況を確認
        if (!Data.IsGameManager) return;

        //各シーンで行う処理
        switch (Data.SceneNumber)
        {
            //タイトルシーンの動作
            case Data.TitleSceneNumber:
                _titleScr.OnUpdate();
                break;

            //オブジェクト指向ゲームシーンの動作
            case Data.GameSceneNumber:
                _gameScr.OnUpdate();
                break;

            //データ指向ゲームシーンの動作
            case Data.DotsSceneNumber:
                break;
        }

        //全てのシーンで行う処理
        _camScr.OnUpdate();
        EscAction();
    }

    /// <summary>Escが押された時の処理</summary>
    private void EscAction()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Data.SceneNumber==Data.TitleSceneNumber)
            {
                //アプリケーションの終了
                Application.Quit();
            }
            else
            {
                //タイトルに戻る
                _gameScr.Cancellation();
                Data.SetScene(Data.TitleSceneNumber);
            }
        }
    }

    /// <summary>Startと同じ動作。シーンが切り替わる際に呼び出される。</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //多重起動の防止
        if (Data.IsGameManager) return;

        //疑似スタート処理。それぞれのシーンで最初期に行う処理
        switch (Data.SceneNumber)
        {
            //タイトルシーン
            case Data.TitleSceneNumber:
                {
                    //タイトルシーンのデータを取得
                    _titleData = GameObject.FindGameObjectWithTag(Data.TitleDataTagName).GetComponent<SerializeTitleData>();
                    //取得したデータを設定する。
                    _titleData.SetSerializeData(_titleData);
                    //タイトルシーンのスタートメソッド
                    _titleScr.OnStart();
                }
                break;

            //ゲームシーン
            case Data.GameSceneNumber:
                {
                    //ゲームシーンのデータを取得
                    _gameData = GameObject.FindGameObjectWithTag(Data.GameDataTagName).GetComponent<SerializeGameData>();
                    //取得したデータを設定する。
                    _gameData.SetSerializeData(_gameData);
                    //ゲームシーンのスタートメソッド
                    _gameScr.OnStart();
                }
                break;

            //Dots使用のゲームシーン
            case Data.DotsSceneNumber:
                {
                    //Dotsゲームシーンのデータを取得
                    _dotsData = GameObject.FindGameObjectWithTag(Data.DotsDataTagName).GetComponent<SerializeDotsData>();
                    //取得したデータを設定する。
                    _dotsData.SetSerializeData(_dotsData);
                }
                break;
        }

        //起動状況を変更
        Data.IsGameManager = true;
    }

    /// <summary>シングルトン処理</summary>
    private void SingletonSetting()
    {
        if (_gameManager == null)
        {
            _gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
