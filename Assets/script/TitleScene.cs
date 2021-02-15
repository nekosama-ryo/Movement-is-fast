using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>タイトルシーンの挙動 </summary>
public class TitleScene
{
    //現在行う処理。こいつを通して処理を呼び出すことで、pubulicを作りすぎずに完結できる。後々処理が増えても大丈夫な拡張性。
    private System.Action titleAction = default;

    #region ーーーーーーゲーム開始画面で使用する変数ーーーーーーー
    //ゲーム開始画面に存在するオブジェクト群
    private GameObject _titleLogoObj = default;
    private GameObject _anyButtonObj = default;

    //AnyButtonのテキスト。透明度を徐々に変える。
    private Text _anyButtonText = default;
    private Color _anyButtonColor = new Color(1, 1, 1, 1);

    //AnyButtonのテキストの透明度を切り替える速度。
    private float _anyButtonColorSpeed = 0.002f;
    //AnyButtonの画面切り替え時に透明度を切り替える速度
    private float _anyButtonReplacedSpeed = 0.1f;

    //ボタンが押された際にAnyButtonが何秒後に切り替わるか
    private float _anyButtonReplacedTime = 0.3f;
    private float _anyButtonCountTime = 0;

    //ゲーム開始画面にあるオブジェクトが消えるまでの時間
    private float _titleObjDestroyTime = 0.3f;
    #endregion

    #region ーーーーーー選択画面で使用する変数ーーーーーーー
    //ゲーム開始画面に存在するオブジェクト群
    private GameObject _selectWindowObj = default;
    private GameObject _selectTextObj = default;

    private Text _selectObjectText = default;
    private Text _selectDataText = default;
    private Text _selectPoolText = default;
    private Text _selectAsyncText = default;
    private Text _selectBulletCountText = default;
    private Text _selectOnText = default;
    private Text _selectOffText = default;
    private Text _selectGameStartText = default;

    //選択画面で現在どこを参照しているか
    private int _selectPoint = 0;
    //選択画面の参照位置の最大値
    private const int _selectPointMax = 5;
    //選択位置のプロパティ
    private int SelectPoint
    {
        get { return _selectPoint; }
        set
        {
            if (value > _selectPointMax)
            {
                value = 0;
            }
            if (value < 0)
            {
                value = _selectPointMax;
            }
            _selectPoint = value;
        }
    }

    //選択画面オブジェクトのサイズ。
    private float _selectWindowSize = 1f;
    private float _selectTextSize = 1f;
    //選択画面オブジェクトが表示完了するまでの時間
    private float _selectWindowTime = 0.5f;

    private bool _orientedData = false;
    //非選択時のテキストの色、透明度
    private float _SelectOffTransparence = 0.6f;
    private Color _selectOffColor = new Color(0.3f, 0.3f, 0.3f);

    private int _sumBullet = 10;
    #endregion

    //コンストラクタ
    public TitleScene()
    {
        //ゲーム開始画面のオブジェクト
        _titleLogoObj = GameObject.Find("TitleLogo");
        _anyButtonObj = GameObject.Find("Press Any Button");
        _anyButtonText = _anyButtonObj.GetComponent<Text>();

        //選択画面のオブジェクト
        _selectObjectText = GameObject.Find("Object").GetComponent<Text>();
        _selectDataText = GameObject.Find("Data").GetComponent<Text>();
        _selectPoolText = GameObject.Find("Pool").GetComponent<Text>();
        _selectAsyncText = GameObject.Find("Async").GetComponent<Text>();
        _selectBulletCountText = GameObject.Find("BulletCount").GetComponent<Text>();
        _selectOnText = GameObject.Find("ON").GetComponent<Text>();
        _selectOffText = GameObject.Find("OFF").GetComponent<Text>();
        _selectGameStartText = GameObject.Find("GameStart").GetComponent<Text>();
        _selectWindowObj = GameObject.Find("SelectWindow");
        _selectTextObj = GameObject.Find("SelectText");

        //ゲーム開始時の処理を設定する
        SetAction(StartScene);
    }

    /// <summary>タイトル画面で現在行う処理を呼び出す</summary>
    public void TitleAction()
    {
        //現在行いたい処理を返す。
        titleAction();
    }

    /// <summary>現在行う処理を変更する</summary>
    private void SetAction(System.Action newAction)
    {
        //現在呼び出す処理が同じだった場合変更しない。
        if (titleAction != newAction)
        {
            //処理の変更
            titleAction = newAction;
        }
    }

    /// <summary>ゲーム開始画面の処理</summary>
    private void StartScene()
    {
        //何かボタンが押されたら、次の画面へ移行する。
        if (Input.anyKeyDown)
        {
            //画面を切り替える前処理を呼び出す
            SetAction(StartSceneNext);
        }

        //AnyButtonの透明度を動かす処理
        SetAnyButtonTransparency(ref _anyButtonColorSpeed);
    }

    /// <summary>AnyButtonのテキストの透明度を動かす</summary>
    private void SetAnyButtonTransparency(ref float speed)
    {
        //透明度の加減算の切り替えを行う。
        if (_anyButtonColor.a == 0 || _anyButtonColor.a == 1)
        {
            speed *= -1;
        }

        //透明度を加減算で動かす。
        _anyButtonColor.a = Mathf.Clamp(_anyButtonColor.a + speed, 0, 1f);
        //透明度を設定する。
        _anyButtonText.color = _anyButtonColor;
    }

    /// <summary>ゲーム開始画面から次の画面へ移行するまでの処理</summary>
    private void StartSceneNext()
    {
        //一定秒数後にオブジェクトを消す
        if (_anyButtonCountTime > _anyButtonReplacedTime)
        {
            //オブジェクトが完全に消えたら、次の画面に移行する。
            if (_titleLogoObj.transform.localScale.y == 0 && _anyButtonObj.transform.localScale.y == 0)
            {
                SetAction(SelectScene);
            }

            //徐々にタイトルロゴと、AnyButtonの文字を消す処理
            _titleLogoObj.transform.localScale = GraduallySizeY(-_titleObjDestroyTime, _titleLogoObj.transform.localScale, 2);
            _anyButtonObj.transform.localScale = GraduallySizeY(-_titleObjDestroyTime, _anyButtonObj.transform.localScale, 1);
        }
        else
        {
            //秒数をカウントし、数秒後に処理を切り替える。
            _anyButtonCountTime += Time.deltaTime;
            //AnyButtonを点滅させる
            SetAnyButtonTransparency(ref _anyButtonReplacedSpeed);
        }
    }

    /// <summary>徐々にオブジェクトのY軸を広げて表示させる</summary>
    /// <param name="time">最大、最小サイズになるまでの時間。マイナス値で切り替わる</param>
    /// <param name="size">変えたいオブジェクトの大きさ</param>
    /// <param name="max">最大サイズ</param>
    private Vector3 GraduallySizeY(float time, Vector3 size, float max)
    {
        size.y = Mathf.Clamp(size.y + Time.deltaTime / time, 0, max);
        return size;
    }

    /// <summary>選択画面の処理 </summary>
    private void SelectScene()
    {
        //選択画面のウィンドウの表示が終わっているかどうか。
        if (_selectWindowObj.transform.localScale.y >= _selectWindowSize && _selectTextObj.transform.localScale.y >= _selectTextSize)
        {
            //上キー処理
            if (Input.GetKeyDown(Data.UP))
            {
                SelectPoint -= 1;
            }
            //下キー処理
            if (Input.GetKeyDown(Data.Down))
            {
                SelectPoint += 1;
            }

            //選択の処理
            //指向
            (_selectObjectText.color, _selectDataText.color) = SelectColorSwitch(0, (_selectObjectText.color, _selectDataText.color));
            //プール
            _selectPoolText.color = SelectColor(1, _selectPoolText.color);
            //非同期
            _selectAsyncText.color = SelectColor(2, _selectAsyncText.color);
            //弾数
            _selectBulletCountText.color = SelectColor(3, _selectBulletCountText.color, false, () =>
            {
                //弾を増やす処理
                if (Input.GetKeyDown(Data.Right))
                {
                    GameManager.Bullet += _sumBullet;
                    _selectBulletCountText.text = System.Convert.ToString(GameManager.Bullet);
                }
                //弾を減らす処理
                if (Input.GetKeyDown(Data.Left))
                {
                    GameManager.Bullet -= _sumBullet;
                    _selectBulletCountText.text = System.Convert.ToString(GameManager.Bullet);
                }
            });
            //無敵モード
            (_selectOnText.color, _selectOffText.color) = SelectColorSwitch(4, (_selectOnText.color, _selectOffText.color));
            //ゲーム開始処理のテキスト
            _selectGameStartText.color = SelectColor(5, _selectGameStartText.color, false, () =>
            {
                //ゲーム開始
            });
        }
        else
        {
            //選択ウィンドウとテキストの表示処理
            _selectWindowObj.transform.localScale = GraduallySizeY(_selectWindowTime, _selectWindowObj.transform.localScale, _selectWindowSize);
            _selectTextObj.transform.localScale = GraduallySizeY(_selectWindowTime, _selectTextObj.transform.localScale, _selectTextSize);
        }
    }

    /// <summary>選択位置の色を変える処理 </summary>
    private Color SelectColor(int selectPoint, Color color, bool switchOff = true, System.Action buttonAction = null)
    {
        //選択位置が自分の位置になったら表示する
        if (SelectPoint == selectPoint)
        {
            //文字の透明度をなくす
            color.a = 1;

            //決定が押された際の処理&特殊な処理が必要な場合かどうか
            if (switchOff)
            {
                if (Input.GetKeyDown(Data.AButton))
                {
                    //オンの時はオフ、オフの時はオンにする。
                    if (color.r < 1 && color.g < 1 && color.b < 1)
                    {
                        color = Color.white;
                    }
                    else
                    {
                        color = _selectOffColor;
                    }
                }
            }

            else
            {
                //特殊処理を呼び出す。
                buttonAction();
            }
        }
        else
        {
            //非選択時に文字を薄くする。
            color.a = _SelectOffTransparence;
        }

        return color;
    }

    /// <summary>選択位置の色を変える処理（複数選択） </summary>
    private (Color, Color) SelectColorSwitch(int selectPoint, (Color on, Color off) color)
    {
        //選択位置が自分の位置になったら表示する
        if (SelectPoint == selectPoint)
        {
            //文字の透明度をなくす
            color.on.a = 1;
            color.off.a = 1;

            //決定が押された際の処理
            if (Input.GetKeyDown(Data.AButton))
            {
                //それぞれのテキストのオンとオフを切り替える
                if (color.on.r < 1 && color.on.g < 1 && color.on.b < 1)
                {
                    color = (Color.white, _selectOffColor);
                }
                else
                {
                    color = (_selectOffColor, Color.white);
                }
            }
        }
        else
        {
            //非選択時に文字を薄くする。
            color.on.a = _SelectOffTransparence;
            color.off.a = _SelectOffTransparence;
        }

        return color;
    }

    /// <summary>ゲームの開始処理 </summary>
    private void GameStart()
    {

    }
}
