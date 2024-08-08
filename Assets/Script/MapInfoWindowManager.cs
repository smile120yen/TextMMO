using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class MapInfoWindowManager : SingletonMonoBehaviour<MapInfoWindowManager>
{
    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform parent;
    public List<ActionInfo> additionalActions = new List<ActionInfo>();

    private void Start()
    {
        UpdateMapInfo();
        SaveDataManager.OnSaveDataUpdate.AddListener(() => UpdateMapInfo());
        OtherPlayerCharaInfoManager.Instance.OnPlayerInfoUpdate.AddListener(() => UpdateMapInfo());
        EnemyInfoManager.Instance.OnEnemyInfoUpdate.AddListener(() => UpdateMapInfo());
    }

    public void UpdateMapInfo()
    {

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        MapInfo mapInfo = SaveDataManager.saveData.mapInfo;

        AddText(mapInfo.mapData.name);

        foreach(ActionInfo actionInfo in mapInfo.mapData.infoTexts)
        {
            AddText(actionInfo);
        }

        //マップ側から追加されるテキスト
        foreach(ActionInfo actionInfo in additionalActions)
        {
            AddText(actionInfo);
        }
        AddText("   ");

        if (mapInfo.mapData.enemyDatas.Count > 0)
        {
            AddText(">同じマップにいる敵");

            if (EnemyInfoManager.Instance.isInitalEnemyInfo)
            {
                //マップにいる敵
                foreach (EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
                {
                    ActionInfo actionInfo = new ActionInfo();
                    actionInfo.actionDatas = new List<ActionData>();


                    //ActionAttackEnemyにスタミナ消費は移設
                    /*
                    ActionData stamina_actionData = new ActionData();
                    stamina_actionData.action = ScriptableObject.CreateInstance("ActionStaminaAdjustmentWithEquipBonus") as ActionStaminaAdjustmentWithEquipBonus;
                    stamina_actionData.args = new ActionArgs();
                    stamina_actionData.args.mount = -1;
                    actionInfo.actionDatas.Add(stamina_actionData); 
                    */

                    ActionData actionData = new ActionData();
                    actionData.action = ScriptableObject.CreateInstance("ActionAttackEnemy") as ActionAttackEnemy;
                    actionData.args = new ActionArgs();
                    actionData.args.targetID = enemyInfo.enemyId;
                    actionInfo.actionDatas.Add(actionData);

                    string str = enemyInfo.enemyData.enemyName + "( HP:" + enemyInfo.currentHp + "/" + enemyInfo.enemyData.enemyMaxHp +" ATK:"+ enemyInfo.enemyData.enemyAtk+ " )";

                    actionInfo.text = str;

                    if (enemyInfo.isPoisoned)
                    {
                        actionInfo.color = Color.magenta;
                    }
                    
                    if (enemyInfo.currentHp <= 0)
                    {
                        actionInfo.color = Color.gray;
                    }

                    AddText(actionInfo);
                }
            }
            else
            {
                AddText("読み込み中……");
            }
        }
        else
        {
            AddText(">このマップに敵はいません");
        }

        Dictionary<string,SaveData> otherPlayer = new Dictionary<string,SaveData>();

        foreach(KeyValuePair<string,SaveData> keyValuePair in OtherPlayerCharaInfoManager.Instance.playerInfo)
        {
            if (keyValuePair.Value.mapInfo.mapName == SaveDataManager.saveData.mapInfo.mapName
                && keyValuePair.Key != PhotonNetwork.LocalPlayer.UserId)
            {
                otherPlayer.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        if (otherPlayer.Count > 0)
        {
            AddText(">同じマップにいるひと");

            foreach(KeyValuePair<string, SaveData> keyValuePair in otherPlayer)
            {
                CharaInfo charaInfo = keyValuePair.Value.charaInfo;
                string userId = keyValuePair.Key;

                ActionInfo openCharaInfo = new ActionInfo();
                openCharaInfo.text = charaInfo.name + "( HP:" + charaInfo.hp + "/" + charaInfo.maxHp + " )";
                ActionData actionData = new ActionData();
                actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
                actionData.args = new ActionArgs();
                actionData.args.userId = userId;
                openCharaInfo.actionDatas.Add(actionData);

                AddText(openCharaInfo);
            }
        }
        else
        {
            AddText(">このマップには誰もいません");
        }
    }

    private void AddText(string text)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = text;
    }

    private void AddText(ActionInfo actionInfo)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = actionInfo.text;
        temp.GetComponent<TextObject>().color = actionInfo.color;
        temp.GetComponent<TextObject>().button.onClick.AddListener(
                () => {

                    bool isCanExecute = true;
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        if (!actionData.action.CheckCanExecute(actionData.args))
                        {
                            isCanExecute = false;
                            break;
                        }
                    }


                    if (isCanExecute)
                    {
                        foreach (ActionData actionData in actionInfo.actionDatas)
                        {
                            bool isContinue = actionData.action.ExecuteAction(actionData.args);
                            if (!isContinue) break;
                        }
                    }
                });
    }
}