using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ActionDrink : Action
{
    DateTime lastExecuteTime;
    int yoi;

    public override bool ExecuteAction(ActionArgs args)
    {

        if (lastExecuteTime != null)
        {
            int executeTime = (int)(DateTime.Now - lastExecuteTime).TotalSeconds;
            yoi -= executeTime;
            if (yoi < 0) yoi = 0;
        }
        lastExecuteTime = DateTime.Now;
        yoi += 2;

        Debug.Log("Action Drink:" + yoi);

        if (yoi < 10)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+SaveDataManager.saveData.charaInfo.name + "�͎������񂾁B");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name+":�����߁`�`�I�I");
        }
        else if(yoi<20)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "�͎������񂾁B");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":������Ɛ����ς���Ă���������c�c///");
        }
        else if (yoi < 20)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "�͎������ގ肪�~�܂�Ȃ��I");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":���ō��I���ō��I���ō��I�I�I�I�I");
        }
        else if (yoi < 30)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "�̓Q����f�����B");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":�Q�G�G�G�G�G�G�G�G�I�I�I�I�I�I�I�I");
            yoi = 20;
        }


        return true;
    }
    
}
