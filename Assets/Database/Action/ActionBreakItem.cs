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
            //イベントが発生しない
            return false;
        }

        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;

        
        if (args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipHand
            || args.targetItemName == SaveDataManager.saveData.charaInfo.itemNameEquipBody)
        {
            if (count >= 1)
            {
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！新しいものに取り換えた(残り" + count + "個)",Color.red);
            }
            else
            {
                //装備品が0個になってしまったときの処理
                ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！替えがないので素手になった", Color.red);
                Action actionEquipWeapon = new ActionEquipWeapon();
                ActionArgs actionArgs = new ActionArgs();
                actionArgs.targetItemName = ItemName.fist;
                actionEquipWeapon.ExecuteAction(actionArgs);
            }
        }
        else
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "は壊れた！", Color.red);
        }

        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, -1);

        SaveDataManager.Save();
        return false;


        

    }
}
