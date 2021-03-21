using Unity.Entities;

[GenerateAuthoringComponent]
public struct BulletTag : IComponentData
{
}

/*
 ・生成時にローテーションを設定。
 ・常時処理するのは直線移動

必要
・エンティティprefab
・回転情報

・正面
 
 */
