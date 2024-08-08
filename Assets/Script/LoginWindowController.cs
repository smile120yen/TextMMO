using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System;

public class LoginWindowController : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject loginWindow;
    [SerializeField] Transform parent;
    [SerializeField] GameObject textPrefab;
    [SerializeField] bool isDebug;
    [SerializeField] SaveData debugSaveData;
    string inputText;
    bool isConnectMaster;

    public void Start()
    {
        loginWindow.SetActive(true);
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(LoginCoroutine());
    }
    public override void OnConnectedToMaster()
    {
        isConnectMaster = true;
    }

    public void OnInputFieldSend(string str)
    {
        inputText = str;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log(cause.ToString());
        AddText(">" + cause.ToString());
    }

    public IEnumerator LoginCoroutine()
    {

        AddText(">�T�[�o�[�ɐڑ����c�c");



        while (true)
        {
            if (isConnectMaster) break;
            yield return null;
        }

        if (isDebug)
        {
            SaveDataManager.saveData = debugSaveData;
            SaveDataManager.Save();
        }

        AddText(">�T�[�o�[�ɐڑ����܂���");

        if (SaveDataManager.saveData.charaInfo.name == null)
        {
            AddText(">���Ȃ��̖��O����͂��Ă�������");

            //�e�L�X�g���͑҂����킹����

            while (true)
            {
                if (inputText != null) break;
                yield return null;
            }

            SaveDataManager.saveData.charaInfo.name = inputText;
            PhotonNetwork.NickName = inputText;
            SaveDataManager.Save();
            AddText(inputText);
            inputText = null;
        }
        else
        {
            PhotonNetwork.NickName = SaveDataManager.saveData.charaInfo.name;
        }       


        yield return new WaitForSeconds(0.5f);

        AddText(">���[���ɓ������Ă��܂��c�c");

        RoomOptions options = new RoomOptions();
        options.PublishUserId = true;
        options.MaxPlayers = 100;

        PhotonNetwork.JoinOrCreateRoom("Room", options, TypedLobby.Default);

        while (true)
        {
            if (PhotonNetwork.InRoom) break;
            yield return null;
        }

        AddText(">�����ɐ������܂���");

        //�l�b�g�[���[�N�J�����O�ݒ�
        PhotonNetwork.SetInterestGroups(Array.Empty<byte>(), new byte[] { (byte)SaveDataManager.saveData.mapInfo.mapName });

        //�G����V�����X�|�[��������
        EnemyInfoManager.Instance.InitialEnemyInfoList();

        //�����̏��𓯂��O���[�v�ɑ��M����
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        //�����O���[�v�̑��v���C���[�̏�񂪂��炦��悤�Ƀ��N�G�X�g����
        OtherPlayerCharaInfoManager.Instance.RequestOtherPlayerCharaInfo();

        yield return new WaitForSeconds(1.0f);

        if (SaveDataManager.saveData.charaInfo.hp <= 0)
        {
            CharacterInfoManager.Instance.CharacterDeathEvent();
        }


        loginWindow.SetActive(false);
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);
        AddText(">"+errorInfo.Info);
        Debug.LogError(errorInfo.Info);
    }

    public void AddText(string text)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = text;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        AddText(">"+ returnCode.ToString() +":"+ message);
        Debug.LogError(message);
    }

    [ContextMenu("RemoveSaveDataFile")]
    public void RemoveSaveDataFile()
    {
        SaveDataManager.RemoveSaveDataFile();
    }
}
