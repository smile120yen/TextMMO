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
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
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

        AddText(">サーバーに接続中……");



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

        AddText(">サーバーに接続しました");

        if (SaveDataManager.saveData.charaInfo.name == null)
        {
            AddText(">あなたの名前を入力してください");

            //テキスト入力待ち合わせ処理

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

        AddText(">ルームに入室しています……");

        RoomOptions options = new RoomOptions();
        options.PublishUserId = true;
        options.MaxPlayers = 100;

        PhotonNetwork.JoinOrCreateRoom("Room", options, TypedLobby.Default);

        while (true)
        {
            if (PhotonNetwork.InRoom) break;
            yield return null;
        }

        AddText(">入室に成功しました");

        //ネットーワークカリング設定
        PhotonNetwork.SetInterestGroups(Array.Empty<byte>(), new byte[] { (byte)SaveDataManager.saveData.mapInfo.mapName });

        //敵情報を新しくスポーンさせる
        EnemyInfoManager.Instance.InitialEnemyInfoList();

        //自分の情報を同じグループに送信する
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        //同じグループの他プレイヤーの情報がもらえるようにリクエストする
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
