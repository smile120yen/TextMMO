using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class OtherPlayerCharaInfoManager : SingletonMonoBehaviourPunCallbacks<OtherPlayerCharaInfoManager>
{
    public Dictionary<string, SaveData> playerInfo;
    public UnityEvent OnPlayerInfoUpdate = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = new Dictionary<string, SaveData>();
    }

    public override void OnJoinedRoom()
    {
        ReceveCharaInfoRPC(PhotonNetwork.LocalPlayer.UserId, SaveDataManager.GetJson());
        SaveDataManager.OnSaveDataUpdateBefore.AddListener(
            () =>
            {
                //これ、他の情報更新系よりも先に届かないと表示に間に合わない
                ReceveCharaInfoRPC(PhotonNetwork.LocalPlayer.UserId, SaveDataManager.GetJson());
            }
        );
    }

    public SaveData GetPlayerSaveData(string userId)
    {
        SaveData saveData;
        playerInfo.TryGetValue(userId, out saveData);

        if (saveData != null)
        {
            return saveData;
        }
        else
        {
            return null;
        }
    }

    public void RequestOtherPlayerCharaInfo()
    {
        //他のプレイヤーに共有してほしいとリクエストする
        photonView.RPC(nameof(SendMyCharaInfo), RpcTarget.Others);
    }

    [PunRPC]
    public void SendMyCharaInfo()
    {
        //自身のキャラクターのセーブデータを送る
        photonView.RPC(nameof(ReceveCharaInfoRPC), RpcTarget.All, new object[] { PhotonNetwork.LocalPlayer.UserId, SaveDataManager.GetJson() });

    }

    [PunRPC]
    public void ReceveCharaInfoRPC(string userId, string info)
    {
        //セーブデータを受け取ってリストに保存する
        SaveData saveData = JsonUtility.FromJson<SaveData>(info);
        Debug.Log("Receve chara info data:" + userId + "," + saveData.charaInfo.name.ToString());
        if (playerInfo.ContainsKey(userId))
        {
            playerInfo[userId] = saveData;
        }
        else
        {
            playerInfo.Add(userId, saveData);
        }
        OnPlayerInfoUpdate.Invoke();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //ゲームから退出したプレイヤーがいたらそのプレイヤーはリストから消す
        playerInfo.Remove(otherPlayer.UserId);
        OnPlayerInfoUpdate.Invoke();
    }
}
