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
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"+SaveDataManager.saveData.charaInfo.name + "は酒を飲んだ。");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name+":酒うめ～～！！");
        }
        else if(yoi<20)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "は酒を飲んだ。");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":ちょっと酔っぱらってきちゃった……///");
        }
        else if (yoi < 20)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "は酒を飲む手が止まらない！");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":酒最高！酒最高！酒最高！！！！！");
        }
        else if (yoi < 30)
        {
            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + SaveDataManager.saveData.charaInfo.name + "はゲロを吐いた。");
            ChatMenuManager.Instance.SendTextRPCInSameMap(SaveDataManager.saveData.charaInfo.name + ":ゲエエエエエエエエ！！！！！！！！");
            yoi = 20;
        }


        return true;
    }
    
}
