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
            //1�C�ł�HP0�ȏ�̓G����������s�ł��Ȃ�
            foreach (EnemyInfo enemyInfo in EnemyInfoManager.Instance.enemyInfoList.enemyInfos)
            {
                if (enemyInfo.currentHp > 0)
                {
                    ChatMenuManager.Instance.AddText(">�G������Ƃ��͑��̃}�b�v�Ɉړ��ł��Ȃ�");
                    return false;
                }
            }
        }

        Debug.Log("MoveMap:"+args.targetMapName);


        //���M�O���[�v��ς���O�Ƀ��b�Z�[�W��O�̃O���[�v�Ɏc��
        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "��" + DatabaseManager.Instance.mapDatabase.mapDatas.Find(s => s.uniqueName == args.targetMapName.ToString()).name + "�Ɉړ�����");

        //���M�O���[�v��ς���O�ɑO�̃O���[�v�ɍŌ�̃L�������𑗐M����
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();

        SaveDataManager.saveData.mapInfo.mapName = args.targetMapName;

        //�}�b�v�̒ǉ������N���A
        MapInfoWindowManager.Instance.additionalActions.Clear();
                
        //��M�O���[�v��ς���
        PhotonNetwork.SetInterestGroups(Array.Empty<byte>(), new byte[] { (byte)args.targetMapName });

        //�G����V�����X�|�[��������
        EnemyInfoManager.Instance.InitialEnemyInfoList();

        //�����O���[�v�̑��v���C���[�̃L������񂪂��炦��悤�Ƀ��N�G�X�g����
        OtherPlayerCharaInfoManager.Instance.RequestOtherPlayerCharaInfo();

        //�A�C�e���ڍׂ͕���
        ItemDetailInfoManager.Instance.CloseItemDetailInfo();

        //�L�����ڍׂ�����
        CharaDetailInfoManager.Instance.CloseCharaDetailInfo();

        SaveDataManager.Save();
        return true;
    }
}
