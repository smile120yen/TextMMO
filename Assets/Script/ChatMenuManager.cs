using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ChatMenuManager : SingletonMonoBehaviourPunCallbacks<ChatMenuManager>
{
    [SerializeField] Transform parent;
    [SerializeField] GameObject textPrefab;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Transform parentWorldChat;
    [SerializeField] ScrollRect scrollRectWorldChat;

    public override void OnJoinedRoom()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            RPCCalledMethodAddText(">" + player.NickName + "がログイン中です", player.UserId);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddText(">"+ newPlayer.NickName + "がゲームに参加しました");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AddText(">" + otherPlayer.NickName + "がゲームから退出しました");
    }

    public void SendTextRPCInSameMap(string text)
    {
        photonView.Group = 0;
        photonView.RPC(nameof(RPCCalledMethodAddWorldChatText), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);

        photonView.Group = (byte)SaveDataManager.saveData.mapInfo.mapName;
        photonView.RPC(nameof(RPCCalledMethodAddText), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);
    }

    public void SendTextRPCInSameMapWithHighlight(string text)
    {
        photonView.Group = 0;
        photonView.RPC(nameof(RPCCalledMethodAddWorldChatText), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);

        photonView.Group = (byte)SaveDataManager.saveData.mapInfo.mapName;
        photonView.RPC(nameof(RPCCalledMethodAddTextWithHighlight), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);
    }

    public void SendTextRPCInSameMapDanger(string text)
    {
        photonView.Group = 0;
        photonView.RPC(nameof(RPCCalledMethodAddWorldChatText), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);

        photonView.Group = (byte)SaveDataManager.saveData.mapInfo.mapName;
        photonView.RPC(nameof(RPCCalledMethodAddTextWithDanger), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);
    }

    public void SendTextRPC(string text)
    {
        photonView.Group = 0;
        photonView.RPC(nameof(RPCCalledMethodAddText), RpcTarget.All, text, PhotonNetwork.LocalPlayer.UserId);
    }

    public void SendChatRPC(string text)
    {
        photonView.Group = 0;
        photonView.RPC(nameof(RPCCalledMethodAddText), RpcTarget.All, PhotonNetwork.NickName+":"+text, PhotonNetwork.LocalPlayer.UserId);
    }

    [PunRPC]
    private void RPCCalledMethodAddText(string text,string userId)
    {
        ActionInfo actionInfo = new ActionInfo();
        actionInfo.text = text;

        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
        actionData.args.userId = userId;
        
        actionInfo.actionDatas.Add(actionData);

        AddText(actionInfo);
    }

    [PunRPC]
    private void RPCCalledMethodAddWorldChatText(string text, string userId)
    {
        ActionInfo actionInfo = new ActionInfo();
        actionInfo.text = text;

        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
        actionData.args.userId = userId;

        actionInfo.actionDatas.Add(actionData);

        AddWorldChatText(actionInfo);
    }

    [PunRPC]
    private void RPCCalledMethodAddTextWithHighlight(string text, string userId)
    {
        ActionInfo actionInfo = new ActionInfo();
        actionInfo.text = text;
        actionInfo.color = Color.yellow;

        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
        actionData.args.userId = userId;

        actionInfo.actionDatas.Add(actionData);

        AddText(actionInfo);
    }

    [PunRPC]
    private void RPCCalledMethodAddTextWithDanger(string text, string userId)
    {
        ActionInfo actionInfo = new ActionInfo();
        actionInfo.text = text;
        actionInfo.color = Color.red;

        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionOpenCharaInfo") as ActionOpenCharaInfo;
        actionData.args.userId = userId;

        actionInfo.actionDatas.Add(actionData);

        AddText(actionInfo);
    }

    private void AddWorldChatText(ActionInfo actionInfo)
    {
        GameObject temp = Instantiate(textPrefab, parentWorldChat);
        temp.GetComponent<TextObject>().text = actionInfo.text;
        temp.GetComponent<TextObject>().color = Color.gray;
        temp.GetComponent<TextObject>().button.onClick.AddListener(
                () => {
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        bool isContinue = actionData.action.ExecuteAction(actionData.args);
                        if (!isContinue) break;
                    }
                });


        if (parentWorldChat.childCount > 20)
        {

            int count = parentWorldChat.childCount - 20;
            for (int i = 0; i < count; i++)
            {
                Destroy(parentWorldChat.GetChild(i).gameObject);
            }
        }

        Canvas.ForceUpdateCanvases();
        scrollRectWorldChat.verticalNormalizedPosition = 0;

        //SoundManager.Instance.Play(SoundName.ka);
    }

    private void AddText(ActionInfo actionInfo)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = actionInfo.text;
        temp.GetComponent<TextObject>().color = actionInfo.color;
        temp.GetComponent<TextObject>().button.onClick.AddListener(
                () => {
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        bool isContinue = actionData.action.ExecuteAction(actionData.args);
                        if (!isContinue) break;
                    }
                });


        if (parent.childCount>50)
        {

            int count = parent.childCount - 50;
            for (int i=0; i< count; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        parent.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;

        SoundManager.Instance.Play(SoundName.ka);
    }

    public void AddText(string text)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextMeshProUGUI>().text = text;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;

        SoundManager.Instance.Play(SoundName.ka);
        //Canvas.ForceUpdateCanvases();
    }

    public void AddText(string text,Color color)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextMeshProUGUI>().text = text;
        temp.GetComponent<TextMeshProUGUI>().color = color;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;

        SoundManager.Instance.Play(SoundName.ka);
        //Canvas.ForceUpdateCanvases();
    }
}
