using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class FPS:MonoBehaviour
{
    //フレーム
    private int _frameCount = 0;
    //経過時間
    private float _time = 0f;
    //時間情報を保存
    private float _previewTime = 0f;
    //現在のFPS
    private float _fps = 0;

    //自身のテキスト
    private Text text = default;

    //平均FPSを求める
    private Queue<float> _fpsAverage =new Queue<float>();

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        //フレームをカウント
        _frameCount++;
        //経過時間をカウント
        _time = Time.realtimeSinceStartup - _previewTime;

        //FPSを出力
        if(_time>=0.5f)
        {
            //FPSを求める
            _fps = _frameCount / _time;
            //現在のFPS情報を保存
            _fpsAverage .Enqueue(_fps);

            //平均のカウント数が設定数を超えたら要素を減らす。
            if (_fpsAverage .Count > Data.FpsMaxCount)
            {
                _fpsAverage .Dequeue();
            }

            //平均を求める
            float average = 0;
            foreach(float i in _fpsAverage)
            {
                average += i;
            }

            //FPSをテキストに表示する
            text.text = "F P S : " + _fps.ToString()+ "\nAVERAGE : " + average/_fpsAverage.Count;

            //リセット
            _frameCount = 0;
            _previewTime = Time.realtimeSinceStartup;
        }
    }
}
