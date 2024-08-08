using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionConsumeMP : Action
{
    public override bool CheckCanExecute(ActionArgs args)
    {
        if (SaveDataManager.saveData.charaInfo.mp < args.mount)
        {
            ChatMenuManager.Instance.AddText(">MPÇ™ë´ÇËÇ»Ç¢ (è¡îÔMP:"+args.mount+")");
            return false;
        }

        return true;

    }

    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionConsumeMP:" + args.mount);

        SaveDataManager.saveData.charaInfo.mp -= args.mount;
        SaveDataManager.Save();
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        return true;
    }
}
