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

            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+PhotonNetwork.LocalPlayer.NickName+"��" + args.targetItemData.name + "��"
                + mount + "�Q�b�g�����i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "��)");
        }
        else if (args.mount < 0)
        {
            if (SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount >= args.mount)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "������܂���(����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "��)");
                return false;
            }

            SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, args.mount);

            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName +"��"+ args.targetItemData.name + "��"
                + Mathf.Abs(args.mount) + "�������i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "��)");
        }
        SaveDataManager.Save();

        return true;
    }
}
