using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ActionEmote : Action
{
    DateTime lastExecuteTime;
    int drunkenness = 0;
    public override bool ExecuteAction(ActionArgs args)
    {
        ChatMenuManager.Instance.SendTextRPCInSameMap(">"+PhotonNetwork.NickName + args.emoteMessage);
        return true;
    }
    
}
