using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionItemUseCount : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionItemUseCount:" + args.targetItemName.ToString());

        ItemInfo targetItem = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName);
        targetItem.useCount++;

        //まだ耐久力がある
        if (targetItem.useCount < targetItem.itemData.maxDurability) return true;

        //耐久力がなくなった
        targetItem.useCount = 0;
        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;
        
        if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand
            || args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
        {
            if (count >= 1)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！新しいものに取り換えた(残り" + count + "個)",Color.red);
            }
            else if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand)
            {
                //装備品が0個になってしまったときの処理
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！替えがないので素手になった", Color.red);
                Action actionEquipWeapon = new ActionEquipWeapon();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.fist;
                actionEquipWeapon.ExecuteAction(actionArgs);
            }
            else if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
            {
                //装備品が0個になってしまったときの処理
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！替えがないので素肌になった", Color.red);
                Action actionEquipArmor = new ActionEquipArmor();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.skin;
                actionEquipArmor.ExecuteAction(actionArgs);
            }
        }
        else
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！", Color.red);
        }

        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, -1);

        SaveDataManager.Save();
        return true;
    }
}
