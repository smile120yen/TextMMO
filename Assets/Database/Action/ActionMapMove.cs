using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;

public class ActionMapMove : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        if (!args.forceExecute)
        {
            //1匹でもHP0以上の敵が居たら実行できない
            foreach (EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
            {
                if (enemyInfo.currentHp > 0)
                {
                    ChatMenuManager.Instance.AddText(">敵がいるときは他のマップに移動できない");
                    return false;
                }
            }
        }

        Debug.Log("MoveMap:"+args.targetMapName);


        //送信グループを変える前にメッセージを前のグループに残す
        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "は" + DatabaseManager.Instance.mapDatabase.mapDatas.Find(s => s.uniqueName == args.targetMapName.ToString()).name + "に移動した");

        //送信グループを変える前に前のグループに最後のキャラ情報を送信する
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        SaveDataManager.saveData.mapInfo.mapName = args.targetMapName;

        //マップの追加情報をクリア
        MapInfoWindowManager.Instance.additionalActions.Clear();
                
        //受信グループを変える
        PhotonNetwork.SetInterestGroups(Array.Empty<byte>(), new byte[] { (byte)args.targetMapName });

        //敵情報を新しくスポーンさせる
        EnemyInfoManager.Instance.InitialEnemyInfoList();

        //同じグループの他プレイヤーのキャラ情報がもらえるようにリクエストする
        OtherPlayerCharaInfoManager.Instance.RequestOtherPlayerCharaInfo();

        //アイテム詳細は閉じる
        ItemDetailInfoManager.Instance.CloseItemDetailInfo();

        //キャラ詳細も閉じる
        CharaDetailInfoManager.Instance.CloseCharaDetailInfo();

        SaveDataManager.Save();
        return true;
    }
}
