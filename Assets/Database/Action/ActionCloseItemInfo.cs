using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCloseItemInfo : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("CloseItemInfo");

        ItemDetailInfoManager.Instance.CloseItemDetailInfo();
        CharaDetailInfoManager.Instance.ScrollTop();

        return true;
    }
}
