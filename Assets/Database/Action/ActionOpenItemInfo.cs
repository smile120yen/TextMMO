using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOpenItemInfo : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenItemInfo");

        ItemDetailInfoManager.Instance.OpenItemDetailInfo(args.targetItemName);

        return true;
    }
}
