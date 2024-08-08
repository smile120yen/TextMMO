using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SampleChat : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform parent;
    [SerializeField] GameObject textPrefab;

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        //Debug.Log("TRY CONNECT ROOM");
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("CONNECTING ROOM");
        photonView.RPC(nameof(AddText), RpcTarget.All, PhotonNetwork.NickName+"がゲームに参加しました");
    }

    [PunRPC]
    public void AddText(string text)
    {
        Debug.Log("ADD TEXT");
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextMeshProUGUI>().text = text;
    }
}
