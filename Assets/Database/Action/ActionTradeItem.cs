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
                ChatMenuManager.Instance.AddText(">" + itemInfo.itemData.name + "が足りません(現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "個)");
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
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "は" + itemInfo.itemData.name + "を"
                + Mathf.Abs(itemInfo.mount) + "個失った（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "個)");
        }

        foreach (ItemInfo itemInfo in args.getItemInfo)
        {
            SaveDataManager.saveData.charaInfo.AddItemToInventory(itemInfo.itemName, itemInfo.mount);
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "は" + itemInfo.itemData.name + "を"
                + itemInfo.mount + "個ゲットした（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(itemInfo.itemName).mount + "個)");
        }

        SaveDataManager.Save();

        return true;
    }
}
