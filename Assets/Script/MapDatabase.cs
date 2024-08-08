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
        //Enumの項目をstring、その数値をintでまとめる
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
                // 失敗した場合こっち
                // ここで、今Enumにも、itemDictの中にもない数値を新しい数値として与える
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
        //Enum作成
        EnumCreator.Create(
            enumName: "MapName",    //enumの名前
            itemDict: itemDict,                //enumの項目
            exportPath: "Assets/Database/Enum/MapName.cs"  //作成したファイルのパスをAssetsから拡張子まで指定
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

