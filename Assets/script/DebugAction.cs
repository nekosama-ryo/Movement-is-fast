using UnityEngine;

public class DebugAction : MonoBehaviour
{
    //デバッグ用のタイマー
    private System.Diagnostics.Stopwatch _debugSystem = new System.Diagnostics.Stopwatch();
    //デバッグスタート時に多重起動されないように。
    private bool _isDebug = true;

    /// <summary>デバッグの時間計測を開始する</summary>
    public void DebugStart()
    {
        //デバッグ開始
        if(_isDebug)
        {
            _debugSystem.Start();
            _isDebug = false;
        }
    }

    /// <summary>ここまでに到達した時間を表示する</summary>
    public void DebugTime()
    {
        //デバッグの停止
        Debug.Log(_debugSystem.ElapsedMilliseconds + "ミリ秒掛かりました。");
    }

    /// <summary>デバッグのタイマーを停止する</summary>
    public void DebugStop()
    {
        _debugSystem.Stop();
        _isDebug = true;
    }
}
