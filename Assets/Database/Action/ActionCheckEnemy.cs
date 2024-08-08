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
        //1�C�ł�HP0�ȏ�̓G����������s�ł��Ȃ�
        foreach(EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
        {
            if (enemyInfo.currentHp > 0)
            {
                ChatMenuManager.Instance.AddText(">�G������Ƃ��͎��s�ł��Ȃ�");
                return false;
            }
        }

        return true;
    }
}
