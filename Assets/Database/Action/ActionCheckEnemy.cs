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
        //1匹でもHP0以上の敵が居たら実行できない
        foreach(EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
        {
            if (enemyInfo.currentHp > 0)
            {
                ChatMenuManager.Instance.AddText(">敵がいるときは実行できない");
                return false;
            }
        }

        return true;
    }
}
