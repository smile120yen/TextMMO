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

        string tani = "��";
        if(args.targetItemName == ItemName.money)
        {
            tani = "G";
        }
                


        if (!isSuccess)
        {
            ChatMenuManager.Instance.AddText(">" + args.targetItemData.name + "������܂���(����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
            return false;
        }

        if (args.mount > 0)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+PhotonNetwork.LocalPlayer.NickName+"��" + args.targetItemData.name + "��"
                + args.mount + tani + "�Q�b�g�����i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
        }
        else if (args.mount < 0)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName +"��"+ args.targetItemData.name + "��"
                + Mathf.Abs(args.mount) + tani + "�������i����"
                + SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount + tani + ")");
        }
        SaveDataManager.Save();

        return true;
    }
}
