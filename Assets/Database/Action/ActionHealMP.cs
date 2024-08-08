using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionHealMP : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("Heal MP:"+args.mount);

        int oldMP = SaveDataManager.saveData.charaInfo.mp;

        SaveDataManager.saveData.charaInfo.mp += args.mount;
        if (SaveDataManager.saveData.charaInfo.mp > SaveDataManager.saveData.charaInfo.maxMp)
            SaveDataManager.saveData.charaInfo.mp = SaveDataManager.saveData.charaInfo.maxMp;

        int newMp = SaveDataManager.saveData.charaInfo.mp;
        int diff = newMp - oldMP;

        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName 
            + "��MP��" + diff + "�񕜂���");

        SaveDataManager.Save();
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        return true;
    }
}
