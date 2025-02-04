using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionCheckItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("CheckItem:" + args.targetItemName.ToString());

        if(SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount < args.mount)
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "が足りません(現在"
            + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "個)");
            return false;
        }

        return true;
    }
}
