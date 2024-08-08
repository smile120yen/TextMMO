using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionHealHP : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("Heal HP:"+args.mount);

        int oldHP = SaveDataManager.saveData.charaInfo.hp;

        SaveDataManager.saveData.charaInfo.hp += args.mount;
        if (SaveDataManager.saveData.charaInfo.hp > SaveDataManager.saveData.charaInfo.maxHp)
            SaveDataManager.saveData.charaInfo.hp = SaveDataManager.saveData.charaInfo.maxHp;

        int newHP = SaveDataManager.saveData.charaInfo.hp;
        int diff = newHP - oldHP;

        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName 
            + "‚ÍHP‚ð" + diff + "‰ñ•œ‚µ‚½");

        SaveDataManager.Save();
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        return true;
    }
}
