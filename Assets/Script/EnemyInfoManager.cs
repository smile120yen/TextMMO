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
        //毒のオーナーを破棄する
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
                    //毒のオーナープレイヤーだけが処理
                    if (enemyInfo.enemyPosion.owenrUserId == PhotonNetwork.LocalPlayer.UserId)
                    {
                        if (enemyInfo.isPoisoned)
                        {
                            enemyInfo.currentHp -= enemyInfo.enemyPosion.damage;
                            if (enemyInfo.currentHp < 0) enemyInfo.currentHp = 0;
                            ChatMenuManager.Instance.SendTextRPCInSameMap(">" + enemyInfo.enemyData.enemyName + "は毒で" + enemyInfo.enemyPosion.damage + "ダメージ受けた");
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
        //自身の画面で見えている敵の情報を送る
        photonView.RPC(nameof(ReceveEnemyInfoRPC), RpcTarget.Others, new object[] { SaveDataManager.saveData.mapInfo.mapName, enemyInfoList.GetJson() }) ;

        OnEnemyInfoUpdate.Invoke();
    }

    [PunRPC]
    private void ReceveEnemyInfoRPC(MapName mapName, string info)
    {
        //エネミーデータを受け取って、自分の画面よりもHPを減らしていたらそっちに合わせる
        EnemyInfoList receveEnemyInfoList = JsonUtility.FromJson<EnemyInfoList>(info);
        Debug.Log("Receve enemy info data:" + mapName.ToString() + "," + enemyInfoList.enemyInfos.Count);

        if (SaveDataManager.saveData.mapInfo.mapName != mapName) return;
        foreach(EnemyInfo receveEnemyInfo in receveEnemyInfoList.enemyInfos)
        {
            foreach(EnemyInfo enemyInfo in enemyInfoList.enemyInfos)
            {
                if(receveEnemyInfo.enemyId == enemyInfo.enemyId)
                {
                    //HPの状態を合わせる（低いほうに）
                    if (receveEnemyInfo.currentHp < enemyInfo.currentHp)
                    {
                        enemyInfo.currentHp = receveEnemyInfo.currentHp;
                    }

                    //毒の状態を合わせる（なるべく最後に付与したほうに）
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

        //ここで敵をたおしたかどうかの処理をする？
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
                + GetEnemy(targetID).enemyData.enemyName + "をたおした！");

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
            ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + SaveDataManager.saveData.charaInfo.name + "はレベルアップした！（Lv" + jobInfo.lv + "）");
            if (jobInfo.jobData.lvupAddHp>0)
            {
                ChatMenuManager.Instance.AddText(">最大HPが増加した " + (SaveDataManager.saveData.charaInfo.maxHp - jobInfo.jobData.lvupAddHp) + " -> " + SaveDataManager.saveData.charaInfo.maxHp);
            }
            if (jobInfo.jobData.lvupAddMp > 0)
            {
                ChatMenuManager.Instance.AddText(">最大MPが増加した " + (SaveDataManager.saveData.charaInfo.maxMp - jobInfo.jobData.lvupAddMp) + " -> " + SaveDataManager.saveData.charaInfo.maxMp);
            }
        }

        foreach (ActionData actionData in GetEnemy(targetID).enemyData.deathActions)
        {
            actionData.action.ExecuteAction(actionData.args);
        }
    }

    public void PlayerAttacked(int targetID)
    {
        //回避チェック
        bool isAvoid = false;

        float weaponAvoidChance = SaveDataManager.saveData.charaInfo.itemDataEquipHand.avoidChance;
        float weaponAvoidRandom = UnityEngine.Random.Range(0, 1.0f);
        //ジョブによる回避率補正
        weaponAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (weaponAvoidRandom <= weaponAvoidChance) isAvoid = true;

        float bodyAvoidChance = SaveDataManager.saveData.charaInfo.itemDataEquipBody.avoidChance;
        float bodyAvoidRandom = UnityEngine.Random.Range(0, 1.0f);
        //ジョブによる回避率補正
        bodyAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (bodyAvoidRandom <= bodyAvoidChance)
        {
            isAvoid = true;

            //鎧の使用回数を減らす
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
        //ジョブによる回避率補正
        accessoryAvoidChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.avoidChanceMultiple;
        if (accessoryAvoidRandom <= accessoryAvoidChance)
        {
            isAvoid = true;

            //アクセサリの使用回数を減らす
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
            + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "の攻撃！"
            + PhotonNetwork.LocalPlayer.NickName + "は攻撃をひらりとかわした！", Color.yellow);

            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return ;
        }


        int guardPoint = 0;

        float weaponGuardChance = SaveDataManager.saveData.charaInfo.itemDataEquipHand.guardChance;
        float weaponGuardRandom = UnityEngine.Random.Range(0, 1.0f);
        //ジョブによるガード率補正
        weaponGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (weaponGuardRandom <= weaponGuardChance)
        {
            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipHand.guardPoint;
        }

        float bodyGuardChance = SaveDataManager.saveData.charaInfo.itemDataEquipBody.guardChance;
        float bodyGuardRandom = UnityEngine.Random.Range(0, 1.0f);
        //ジョブによるガード率補正
        bodyGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (bodyGuardRandom <= bodyGuardChance)
        {
            //防御時アクション
            foreach (ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipBody.equipDefenceActions)
            {
                actionData.action.ExecuteAction(actionData.args);
            }

            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipBody.guardPoint;

            //鎧の使用回数を減らす
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

        //ジョブによるガード率補正
        accessoryGuardChance *= SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.guardChanceMultiple;
        if (accessoryGuardRandom <= accessoryGuardChance)
        {
            //防御時アクション
            foreach (ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.equipDefenceActions)
            {
                actionData.action.ExecuteAction(actionData.args);
            }

            guardPoint += SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.guardPoint;

            //アクセサリの使用回数を減らす
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
            + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "の攻撃！"
            + PhotonNetwork.LocalPlayer.NickName + "は防御して" + atk + "ダメージを受けた", Color.yellow);
        }
        else
        {
            ChatMenuManager.Instance.AddText(">"
               + EnemyInfoManager.Instance.GetEnemy(targetID).enemyData.enemyName + "の攻撃！"
               + PhotonNetwork.LocalPlayer.NickName + "に" + atk + "ダメージ！");
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

