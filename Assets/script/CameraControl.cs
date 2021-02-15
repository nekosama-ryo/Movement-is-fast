using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>カメラの挙動を制御する</summary>
public class CameraControl
{
    //シングルトン
    static public CameraControl CameraManager = default;

    public System.Action _camAction = default;

    private GameObject _camObj = default;
    private Vector3 _cameraRot = default;

    //コンストラクタ
    public CameraControl()
    {
        _camAction = CameraMoveTitle;
        _camObj = GameObject.Find("Camera");
        _cameraRot = _camObj.transform.eulerAngles;
    }


    /// <summary>現在のカメラの挙動を返す</summary>
    public void CameraAction()
    {
        //現在行いたい処理を返す。
        _camAction();
    }

    private void CameraMoveTitle()
    {
        _cameraRot.y = Mathf.Repeat(_cameraRot.y - Time.deltaTime,360f);
        _camObj.transform.eulerAngles = _cameraRot;
    }
}
