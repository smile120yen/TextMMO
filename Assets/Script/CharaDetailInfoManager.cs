using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Realtime;
using Photon.Pun;

public class CharaDetailInfoManager : SingletonMonoBehaviour<CharaDetailInfoManager>
{

    [SerializeField] GameObject CharaDetailInfo;
    [SerializeField] Transform parent;
    [SerializeField] GameObject textPrefab;
    [SerializeField] ScrollRect scrollRect;
    string userId = null;

    private void Start()
    {
        SaveDataManager.OnSaveDataUpdate.AddListener(()=> CharaDetailInfoUpdate());
    }

    public void CharaDetailInfoUpdate()
    {
        if (userId == null) return;
        SaveData saveData = OtherPlayerCharaInfoManager.Instance.GetPlayerSaveData(userId);
        CharaInfo charaInfo = saveData.charaInfo;
        //CharaInfo charaInfo = OtherPlayerCharaInfoManager.Instance.GetPlayerCharaInfo(userId);
        if (charaInfo == null) return;

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        ActionInfo closeCharaInfo = new ActionInfo();
        closeCharaInfo.text = "[ 閉じる ]";
        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionCloseCharaInfo") as ActionCloseCharaInfo;
        closeCharaInfo.actionDatas.Add(actionData);

        AddText(closeCharaInfo);

        AddText("名前:" + charaInfo.name);
        AddText(charaInfo.GetCurrentJobInfo().jobData.name +" Lv:" + charaInfo.GetCurrentJobInfo().lv +" ( "+ "次のLvまで:" + (charaInfo.GetCurrentJobInfo().nextMaxExp - charaInfo.GetCurrentJobInfo().exp)+" )");

        AddText("HP:" + charaInfo.hp + "/" + charaInfo.maxHp);
        AddText("MP:" + charaInfo.mp + "/" + charaInfo.maxMp);
        AddText("スタミナ:" + charaInfo.currentStamina + "/" + charaInfo.maxStamina);
        AddText("所持金:" + charaInfo.GetItemData(ItemName.money).mount + "G");
        AddText("現在のリスポーン地点:" + charaInfo.respawnPoint.mapData.name);

        ItemInfo weapon = charaInfo.GetItemData(charaInfo.itemNameEquipHand);
        ActionInfo weaponInfoAction = new ActionInfo();
        if (weapon.itemData.maxDurability <= 0)
        {
            weaponInfoAction.text = "武器:" + weapon.itemData.name;
        }
        else
        {
            weaponInfoAction.text = "武器:" + weapon.itemData.name + "<br>( 残り回数:" + (weapon.itemData.maxDurability - weapon.useCount) + " / " + weapon.itemData.maxDurability + " )";
        }
        ActionData weaponInfoactionData = new ActionData();
        weaponInfoactionData.args.targetItemName = weapon.itemName;
        weaponInfoactionData.action = ScriptableObject.CreateInstance("ActionOpenItemInfo") as ActionOpenItemInfo;
        weaponInfoAction.actionDatas.Add(weaponInfoactionData);

        AddText(weaponInfoAction);

        ItemInfo armor = charaInfo.GetItemData(charaInfo.itemNameEquipBody);
        ActionInfo armorInfoAction = new ActionInfo();
        if (armor.itemData.maxDurability <= 0)
        {
            armorInfoAction.text = "防具:" + armor.itemData.name;
        }
        else
        {
            armorInfoAction.text = "防具:" + armor.itemData.name + "<br>( 残り回数:" + (armor.itemData.maxDurability - armor.useCount) + " / " + armor.itemData.maxDurability + " )";
        }
        ActionData armorInfoactionData = new ActionData();
        armorInfoactionData.args.targetItemName = armor.itemName;
        armorInfoactionData.action = ScriptableObject.CreateInstance("ActionOpenItemInfo") as ActionOpenItemInfo;
        armorInfoAction.actionDatas.Add(armorInfoactionData);

        AddText(armorInfoAction);

        if (PhotonNetwork.LocalPlayer.UserId != userId)
        {

            AddText("　　　");
            AddText("現在いるマップ:" + saveData.mapInfo.mapData.name);

            ActionInfo warpActionInfo = new ActionInfo();
            warpActionInfo.text = "[ このプレイヤーの場所にワープする ]";
            ActionData warpActionData = new ActionData();
            warpActionData.action = ScriptableObject.CreateInstance("ActionMapMove") as ActionMapMove;
            warpActionData.args.targetMapName = saveData.mapInfo.mapName;
            warpActionInfo.actionDatas.Add(warpActionData);
            AddText(warpActionInfo);
        }


        AddText("　　　");

        AddText("所持アイテム:");

        bool isExistVisibleItem = false;

        foreach (ItemInfo itemInfo in charaInfo.inventory)
        {
            if (itemInfo.itemName == ItemName.money) continue;
            if (itemInfo.mount <= 0) continue;

            ActionInfo ItemInfoAction = new ActionInfo();
            ItemInfoAction.text = itemInfo.itemData.name + ":" + itemInfo.mount;
            ActionData ItemInfoactionData = new ActionData();
            ItemInfoactionData.args.targetItemName = itemInfo.itemName;
            ItemInfoactionData.action = ScriptableObject.CreateInstance("ActionOpenItemInfo") as ActionOpenItemInfo;
            ItemInfoAction.actionDatas.Add(ItemInfoactionData);

            AddText(ItemInfoAction);
            isExistVisibleItem = true;
        }

        if (!isExistVisibleItem)
        {
            AddText("なし");
        }

        AddText("　　　");
        AddText("==実績項目==");
        TimeSpan time = DateTime.Now - charaInfo.charaCreateTime.Value;
        string timeText = "いままで遊んだ時間:";
        if (time.TotalHours >= 1) timeText += (int)time.TotalHours + "時間";
        if (time.Minutes >= 1) timeText += time.Minutes + "分";
        timeText += time.Seconds + "秒";
        AddText(timeText);

        //虚無をカウントしないので-1。本当は入手不可なものをフラグで消すべき
        int itemTypeCount = charaInfo.inventory.Count;
        int maxItemTypeCount = DatabaseManager.Instance.itemDatabase.itemDatas.Count-1;
        AddText("アイテム発見率: " + itemTypeCount + "/" + maxItemTypeCount);
    }


    public void OpenCharaDetailInfo(string userId)
    {
        if (userId == null) return;
        ItemDetailInfoManager.Instance.CloseItemDetailInfo();
        CharaInfo charaInfo = OtherPlayerCharaInfoManager.Instance.GetPlayerSaveData(userId).charaInfo;
        this.userId = userId;
        CharaDetailInfoUpdate();
        CharaDetailInfo.SetActive(true);
    }

    public void CloseCharaDetailInfo()
    {
        CharaDetailInfo.SetActive(false);
    }

    public void ScrollTop()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
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
}
