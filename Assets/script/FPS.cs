using UnityEngine;
using UnityEngine.UI;

public class FPS
{
    private int _frameCount = 0;
    private float _time = 0f;
    private float _previewTime = 0f;
    private float _fps = 0;

    private Text _text=default;

    public FPS()
    {
        _text = GameObject.Find("Timer").GetComponent<Text>();
    }

    public void FPSCount()
    {
        //フレームをカウント
        _frameCount++;
        //経過時間をカウント
        _time = Time.realtimeSinceStartup - _previewTime;

        //０．５秒後に行う。
        if(_time>=0.5f)
        {
            _fps = _frameCount / _time;
            _text.text = "F P S : " + _fps.ToString();

            //リセット
            _frameCount = 0;
            _previewTime = Time.realtimeSinceStartup;
        }
    }
}
