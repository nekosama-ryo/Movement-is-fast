using UnityEngine;
using UnityEngine.UI;

/// <summary>タイトルシーンの挙動 </summary>
public class TitleScene
{
    //AnyButtonのテキストの透明度を切り替える速度。
    private float _anyButtonColorSpeed = 0.002f;
    //AnyButtonの画面切り替え時に透明度を切り替える速度
    private float _anyButtonReplacedSpeed = 0.1f;

    //ボタンが押された際にAnyButtonが何秒後に切り替わるか
    private float _anyButtonReplacedTime = 0.3f;
    //ゲーム開始画面にあるオブジェクトが消えるまでの時間
    private float _titleObjDestroyTime = 0.3f;

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

    //非選択時のテキストの色、透明度
    private float _SelectOffTransparence = 0.6f;
    private float _SelectOnTransparence = 1f;
    private Color _selectOffColor = new Color(0.3f, 0.3f, 0.3f);

    private int _sumBullet = 100;

    //タイトル画面の位置Ｅｎｕｍ
    private TitleAction _titleAction = 0;

    private float _selectInstantiateTime = 0.5f;
    //秒数で処理を切り替える
    private float _countTime = 0f;



    public void OnStart()
    {
        _titleAction = TitleAction.Title_Start;
        SelectPoint = 0;
        SerializeTitleData.TitleData.selectBulletCountText.text = Data.Bullet.ToString();
    }

    /// <summary>タイトル画面で現在行う処理を呼び出す</summary>
    public void OnUpdate()
    {
        //それぞれの処理
        //※上から順番に処理が行われるので、メソッド分けするよりも辿りやすいと判断して、メソッド分けは行っていません。
        //一部似通っている処理もありましたが、中途半端に分けるよりも、こちらのほうが見やすいと判断しました。変えても問題はない。
        switch (_titleAction)
        {
            //ゲーム開始画面
            case TitleAction.Title_Start:
                {
                    //AnyButtonの透明度を動かす処理
                    SerializeTitleData.TitleData.anyButtonText.color = SetColorGraduallyTransparency(SerializeTitleData.TitleData.anyButtonText.color, ref _anyButtonColorSpeed);

                    //何かボタンが押されたら、次の画面へ移行する。
                    if (!Input.anyKeyDown) return;
                    //AnyButtonの文字を点滅させる処理に移行
                    _titleAction = TitleAction.Title_Flash;
                }
                return;

            //AnyButtonの文字を点滅させる。
            case TitleAction.Title_Flash:
                {
                    //AnyButtonの透明度を動かす処理
                    SerializeTitleData.TitleData.anyButtonText.color = SetColorGraduallyTransparency(SerializeTitleData.TitleData.anyButtonText.color, ref _anyButtonReplacedSpeed);
                    //時間をカウント
                    _countTime += Time.deltaTime;

                    //これ以降は時間経過後の処理
                    if (_countTime < _anyButtonReplacedTime) return;
                    //値のリセットと、ロゴの縮小処理に移行
                    _titleAction = TitleAction.Title_Shrink;
                    _countTime = 0;
                }
                return;

            //ロゴの縮小
            case TitleAction.Title_Shrink:
                {
                    //可読性が下がるので、一時変数にまとめた。
                    Transform[] transforms = SerializeTitleData.TitleData.titleObjTransforms;

                    //徐々に縮小
                    foreach (Transform transform in transforms)
                    {
                        transform.localScale = SetGraduallySizeY(transform.localScale, _titleObjDestroyTime, true);
                    }

                    //これ以降は縮小完了後の処理
                    if (!Mathf.Approximately(transforms[transforms.Length - 1].localScale.y, 0f)) return;
                    //選択画面の表示処理へ
                    _titleAction = TitleAction.Select_Expansion;
                }
                return;

            //選択画面の表示処理
            case TitleAction.Select_Expansion:
                {
                    //可読性が下がるので、一時変数にまとめた。
                    Transform[] transforms = SerializeTitleData.TitleData.selectObjTransforms;

                    //徐々に表示
                    foreach (Transform transform in transforms)
                    {
                        transform.localScale = SetGraduallySizeY(transform.localScale, _selectInstantiateTime);
                    }

                    //これ以降は縮小完了後の処理
                    if (!Mathf.Approximately(transforms[transforms.Length - 1].localScale.y, 1f)) return;
                    //選択画面の処理へ
                    _titleAction = TitleAction.Select;
                }
                return;

            //選択画面
            case TitleAction.Select:
                {
                    //上下のキー操作
                    SelectKey();

                    //指向の選択
                    SelectSetSwitchColor(0, SerializeTitleData.TitleData.oriented);
                    //プールの選択
                    SelectSetColor(1, SerializeTitleData.TitleData.selectPoolText, true);
                    //非同期の選択
                    SelectSetColor(2, SerializeTitleData.TitleData.selectAsyncText, true);
                    //弾数の選択
                    SetBullet(3, SerializeTitleData.TitleData.selectBulletCountText);
                    //無敵モードの選択
                    SelectSetSwitchColor(4, SerializeTitleData.TitleData.Invincibility);
                    //ゲームスタートの選択
                    GameStart(5, SerializeTitleData.TitleData.selectGameStartText);
                }
                return;
        }
    }

    /// <summary>
    /// 透明度を徐々に動かす
    /// </summary>
    /// <param name="color">動かしたい色情報</param>
    /// <param name="speed">動かすスピード</param>
    /// <returns>現在の透明度</returns>
    private Color SetColorGraduallyTransparency(Color color, ref float speed)
    {
        //透明度の加減算の切り替えを行う。
        if (Mathf.Approximately(color.a, 0) || Mathf.Approximately(color.a, 1))
        {
            speed *= -1;
        }

        //透明度を加減算で動かす。
        color.a = Mathf.Clamp(color.a + speed, 0, 1f);
        //透明度を設定する。
        return color;
    }

    /// <summary>
    /// 徐々に大きさを変える
    /// </summary>
    /// <param name="transform">変更するオブジェクトの大きさ</param>
    /// <param name="time">変更がおわる時間</param>
    /// <param name="max">最大サイズ</param>
    /// <param name="reduce">縮小</param>
    private Vector3 SetGraduallySizeY(Vector3 size, float time, bool reduce = false, float max = 1)
    {
        //縮小か拡大かを管理
        int minus = reduce ? -1 : 1;

        //ループ。拡大、縮小をおこなう
        size.y = Mathf.Clamp(size.y + Time.deltaTime * minus / time, 0, max);
        return size;
    }

    /// <summary>
    /// 選択画面のキー移動処理
    /// </summary>
    private void SelectKey()
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
    }

    /// <summary>色のオンオフを切り替える </summary>
    private Color SwitchColor(Color color)
    {
        return color == Color.white ? _selectOffColor : Color.white;
    }

    private Color SetTransparent(int number, Color color)
    {
        color.a = SelectPoint == number ? _SelectOnTransparence : _SelectOffTransparence;
        return color;
    }

    /// <summary>
    /// ボタンのオンオフ処理
    /// </summary>
    /// <param name="number">ボタン番号</param>
    /// <param name="color">色情報</param>
    /// <returns>選択状況に応じた色情報</returns>
    private void SelectSetColor(int number, Text text, bool dotsOff = false)
    {
        //Dotsに設定している際に、オンオフを切り替えられない。
        if (_isDotsMode() && dotsOff)
        {
            //強制オフ
            text.color = SetTransparent(number, _selectOffColor);
            return;
        }

        //選択位置に応じた透明度を設定
        text.color = SetTransparent(number, text.color);

        //選択番号時に、対応したボタンを押すとオンとオフを切り替える。
        if (Input.GetKeyDown(Data.AButton) && SelectPoint == number)
        {
            //オンとオフを切り替える
            text.color = SwitchColor(text.color);
        }
    }

    //現在Dotsに設定されているかどうか
    private bool _isDotsMode()
    {
        return SerializeTitleData.TitleData.oriented[1].color.r == 1 &&
               SerializeTitleData.TitleData.oriented[1].color.g == 1 &&
       SerializeTitleData.TitleData.oriented[1].color.b == 1 ? true : false;
    }

    /// <summary>
    /// 排他的な複数ボタンのオンオフ処理
    /// </summary>
    /// <param name="number">ボタン番号</param>
    /// <param name="texts">テキスト情報</param>
    private void SelectSetSwitchColor(int number, Text[] texts)
    {
        Color color;

        //現在位置が選択位置出なければ、透明にする。
        foreach (Text text in texts)
        {
            color = text.color;
            text.color = SetTransparent(number, color);
        }

        if (!Input.GetKeyDown(Data.AButton) || SelectPoint != number) return;

        //ボタンが押された際、オンとオフを切り替える。
        for (int i = 0; i < texts.Length; i++)
        {
            //最後の位置がオンだったときは、１から処理を始める
            if (texts[texts.Length - 1].color == Color.white)
            {
                //最初をオンにする。
                texts[0].color = Color.white;
                //最終位置をオフにする。
                texts[texts.Length - 1].color = _selectOffColor;
                //これ以上探索の必要がないので終了する。
                return;
            }

            //現在位置がオンだったときは、オンオフを切り替えて、２個先から再処理
            if (texts[i].color != Color.white) continue;

            //オンオフの変更
            texts[i].color = SwitchColor(texts[i].color);
            //処理位置を次のボタンへ
            i++;
            //オンオフの変更
            texts[i].color = SwitchColor(texts[i].color);
            //これ以上探索の必要がないので終了する。
            return;
        }
    }

    private void SetBullet(int number, Text text)
    {
        //選択位置に応じた透明度を設定
        text.color = SetTransparent(number, text.color);

        //自分の位置にカーソルがない場合は以降の処理は行わない。
        if (number != SelectPoint) return;

        //弾の加減
        if (Input.GetKeyDown(Data.Right))
        {
            Data.Bullet += _sumBullet;
        }
        if (Input.GetKeyDown(Data.Left))
        {
            Data.Bullet -= _sumBullet;
        }

        //文字の更新
        SerializeTitleData.TitleData.selectBulletCountText.text = Data.Bullet.ToString();
    }

    /// <summary>ゲームの開始処理 </summary>
    private void GameStart(int number, Text text)
    {
        //選択位置に応じた透明度を設定
        text.color = SetTransparent(number, text.color);
        if (number != SelectPoint || !Input.GetKeyDown(Data.AButton)) return;

        //テキストの透明度からゲームの設定状態を決定する。
        Data.IsOrientedObject = CheckTransparence(SerializeTitleData.TitleData.oriented[0].color);
        Data.IsPool = CheckTransparence(SerializeTitleData.TitleData.selectPoolText.color);
        Data.IsAsync = CheckTransparence(SerializeTitleData.TitleData.selectAsyncText.color);
        Data.IsInvincibilily = CheckTransparence(SerializeTitleData.TitleData.Invincibility[0].color);
        //シーン切り替え
        if (Data.IsOrientedObject)
        {
            Data.SetScene(Data.GameSceneNumber);
        }
        else
        {
            Data.SetScene(Data.DataSceneNumber);
        }
    }

    /// <summary>色の透明度からフラグを設定する </summary>
    private bool CheckTransparence(Color color)
    {
        return Mathf.Approximately(color.r, 1) && Mathf.Approximately(color.g, 1) && Mathf.Approximately(color.b, 1);
    }
}

//処理段階をEnumに分けて、行う。
//もっといい作り方があるはず。思いついたら修正
enum TitleAction
{
    Title_Start,//スタート画面
    Title_Flash,//テキストの点滅
    Title_Shrink,//タイトルロゴの収縮
    Select_Expansion,//ウィンドウの出現
    Select,//選択画面
    GameStartFlash//選択画面
}
