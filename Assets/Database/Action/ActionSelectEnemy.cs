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
        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "��" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"���ɂ��ł���");
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
