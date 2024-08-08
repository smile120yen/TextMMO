using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ActionAttackBuff : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("AttackBuff");

        float random = UnityEngine.Random.Range(0, 1.0f);

        if(random > args.eventProbability) {
            return true;
        }

        SaveDataManager.saveData.charaInfo.attackBuffs.RemoveAll(s => s.attackBuff.uniqueKey == args.attackBuff.uniqueKey);
        SaveDataManager.saveData.charaInfo.attackBuffs.Add(new AttackBuffInfo(args.attackBuffDuration,args.attackBuff));

        ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">"
            + PhotonNetwork.LocalPlayer.NickName + "‚Í"
            + args.attackBuff.name + "‚ðŽó‚¯‚½(" + args.attackBuffDuration + "•b)");
        
        SaveDataManager.Save();
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
        EnemyInfoManager.Instance.SendMyEnemyInfo();
       
        return true;
    }



    
}
