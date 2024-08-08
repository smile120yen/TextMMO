using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ActionSelectEnemy : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        EnemyInfoManager.Instance.targetEnemyId = args.targetID;
        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "‚Í" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"‚ð‚É‚ç‚ñ‚Å‚¢‚é");
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
