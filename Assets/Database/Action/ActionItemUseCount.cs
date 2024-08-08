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

        //�܂��ϋv�͂�����
        if (targetItem.useCount < targetItem.itemData.maxDurability) return true;

        //�ϋv�͂��Ȃ��Ȃ���
        targetItem.useCount = 0;
        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;
        
        if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand
            || args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
        {
            if (count >= 1)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I�V�������̂Ɏ�芷����(�c��" + count + "��)",Color.red);
            }
            else if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand)
            {
                //�����i��0�ɂȂ��Ă��܂����Ƃ��̏���
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I�ւ����Ȃ��̂őf��ɂȂ���", Color.red);
                Action actionEquipWeapon = new ActionEquipWeapon();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.fist;
                actionEquipWeapon.ExecuteAction(actionArgs);
            }
            else if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
            {
                //�����i��0�ɂȂ��Ă��܂����Ƃ��̏���
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I�ւ����Ȃ��̂őf���ɂȂ���", Color.red);
                Action actionEquipArmor = new ActionEquipArmor();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.skin;
                actionEquipArmor.ExecuteAction(actionArgs);
            }
        }
        else
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I", Color.red);
        }

        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, -1);

        SaveDataManager.Save();
        return true;
    }
}
