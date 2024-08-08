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
                //����A���̏��X�V�n������ɓ͂��Ȃ��ƕ\���ɊԂɍ���Ȃ�
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
        //���̃v���C���[�ɋ��L���Ăق����ƃ��N�G�X�g����
        photonView.RPC(nameof(SendMyCharaInfo), RpcTarget.Others);
    }

    [PunRPC]
    public void SendMyCharaInfo()
    {
        //���g�̃L�����N�^�[�̃Z�[�u�f�[�^�𑗂�
        photonView.RPC(nameof(ReceveCharaInfoRPC), RpcTarget.All, new object[] { PhotonNetwork.LocalPlayer.UserId, SaveDataManager.GetJson() });

    }

    [PunRPC]
    public void ReceveCharaInfoRPC(string userId, string info)
    {
        //�Z�[�u�f�[�^���󂯎���ă��X�g�ɕۑ�����
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
        //�Q�[������ޏo�����v���C���[�������炻�̃v���C���[�̓��X�g�������
        playerInfo.Remove(otherPlayer.UserId);
        OnPlayerInfoUpdate.Invoke();
    }
}
