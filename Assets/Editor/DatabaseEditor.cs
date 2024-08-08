using UnityEditor;
using UnityEngine;

// 拡張するクラスを指定する
[CustomEditor(typeof(Database),true)]
// 継承クラスは Editor を設定する
public class DatabaseEditor : Editor
{
    // GUIの表示関数をオーバーライドする
    public override void OnInspectorGUI()
    {
        // targetを変換して対象スクリプトの参照を取得する
        Database transformPaste = target as Database;
        // public関数を実行するボタンの作成
        if (GUILayout.Button("Enum自動生成"))
        {
            transformPaste.createEnum();
        }
        // 元のインスペクター部分を表示
        base.OnInspectorGUI();
    }
}