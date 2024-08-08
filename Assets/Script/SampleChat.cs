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
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        //Debug.Log("TRY CONNECT ROOM");
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        Debug.Log("CONNECTING ROOM");
        photonView.RPC(nameof(AddText), RpcTarget.All, PhotonNetwork.NickName+"���Q�[���ɎQ�����܂���");
    }

    [PunRPC]
    public void AddText(string text)
    {
        Debug.Log("ADD TEXT");
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextMeshProUGUI>().text = text;
    }
}
