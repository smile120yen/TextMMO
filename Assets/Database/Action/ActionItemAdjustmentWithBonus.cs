using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionItemAdjustmentWithBonus : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionItemAdjustmentWithBonus:" + args.targetItemName.ToString());

        if (args.mount > 0)
        {
            int bonus = 0;
            foreach (ItemBonus itemBonus in SaveDataManager.saveData.charaInfo.itemDataEquipHand.itemBonuses)
            {
                if(itemBonus.itemName == args.targetItemName)
                {
                    float random = Random.Range(0, 1);
                    if (random <= itemBonus.chance)
                    {
                        bonus = Random.Range(itemBonus.bonusMin, itemBonus.bonusMax + 1);
                    }

                    break;
                }
            }

            foreach (ActionData bonusAction in SaveDataManager.saveData.charaInfo.itemDataEquipHand.equipItemBonusActions)
            {
                bonusAction.action.ExecuteAction(bonusAction.args);
            }

            int mount = args.mount + bonus;

            SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, mount);

            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+PhotonNetwork.LocalPlayer.NickName+"は" + args.targetItemData.name + "を"
                + mount + "個ゲットした（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "個)");
        }
        else if (args.mount < 0)
        {
            if (SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount >= args.mount)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "が足りません(現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "個)");
                return false;
            }

            SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, args.mount);

            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName +"は"+ args.targetItemData.name + "を"
                + Mathf.Abs(args.mount) + "個失った（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "個)");
        }
        SaveDataManager.Save();

        return true;
    }
}
