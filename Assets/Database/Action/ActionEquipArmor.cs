using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEquipArmor : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("Equip Item:" + args.targetItemData.name);

        if (SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount <= 0)
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "��1�������Ă��Ȃ�");
            return false;
        }


        ItemName currentEquipItem = SaveDataManager.saveData.charaInfo.itemNameEquipBody;
        if (currentEquipItem != ItemName.None)
        {
            string currentItemName = SaveDataManager.saveData.charaInfo.itemDataEquipBody.name;
            ChatMenuManager.Instance.AddText(">"+currentItemName+"���O����" + args.targetItemData.name + "�𑕔�����");
            SaveDataManager.saveData.charaInfo.GetItemData(currentEquipItem).mount++;
        }
        else
        {

            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "�𑕔�����");
        }

        SaveDataManager.saveData.charaInfo.itemNameEquipBody = args.targetItemName;

        if (SaveDataManager.saveData.charaInfo.hp > SaveDataManager.saveData.charaInfo.maxHp)
            SaveDataManager.saveData.charaInfo.hp = SaveDataManager.saveData.charaInfo.maxHp;

        if (SaveDataManager.saveData.charaInfo.mp > SaveDataManager.saveData.charaInfo.maxMp)
            SaveDataManager.saveData.charaInfo.mp = SaveDataManager.saveData.charaInfo.maxMp;

        SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount--;


        SaveDataManager.Save();

        return true;
    }
}
