using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChangeJob : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("Job Change:"+args.mount);

        SaveDataManager.saveData.charaInfo.currentJob = args.targetJob;

        JobInfo jobInfo = SaveDataManager.saveData.charaInfo.GetCurrentJobInfo();

        /*
        SaveDataManager.saveData.charaInfo.lv = jobInfo.lv;
        SaveDataManager.saveData.charaInfo.maxHp = jobInfo.maxHp;
        SaveDataManager.saveData.charaInfo.maxMp = jobInfo.maxMp;
        SaveDataManager.saveData.charaInfo.nextMaxExp = jobInfo.nextMaxExp;
        SaveDataManager.saveData.charaInfo.exp = jobInfo.exp;
        */

        SaveDataManager.saveData.charaInfo.hp = SaveDataManager.saveData.charaInfo.maxHp;
        SaveDataManager.saveData.charaInfo.mp = SaveDataManager.saveData.charaInfo.maxMp;

        ChatMenuManager.Instance.AddText(">"+SaveDataManager.saveData.charaInfo.name+ "��"
            + jobInfo.jobData.name+"�ɓ]�E�����I");
        ChatMenuManager.Instance.AddText(">" + SaveDataManager.saveData.charaInfo.name + "�̃��x����"
            + SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().lv + "�ɂȂ���");

        SaveDataManager.Save();

        return true;
    }
}
