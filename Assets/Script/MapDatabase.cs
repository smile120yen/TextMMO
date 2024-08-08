using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "MapDatabase", menuName = "Database/MapDatabase")]
public class MapDatabase : Database
{
    [SerializeField]
    List<MapData> _mapDatas = new List<MapData>();
    public List<MapData> mapDatas => _mapDatas;

    public override void createEnum()
    {
        //Enum�̍��ڂ�string�A���̐��l��int�ł܂Ƃ߂�
        Dictionary<string, int> itemDict = new Dictionary<string, int>();

        itemDict.Add("None",0);
        foreach (MapData mapData in mapDatas)
        {
            if (Enum.TryParse(mapData.uniqueName, out MapName result))
            {
                itemDict.Add(mapData.uniqueName,(int)result);
            }
            else
            {
                // ���s�����ꍇ������
                // �����ŁA��Enum�ɂ��AitemDict�̒��ɂ��Ȃ����l��V�������l�Ƃ��ė^����
                int count=0;
                while (true)
                {

                    Debug.Log(mapData.uniqueName + "," + count);
                    bool isFind = false;
                    foreach (MapName value in Enum.GetValues(typeof(MapName)))
                    {
                        Debug.Log((int)value);
                        if (count == (int)value)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    foreach (KeyValuePair<string, int> keyValuePair in itemDict)
                    {
                        Debug.Log((int)keyValuePair.Value);
                        if (count == (int)keyValuePair.Value)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        break;
                    }

                    count++;

                }

                itemDict.Add(mapData.uniqueName, count);
            }

        }
#if UNITY_EDITOR
        //Enum�쐬
        EnumCreator.Create(
            enumName: "MapName",    //enum�̖��O
            itemDict: itemDict,                //enum�̍���
            exportPath: "Assets/Database/Enum/MapName.cs"  //�쐬�����t�@�C���̃p�X��Assets����g���q�܂Ŏw��
        );
#endif
    }
}

[System.Serializable]
public class MapData
{
    [SerializeField] public string name;
    [SerializeField] public string uniqueName;
    [SerializeField] public List<ActionInfo> infoTexts;
    [SerializeField] public List<ActionInfo> mapMovementActions;
    [SerializeField] public List<EnemyData> enemyDatas;
}

[System.Serializable]
public class ActionInfo
{
    [SerializeField] public string text;
    [SerializeField] public List<ActionData> actionDatas;
    [SerializeField] public Color color;

    public ActionInfo()
    {
        actionDatas = new List<ActionData>();
        color = Color.white;
    }
}


[System.Serializable]
public class ActionData
{
    [SerializeField] public Action action;
    [SerializeField] public ActionArgs args;
    public ActionData()
    {
        args = new ActionArgs();
    }
}

