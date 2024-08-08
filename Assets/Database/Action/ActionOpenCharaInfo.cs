using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOpenCharaInfo : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenCharaInfo");
        
        CharaDetailInfoManager.Instance.OpenCharaDetailInfo(args.userId);

        return true;
    }
}
