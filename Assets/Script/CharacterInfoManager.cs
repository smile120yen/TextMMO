using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class CharacterInfoManager : SingletonMonoBehaviourPunCallbacks<CharacterInfoManager>
{
    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform parent;
    [SerializeField] float staminaHealTime=0;
    [SerializeField] int staminaHealMount = 2;
    public bool isRespawning = false;

    private void Start()
    {
        UpdateCharaInfo();
        SaveDataManager.OnSaveDataUpdate.AddListener(() => UpdateCharaInfo());
    }

    void Update()
    {
        staminaHealTime += Time.deltaTime;
        if (staminaHealTime > 1)
        {
            //秒間でアップデートする処理はすべてここ

            SaveDataManager.saveData.charaInfo.currentStamina += staminaHealMount;
            if (SaveDataManager.saveData.charaInfo.maxStamina < SaveDataManager.saveData.charaInfo.currentStamina)
            {
                SaveDataManager.saveData.charaInfo.currentStamina = SaveDataManager.saveData.charaInfo.maxStamina;
            }
            staminaHealTime -= 1;

            List<AttackBuffInfo> newAttackBuffList = new List<AttackBuffInfo>();
            foreach(AttackBuffInfo attackBuff in SaveDataManager.saveData.charaInfo.attackBuffs)
            {
                if (attackBuff.end.Value > DateTime.Now)
                {
                    newAttackBuffList.Add(attackBuff);
                }
                else
                {
                    ChatMenuManager.Instance.AddText(">"+ attackBuff.attackBuff.name + "の効果が切れた",Color.gray);
                }
            }

            SaveDataManager.saveData.charaInfo.attackBuffs = newAttackBuffList;



            UpdateCharaInfo();

        }

    }
    /*
    private void UpdateCharaInfoWithSync()
    {
        UpdateCharaInfo();

        photonView.Group = (byte)SaveDataManager.saveData.mapInfo.currentMap;
        photonView.RPC(nameof(SyncCharaInfo), RpcTarget.Others, new object[] { PhotonNetwork.LocalPlayer.UserId, SaveDataManager.GetJson() });
    }
    */

    private void UpdateCharaInfo()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        CharaInfo charaInfo = SaveDataManager.saveData.charaInfo;

        ActionInfo openCharaInfo = new ActionInfo();
        openCharaInfo.text = charaInfo.name + " Lv:" + charaInfo.GetCurrentJobInfo().lv;
        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
        actionData.args = new ActionArgs();
        actionData.args.userId = PhotonNetwork.LocalPlayer.UserId;
        openCharaInfo.actionDatas.Add(actionData);

        AddText(openCharaInfo);

        AddText("HP:" + charaInfo.hp + "/" + SaveDataManager.saveData.charaInfo.maxHp);
        AddText("MP:" + charaInfo.mp + "/" + SaveDataManager.saveData.charaInfo.maxMp);
        AddText("スタミナ:" + charaInfo.currentStamina + "/"+ charaInfo.maxStamina);
        AddText("所持金:" + charaInfo.GetItemData(ItemName.money).mount + "G");

        foreach (AttackBuffInfo attackBuff in charaInfo.attackBuffs)
        {
            TimeSpan time = attackBuff.end.Value - DateTime.Now;
            AddText(attackBuff.attackBuff.name + "(" + time.Minutes.ToString("00") + ":" + Mathf.Round(time.Seconds+1).ToString("00") + ")");
        }

        /*
        foreach (ItemInfo itemInfo in charaInfo.inventory)
        {
            if (itemInfo.itemName == ItemName.money) continue;
            AddText(itemInfo.itemData.name+":" + itemInfo.mount);
        }
        */
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
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        bool isContinue = actionData.action.ExecuteAction(actionData.args);
                        if (!isContinue) break;
                    }
                });
    }



    public void CharacterDeathEvent()
    {
        ChatMenuManager.Instance.SendTextRPCInSameMapDanger(">"
            + PhotonNetwork.LocalPlayer.NickName + "はちからつきた");
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        ChatMenuManager.Instance.AddText(">リスポーンします 3秒前……");
        yield return new WaitForSeconds(1);


        ChatMenuManager.Instance.AddText(">リスポーンします 2秒前……");
        yield return new WaitForSeconds(1);


        ChatMenuManager.Instance.AddText(">リスポーンします 1秒前……");
        yield return new WaitForSeconds(1);

        CharaInfo charaInfo = SaveDataManager.saveData.charaInfo;


        ActionMapMove actionMapMove = new ActionMapMove();
        ActionArgs actionArgs = new ActionArgs();
        actionArgs.targetMapName = SaveDataManager.saveData.charaInfo.respawnPoint.mapName;
        actionArgs.forceExecute = true;
        actionMapMove.ExecuteAction(actionArgs);

        ActionHealHP actionHeal = new ActionHealHP();
        ActionArgs actionArgsHeal = new ActionArgs();
        actionArgsHeal.mount = 9999;
        actionHeal.ExecuteAction(actionArgsHeal);


        if (SaveDataManager.saveData.charaInfo.mp < SaveDataManager.saveData.charaInfo.maxMp)
        {
            ActionHealMP actionHealMP = new ActionHealMP();
            ActionArgs actionArgsHealMP = new ActionArgs();
            actionArgsHealMP.mount = 9999;
            actionHealMP.ExecuteAction(actionArgsHealMP);
        }
    }
}
