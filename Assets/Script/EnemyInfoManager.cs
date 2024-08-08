using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System;

public class EnemyInfoManager : SingletonMonoBehaviourPunCallbacks<EnemyInfoManager>
{
    public bool isInitalEnemyInfo=false;
    public EnemyInfoList enemyInfoList;
    public UnityEvent OnEnemyInfoUpdate = new UnityEvent();
    public int targetEnemyId = 0;
    public float updateTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemyInfoList = new EnemyInfoList();
        OnEnemyInfoUpdate.AddListener(() => CheckKillEnemy());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //�ł̃I�[�i�[��j������
        foreach (EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
        {
            if (enemyInfo.enemyPosion != null)
            {
                if (enemyInfo.enemyPosion.owenrUserId == otherPlayer.UserId)
                {
                    enemyInfo.enemyPosion = null;
                }
            }
        }
    }

    private void Update()
    {
        updateTime += Time.deltaTime;
        if (updateTime > 1)
        {
            bool enemyDamaged = false;
            foreach (EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
            {
                if (enemyInfo.currentHp <= 0) continue;

                if (enemyInfo.enemyPosion != null)
                {
                    //�ł̃I�[�i�[�v���C���[����������
                    if (enemyInfo.enemyPosion.owenrUserId == PhotonNetwork.LocalPlayer.UserId)
                    {
                        if (enemyInfo.isPoisoned)
                        {
                            enemyInfo.currentHp -= enemyInfo.enemyPosion.damage;
                            if (enemyInfo.currentHp < 0) enemyInfo.currentHp = 0;
                            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + enemyInfo.enemyData.enemyName + "�͓ł�" + enemyInfo.enemyPosion.damage + "�_���[�W�󂯂�");
                            enemyDamaged = true;
                        }                        
                    }
                }
            }
            updateTime--;

            if (enemyDamaged)
            {
                SendMyEnemyInfo();
            }
            OnEnemyInfoUpdate.Invoke();
        }
        foreach (EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
        {
            enemyInfo.attackCoolTime -= Time.deltaTime;
            if (enemyInfo.attackCoolTime < 0)
            {
                if (SaveDataManager.saveData.charaInfo.hp > 0)
                {
                    PlayerAttacked(enemyInfo.enemyId);
                }
                enemyInfo.attackCoolTime = 2;
            }            
        }
    }

    public EnemyInfo GetEnemy(int enemyId)
    {
        foreach(EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
        {
            if(enemyInfo.enemyId == enemyId)
            {
                return enemyInfo;
            }
        }
        return null;
    }

    public void InitialEnemyInfoList()
    {
        StartCoroutine(InitalEnemyInfoListCoroutine());
    }

    public IEnumerator InitalEnemyInfoListCoroutine()
    {
        isInitalEnemyInfo = false;
        enemyInfoList.enemyInfos.Clear();
        OnEnemyInfoUpdate.Invoke();
        
        foreach(EnemyData enemyData in SaveDataManager.saveData.mapInfo.mapData.enemyDatas)
        {
            int count = enemyInfoList.enemyInfos.Count;
            EnemyInfo enemyInfo = new EnemyInfo(enemyData, count);
            enemyInfo.attackCoolTime = enemyInfoList.enemyInfos.Count * 0.15f + 2;
            enemyInfoList.enemyInfos.Add(enemyInfo);
        }

        RequestOtherPlayerEnemyInfo();
        Debug.Log("Request");

        yield return new WaitForSeconds(0.5f);
        
        isInitalEnemyInfo = true;
        Debug.Log("isInitalEnemyInfo");
        OnEnemyInfoUpdate.Invoke();
    }

    private void RequestOtherPlayerEnemyInfo()
    {
        photonView.RPC(nameof(SendMyEnemyInfo), RpcTarget.Others);
    }

    [PunRPC]
    public void SendMyEnemyInfo()
    {
        //���g�̉�ʂŌ����Ă���G�̏��𑗂�
        photonView.RPC(nameof(ReceveEnemyInfoRPC), RpcTarget.Others, new object[] { SaveDataManager.saveData.mapInfo.mapName, enemyInfoList.GetJson() }) ;

        OnEnemyInfoUpdate.Invoke();
    }

    [PunRPC]
    private void ReceveEnemyInfoRPC(MapName mapName, string info)
    {
        //�G�l�~�[�f�[�^���󂯎���āA�����̉�ʂ���HP�����炵�Ă����炻�����ɍ��킹��
        EnemyInfoList receveEnemyInfoList = JsonUtility.FromJson<EnemyInfoList>(info);
        Debug.Log("Receve enemy info data:" + mapName.ToString() + "," + enemyInfoList.enemyInfos.Count);

        if (SaveDataManager.saveData.mapInfo.mapName != mapName) return;
        foreach(EnemyInfo receveEnemyInfo in receveEnemyInfoList.enemyInfos)
        {
            foreach(EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
            {
                if(receveEnemyInfo.enemyId == enemyInfo.enemyId)
                {
                    //HP�̏�Ԃ����킹��i�Ⴂ�ق��Ɂj
                    if (receveEnemyInfo.currentHp < enemyInfo.currentHp)
                    {
                        enemyInfo.currentHp = receveEnemyInfo.currentHp;
                    }

                    //�ł̏�Ԃ����킹��i�Ȃ�ׂ��Ō�ɕt�^�����ق��Ɂj
                    if (enemyInfo.enemyPosion == null)
                    {
                        enemyInfo.enemyPosion = receveEnemyInfo.enemyPosion;
                    }
                    else if(receveEnemyInfo.enemyPosion.effectTime.Value.Ticks > enemyInfo.enemyPosion.effectTime.Value.Ticks)
                    {
                        enemyInfo.enemyPosion = receveEnemyInfo.enemyPosion;
                    }
                }
            }
        }

        //�����œG�������������ǂ����̏���������H
        OnEnemyInfoUpdate.Invoke();
    }

    public void CheckKillEnemy()
    {
        foreach (EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
        {
            if (enemyInfo.currentHp <= 0 && !enemyInfo.isGetDropItem)
            {
                OnKillEnemy(enemyInfo.enemyId);
                enemyInfo.isGetDropItem = true;
            }
        }
    }

    public void OnKillEnemy(int targetID)
    {
        ChatMenuManager.Instance.AddText(">"
                + GetEnemy(targetID).enemyData.enemyName + "�����������I");

        foreach (DropItemData dropItemData in GetEnemy(targetID).enemyData.dropItemDatas)
        {
            float random = UnityEngine.Random.Range(0, 1.0f);
            if (random < dropItemData.dropChance)
            {
                ActionItemAdjustment actionGetItem = new ActionItemAdjustment();
                ActionArgs actionArgs = new ActionArgs();

                int mount = UnityEngine.Random.Range(dropItemData.dropMinMount, dropItemData.dropMaxMount + 1);

                actionArgs.targetItemName = dropItemData.itemName;
                actionArgs.mount = mount;
                actionGetItem.ExecuteAction(actionArgs);
            }
        }

        JobInfo jobInfo = SaveDataManager.saveData.charaInfo.GetCurrentJobInfo();
        jobInfo.exp++;
        if (jobInfo.exp >= jobInfo.nextMaxExp)
        {
            jobInfo.maxHp += jobInfo.jobData.lvupAddHp;
            jobInfo.maxMp += jobInfo.jobData.lvupAddMp;
            SaveDataManager.saveData.charaInfo.hp = SaveDataManager.saveData.charaInfo.maxHp;
            SaveDataManager.saveData.charaInfo.mp = SaveDataManager.saveData.charaInfo.maxMp;
            jobInfo.lv++;
            jobInfo.exp -= jobInfo.nextMaxExp;
            jobInfo.nextMaxExp += 2;
            ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + SaveDataManager.saveData.charaInfo.name + "�̓��x���A�b�v�����I�iLv" + jobInfo.lv + "�j");
            if (jobInfo.jobData.lvupAddHp>0)
            {
                ChatMenuManager.Instance.AddText(">�ő�HP���������� " + (SaveDataManager.saveData.charaInfo.maxHp - jobInfo.jobData.lvupAddHp) + " -> " + SaveDataManager.saveData.charaInfo.maxHp);
            }
            if (jobInfo.jobData.lvupAddMp > 0)
            {
                ChatMenuManager.Instance.AddText(">�ő�MP���������� " + (SaveDataManager.saveData.charaInfo.maxMp - jobInfo.jobData.lvupAddMp) + " -> " + SaveDataManager.saveData.charaInfo.maxMp);
            }
        }

        foreach (ActionData actionData in GetEnemy(targetID).enemyData.deathActions)
        {
            actionData.action.ExecuteAction(actionData.args);
        }
    }

    public void PlayerAttacked(int targetID)
    {
        //����`�F�b�N
        bool isAvoid = false;

        float weaponAvoidChance = SaveDataManager.saveData.charaInfo.itemDataEquipHand.avoidChance;
        float weaponAvoidRandom = UnityEngine.Random.Range(0, 1.0f);
        //�W���u�ɂ���𗦕␳
        weaponAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (weaponAvoidRandom <= weaponAvoidChance) isAvoid = true;

        float bodyAvoidChance = SaveDataManager.saveData.charaInfo.itemDataEquipBody.avoidChance;
        float bodyAvoidRandom = UnityEngine.Random.Range(0, 1.0f);
        //�W���u�ɂ���𗦕␳
        bodyAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (bodyAvoidRandom <= bodyAvoidChance)
        {
            isAvoid = true;

            //�Z�̎g�p�񐔂����炷
            if (SaveDataManager.saveData.charaInfo.itemDataEquipBody.maxDurability > 0)
            {
                ActionItemUseCount actionItemUseCount = new ActionItemUseCount();
                ActionArgs itemUseArgs = new ActionArgs();
                itemUseArgs.mount = 1;
                itemUseArgs.targetItemName = SaveDataManager.saveData.charaInfo.itemNameEquipBody;
                actionItemUseCount.ExecuteAction(itemUseArgs);
            }
        }

        float accessoryAvoidChance = SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.avoidChance;
        float accessoryAvoidRandom = UnityEngine.Random.Range(0, 1.0f);
        //�W���u�ɂ���𗦕␳
        accessoryAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (accessoryAvoidRandom <= accessoryAvoidChance)
        {
            isAvoid = true;

            //�A�N�Z�T���̎g�p�񐔂����炷
            if (SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.maxDurability > 0)
            {
                ActionItemUseCount actionItemUseCount = new ActionItemUseCount();
                ActionArgs itemUseArgs = new ActionArgs();
                itemUseArgs.mount = 1;
                itemUseArgs.targetItemName = SaveDataManager.saveData.charaInfo.itemNameEquipAccessory;
                actionItemUseCount.ExecuteAction(itemUseArgs);
            }
        }


        if (isAvoid)
        {
            ChatMenuManager.Instance.AddText(">"
            + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "�̍U���I"
            + PhotonNetwork.LocalPlayer.NickName + "�͍U�����Ђ��Ƃ��킵���I", Color.yellow);

            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return ;
        }


        int guardPoint = 0;

        float weaponGuardChance = SaveDataManager.saveData.charaInfo.itemDataEquipHand.guardChance;
        float weaponGuardRandom = UnityEngine.Random.Range(0, 1.0f);
        //�W���u�ɂ��K�[�h���␳
        weaponGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (weaponGuardRandom <= weaponGuardChance)
        {
            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipHand.guardPoint;
        }

        float bodyGuardChance = SaveDataManager.saveData.charaInfo.itemDataEquipBody.guardChance;
        float bodyGuardRandom = UnityEngine.Random.Range(0, 1.0f);
        //�W���u�ɂ��K�[�h���␳
        bodyGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (bodyGuardRandom <= bodyGuardChance)
        {
            //�h�䎞�A�N�V����
            foreach (ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipBody.equipDefenceActions)
            {
                actionData.action.ExecuteAction(actionData.args);
            }

            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipBody.guardPoint;

            //�Z�̎g�p�񐔂����炷
            if (SaveDataManager.saveData.charaInfo.itemDataEquipBody.maxDurability > 0)
            {
                ActionItemUseCount actionItemUseCount = new ActionItemUseCount();
                ActionArgs itemUseArgs = new ActionArgs();
                itemUseArgs.mount = 1;
                itemUseArgs.targetItemName = SaveDataManager.saveData.charaInfo.itemNameEquipBody;
                actionItemUseCount.ExecuteAction(itemUseArgs);
            }
        }

        float accessoryGuardChance = SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.guardChance;
        float accessoryGuardRandom = UnityEngine.Random.Range(0, 1.0f);

        //�W���u�ɂ��K�[�h���␳
        accessoryGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (accessoryGuardRandom <= accessoryGuardChance)
        {
            //�h�䎞�A�N�V����
            foreach (ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.equipDefenceActions)
            {
                actionData.action.ExecuteAction(actionData.args);
            }

            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.guardPoint;

            //�A�N�Z�T���̎g�p�񐔂����炷
            if (SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.maxDurability > 0)
            {
                ActionItemUseCount actionItemUseCount = new ActionItemUseCount();
                ActionArgs itemUseArgs = new ActionArgs();
                itemUseArgs.mount = 1;
                itemUseArgs.targetItemName = SaveDataManager.saveData.charaInfo.itemNameEquipAccessory;
                actionItemUseCount.ExecuteAction(itemUseArgs);
            }
        }


        int atk = EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyAtk;
        atk -= guardPoint;
        if (atk < 0) atk = 0;

        SaveDataManager.saveData.charaInfo.hp -= atk;

        if (guardPoint > 0)
        {
            ChatMenuManager.Instance.AddText(">"
            + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "�̍U���I"
            + PhotonNetwork.LocalPlayer.NickName + "�͖h�䂵��" + atk + "�_���[�W���󂯂�", Color.yellow);
        }
        else
        {
            ChatMenuManager.Instance.AddText(">"
               + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "�̍U���I"
               + PhotonNetwork.LocalPlayer.NickName + "��" + atk + "�_���[�W�I");
        }



        if (SaveDataManager.saveData.charaInfo.hp <= 0)
        {
            SaveDataManager.saveData.charaInfo.hp = 0;
            CharacterInfoManager.Instance.CharacterDeathEvent();
        }
    }
}

[System.Serializable]
public class EnemyInfoList
{
    public List<EnemyInfo> enemyInfos;

    public EnemyInfoList()
    {
        enemyInfos = new List<EnemyInfo>();
    }

    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class EnemyInfo
{
    public int enemyId;
    public EnemyData enemyData;
    public int currentHp;
    public bool isGetDropItem;
    public EnemyPosion enemyPosion;
    public float attackCoolTime = 0;

    public bool isPoisoned
    {
        get
        {
            if (enemyPosion == null) return false;
            if (DateTime.Now > enemyPosion.effectTime.Value.AddSeconds(enemyPosion.count))
            {
                return false;
            }
            return true;
        }
    }

    public EnemyInfo(EnemyData enemyData,int enemyId)
    {
        this.enemyData = enemyData;
        this.currentHp = enemyData.enemyMaxHp;
        this.enemyId = enemyId;
        this.isGetDropItem = false;
        this.enemyPosion = null;
    }

}

[System.Serializable]
public class EnemyPosion
{
    public SerializableDateTime effectTime;
    public string owenrUserId;
    public int count;
    public int damage;
}

