using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;

public class ActionCheckEnemy : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        //1•C‚Å‚àHP0ˆÈã‚Ì“G‚ª‹‚½‚çÀs‚Å‚«‚È‚¢
        foreach(EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
        {
            if (enemyInfo.currentHp > 0)
            {
                ChatMenuManager.Instance.AddText(">“G‚ª‚¢‚é‚Æ‚«‚ÍÀs‚Å‚«‚È‚¢");
                return false;
            }
        }

        return true;
    }
}
