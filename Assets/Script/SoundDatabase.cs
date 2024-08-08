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
        //Enum�̍��ڂ�string�A���̐��l��int�ł܂Ƃ߂�
        List<string> itemNameList = new List<string>();

        foreach (SoundData soundData in soundDatas)
        {
            itemNameList.Add(soundData.name);
        }
#if UNITY_EDITOR
        //Enum�쐬
        EnumCreator.Create(
            enumName: "SoundName",    //enum�̖��O
            itemNameList: itemNameList,                //enum�̍���
            exportPath: "Assets/Database/Enum/SoundName.cs"  //�쐬�����t�@�C���̃p�X��Assets����g���q�܂Ŏw��
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