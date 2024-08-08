using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;

public class ActionSetRespawnPoint : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        SaveDataManager.saveData.charaInfo.respawnPoint.mapName = args.targetMapName;
        ChatMenuManager.Instance.AddText(">"+ PhotonNetwork.LocalPlayer.NickName + "は"
            + SaveDataManager.saveData.charaInfo.respawnPoint.mapData.name+"をリスポーン地点に設定した");

        SaveDataManager.Save();
        return true;
    }
}
