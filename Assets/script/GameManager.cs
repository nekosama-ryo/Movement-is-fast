using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>全てのゲーム内の動作を管理する </summary>
public class GameManager : MonoBehaviour
{
    //シングルトン
    private static GameManager _gameManager = default;

    [SerializeField, Header("シリアライズで拾ったデータ")] private SerializeData _gameData = default;

    //スクリプト
    private TitleScene _titleScr = new TitleScene();
    private SkyBoxControl _camScr;
    //private GameScene _gameScr = new GameScene();
    //private FPS _fpsScr = new FPS();

    private void Awake()
    {
        //シングルトン処理
        SingletonSetting();
        //データの初期処理。現在のシーン番号を保存する。
        Data.StartSceneSetting();
        //シーン読み込み時に呼び出す処理を追加することで、疑似Startメソッドを作成
        SceneManager.sceneLoaded += OnSceneLoaded;


        _gameData.SetSerializeData(_gameData);

        _camScr = new SkyBoxControl();
    }

    private void Update()
    {
        //ゲームマネージャーの起動状況を確認
        if (!Data.IsGameManager)
        {
            return;
        }

        //各シーンで行う処理
        switch (Data.SceneNumber)
        {
            //タイトルシーンの動作
            case Data.TitleNumber:
                _titleScr.OnUpdate();
                break;

            //ゲームシーンの動作
            case Data.GameNumber:
                //_gameScr.GameAction();
                //_fpsScr.FPSCount();
                break;
        }

        //全てのシーンで行う処理
        _camScr.CameraMoveTitle();

        //仮置きリセット
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Data.SetScene(Data.TitleNumber);
        }
    }

    /// <summary>Startと同じ動作。シーンが切り替わる際に呼び出される。</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //疑似スタート処理。それぞれのシーンで最初期に行う処理
        switch (Data.SceneNumber)
        {
            //タイトルシーン
            case Data.TitleNumber:

                break;

            //ゲームシーン
            case Data.GameNumber:

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
