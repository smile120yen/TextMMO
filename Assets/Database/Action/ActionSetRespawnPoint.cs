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
        ChatMenuManager.Instance.AddText(">"+ PhotonNetwork.LocalPlayer.NickName + "��"
            + SaveDataManager.saveData.charaInfo.respawnPoint.mapData.name+"�����X�|�[���n�_�ɐݒ肵��");

        SaveDataManager.Save();
        return true;
    }
}
