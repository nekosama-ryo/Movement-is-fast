using UnityEngine;
/// <summary>カメラの挙動を制御する</summary>
public class SkyBoxControl
{
    //スカイボックス
    private Material _skyBox = default;

    //コンストラクタ
    public SkyBoxControl()
    {
        //スカイボックスの取得
        _skyBox = RenderSettings.skybox;
    }

    public void OnUpdate()
    {
        //徐々にSkyBoxを回転させる。
        _skyBox.SetFloat("_Rotation", Mathf.Repeat(_skyBox.GetFloat("_Rotation")-Time.deltaTime,360f));
    }
}
