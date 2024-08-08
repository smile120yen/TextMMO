using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionBreakItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionItemBreakCheck:" + args.targetItemName.ToString());

        

        float random = Random.Range(0, 1.0f);

        if(random > args.eventProbability)
        {
            //�C�x���g���������Ȃ�
            return false;
        }

        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;

        
        if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand
            || args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
        {
            if (count >= 1)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I�V�������̂Ɏ�芷����(�c��" + count + "��)",Color.red);
            }
            else
            {
                //�����i��0�ɂȂ��Ă��܂����Ƃ��̏���
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I�ւ����Ȃ��̂őf��ɂȂ���", Color.red);
                Action actionEquipWeapon = new ActionEquipWeapon();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.fist;
                actionEquipWeapon.ExecuteAction(actionArgs);
            }
        }
        else
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�͉�ꂽ�I", Color.red);
        }

        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, -1);

        SaveDataManager.Save();
        return false;


        

    }
}
