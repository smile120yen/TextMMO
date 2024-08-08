using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Database/SoundDatabase")]
public class SoundDatabase : Database
{
    [SerializeField]
    List<SoundData> _soundDatas = new List<SoundData>();
    public List<SoundData> soundDatas => _soundDatas;

    public override void createEnum()
    {
        //Enumの項目をstring、その数値をintでまとめる
        List<string> itemNameList = new List<string>();

        foreach (SoundData soundData in soundDatas)
        {
            itemNameList.Add(soundData.name);
        }
#if UNITY_EDITOR
        //Enum作成
        EnumCreator.Create(
            enumName: "SoundName",    //enumの名前
            itemNameList: itemNameList,                //enumの項目
            exportPath: "Assets/Database/Enum/SoundName.cs"  //作成したファイルのパスをAssetsから拡張子まで指定
        );
#endif
    }
}

[System.Serializable]
public class SoundData
{
    [SerializeField] public string name;
    [SerializeField] public AudioClip audioClip;
}