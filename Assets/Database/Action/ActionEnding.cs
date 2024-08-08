using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionEnding : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionEnding:"+args.key);

        if (!SaveDataManager.saveData.charaInfo.endingflag.Contains(args.key))
        {
            SaveDataManager.saveData.charaInfo.endingflag.Add(args.key);
            EndingWindowManager.Instance.OpenEndingWindow(args.key);
        }

        return true;
    }
}
