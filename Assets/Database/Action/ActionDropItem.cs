using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionDropItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionDropItem:" + args.targetItemName.ToString());

        

        float random = Random.Range(0, 1.0f);

        if(random > args.eventProbability)
        {
            //イベントが発生しない
            return false;
        }

        ItemData itemData = args.targetItemData;
        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, 1);

        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;

        ChatMenuManager.Instance.AddText(">ラッキー！" + args.targetItemData.name + "を手に入れた！(現在"+ count+ "個)",Color.yellow);

        SaveDataManager.Save();

        return true;
    }
}
