using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCloseCharaInfo : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("CloseCharaInfo");

        CharaDetailInfoManager.Instance.CloseCharaDetailInfo();

        return true;
    }
}
