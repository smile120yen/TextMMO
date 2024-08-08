using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionItemAdjustment : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("AdjustmentItem:" + args.targetItemName.ToString());
        bool isSuccess = SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, args.mount);

        string tani = "個";
        if(args.targetItemName == ItemName.money)
        {
            tani = "G";
        }
                


        if (!isSuccess)
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "が足りません(現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
            return false;
        }

        if (args.mount > 0)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+PhotonNetwork.LocalPlayer.NickName+"は" + args.targetItemData.name + "を"
                + args.mount + tani + "ゲットした（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
        }
        else if (args.mount < 0)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName +"は"+ args.targetItemData.name + "を"
                + Mathf.Abs(args.mount) + tani + "失った（現在"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
        }
        SaveDataManager.Save();

        return true;
    }
}
