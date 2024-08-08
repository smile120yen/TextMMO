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
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "Ç™ë´ÇËÇ‹ÇπÇÒ(åªç›"
            + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + "å¬)");
            return false;
        }

        return true;
    }
}
