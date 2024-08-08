using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionTradeItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("TradeItems");


        bool isSuccess = true;
        foreach (ItemInfo itemInfo in args.consumeItemInfo)
        {
            if(SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount < itemInfo.mount)
            {
                ChatMenuManager.Instance.AddText(">" + itemInfo.itemData.name + "������܂���(����"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "��)");
                isSuccess = false;
            }
        }

        if (!isSuccess)
        {
            return false;
        }

        foreach (ItemInfo itemInfo in args.consumeItemInfo)
        {
            SaveDataManager.saveData.charaInfo.AddItemToInventory(itemInfo.itemName, -itemInfo.mount);
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "��" + itemInfo.itemData.name + "��"
                + Mathf.Abs(itemInfo.mount) + "�������i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "��)");
        }

        foreach (ItemInfo itemInfo in args.getItemInfo)
        {
            SaveDataManager.saveData.charaInfo.AddItemToInventory(itemInfo.itemName, itemInfo.mount);
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "��" + itemInfo.itemData.name + "��"
                + itemInfo.mount + "�Q�b�g�����i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "��)");
        }

        SaveDataManager.Save();

        return true;
    }
}
