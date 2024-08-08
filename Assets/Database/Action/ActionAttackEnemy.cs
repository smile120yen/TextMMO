using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ActionAttackEnemy : Action
{
    public override bool CheckCanExecute(ActionArgs args)
    {
        if (SaveDataManager.saveData.charaInfo.hp <= 0)
        {
            ChatMenuManager.Instance.AddText(">あなたはすでに死んでいる");
            return false;
        }

        if (EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp <= 0)
        {
            ChatMenuManager.Instance.AddText(">"
                + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "はすでに息絶えている");
            return false;
        }

        int consumeStamina = 1;
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipHand.staminaConsumptionIncrease;
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipBody.staminaConsumptionIncrease;
        

        if (SaveDataManager.saveData.charaInfo.currentStamina < consumeStamina)
        {
            ChatMenuManager.Instance.AddText(">攻撃に必要なスタミナが足りません（スタミナが" + consumeStamina + "必要）");
            return false;
        }

        int consumeMp = SaveDataManager.saveData.charaInfo.itemDataEquipHand.consumeMp;
        int currentMp = SaveDataManager.saveData.charaInfo.mp;

        if (consumeMp > currentMp)
        {
            ChatMenuManager.Instance.AddText(">攻撃に必要なMPが足りません（消費MP:"+consumeMp+"）");
            return false;
        }


        return true;
    }

    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("Attack:" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"("+ args.targetID+")");

        //スタミナ消費
        int consumeStamina = 1;

        //武器による追加スタミナ消費
        int weaponStaminaIncrease = SaveDataManager.saveData.charaInfo.itemDataEquipHand.staminaConsumptionIncrease;

        //ジョブ補正判別用変数
        EquipmentType weaponEquipmentType = SaveDataManager.saveData.charaInfo.itemDataEquipHand.equipmentType;
        EquipmentType armorEquipmentType = SaveDataManager.saveData.charaInfo.itemDataEquipBody.equipmentType;

        //ジョブによる消費スタミナ補正
        foreach (MultipleWithType multipleWithType in SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.staminaConsumeMultipleWithTypes)
        {
            if (weaponEquipmentType == multipleWithType.equipmentType)
            {
                weaponStaminaIncrease = (int)Mathf.Ceil(weaponStaminaIncrease * multipleWithType.multiple);
                break;
            }
        }
        consumeStamina += weaponStaminaIncrease;

        //防具による追加スタミナ消費
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipBody.staminaConsumptionIncrease;

        //アクセサリによる追加スタミナ消費
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.staminaConsumptionIncrease;

        SaveDataManager.saveData.charaInfo.currentStamina -= consumeStamina;

        //MP消費
        SaveDataManager.saveData.charaInfo.mp -= SaveDataManager.saveData.charaInfo.itemDataEquipHand.consumeMp;

        //ダメージ決定
        int minAtk = SaveDataManager.saveData.charaInfo.itemDataEquipHand.minAtk;
        int maxAtk = SaveDataManager.saveData.charaInfo.itemDataEquipHand.maxAtk;
        int damage = UnityEngine.Random.Range(minAtk, maxAtk+1);

        //ダメージバフ
        foreach(AttackBuffInfo attackBuffInfo in SaveDataManager.saveData.charaInfo.attackBuffs)
        {
            damage += attackBuffInfo.attackBuff.mount;
        }


        //ジョブによるダメージ補正
        foreach(MultipleWithType multipleWithType in SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.damageMultipleWithTypes)
        {
            if(weaponEquipmentType == multipleWithType.equipmentType)
            {
                damage = (int)Mathf.Ceil(damage * multipleWithType.multiple);
                break;
            }
        }


        if (damage < 0) damage = 0;

        EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp-= damage;
        if (EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp < 0) EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp = 0;


        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "は" 
        + SaveDataManager.saveData.charaInfo.itemDataEquipHand.name + "で"
        + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"に"+ damage + "ダメージ！");

        //毒判定
        if (SaveDataManager.saveData.charaInfo.itemDataEquipHand.poisonDamage > 0)
        {
            float poisonRandom = UnityEngine.Random.Range(0, 1.0f);
            if (SaveDataManager.saveData.charaInfo.itemDataEquipHand.poisonChance > poisonRandom)
            {
                EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyPosion =
                    new EnemyPosion
                    {
                        owenrUserId = PhotonNetwork.LocalPlayer.UserId,
                        effectTime = DateTime.Now,
                        count = SaveDataManager.saveData.charaInfo.itemDataEquipHand.poisonDuration,
                        damage = SaveDataManager.saveData.charaInfo.itemDataEquipHand.poisonDamage
                    };

                ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "は毒になった！");
            }
        }

        //攻撃時アクション
        foreach(ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipHand.equipAttackActions)
        {
            actionData.action.ExecuteAction(actionData.args);
        }


        //アイテム消費判定
        if (SaveDataManager.saveData.charaInfo.itemDataEquipHand.maxDurability > 0)
        {
            ActionItemUseCount actionItemUseCount = new ActionItemUseCount();
            ActionArgs itemUseArgs = new ActionArgs();
            itemUseArgs.mount = 1;
            itemUseArgs.targetItemName = SaveDataManager.saveData.charaInfo.itemNameEquipHand;
            actionItemUseCount.ExecuteAction(itemUseArgs);
        }

        if (EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp <= 0)
        {


            //ドロップアイテム処理
            //同じマップの別のプレイヤーがたおしたときも貰えるようにしたい
            //しかし、同時に敵を倒したときに2重に処理されてしまう
            //ローカルでHPを見てたおしたかどうか判定するようにする？
            //EnemyInfoManagerで処理する
            /*
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"
                + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "をたおした！");

            foreach (DropItemData dropItemData in EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.dropItemDatas)
            {
                float random = UnityEngine.Random.Range(0, 1.0f);
                if (random < dropItemData.dropChance)
                {
                    ActionItemAdjustment actionGetItem = new ActionItemAdjustment();
                    ActionArgs actionArgs = new ActionArgs();

                    int mount = UnityEngine.Random.Range(dropItemData.dropMinMount, dropItemData.dropMaxMount + 1);

                    actionArgs.targetItemInfo = new ItemInfo(dropItemData.itemName);
                    actionArgs.targetItemInfo.mount = mount;
                    actionGetItem.ExecuteAction(actionArgs);
                }
            }

            SaveDataManager.saveData.charaInfo.exp++;
            if (SaveDataManager.saveData.charaInfo.exp >= SaveDataManager.saveData.charaInfo.nextMaxExp)
            {
                SaveDataManager.saveData.charaInfo.maxHp += 2;
                SaveDataManager.saveData.charaInfo.hp = SaveDataManager.saveData.charaInfo.maxHp;
                SaveDataManager.saveData.charaInfo.lv++;
                SaveDataManager.saveData.charaInfo.exp -= SaveDataManager.saveData.charaInfo.nextMaxExp;
                SaveDataManager.saveData.charaInfo.nextMaxExp += 2;
                ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + SaveDataManager.saveData.charaInfo.name + "はレベルアップした！（Lv" + SaveDataManager.saveData.charaInfo.lv + "）");
                ChatMenuManager.Instance.AddText(">最大HPが増加した " + (SaveDataManager.saveData.charaInfo.maxHp - 2) + " -> " + SaveDataManager.saveData.charaInfo.maxHp);
            }
            */
            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return true;
        }


        //敵の攻撃イベントはぜんぶEnemyInfoに移植
        /*
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
            + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "の反撃！"
            + PhotonNetwork.LocalPlayer.NickName + "は攻撃をひらりとかわした！",Color.yellow);

            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return true;
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


        int atk = EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyAtk;
        atk -= guardPoint;
        if (atk < 0) atk = 0;

        SaveDataManager.saveData.charaInfo.hp -= atk;

        if (guardPoint > 0)
        {          
            ChatMenuManager.Instance.AddText(">"
            + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "の反撃！"
            + PhotonNetwork.LocalPlayer.NickName + "は防御して" + atk + "ダメージを受けた", Color.yellow);
        }
        else
        {
            ChatMenuManager.Instance.AddText(">"
               + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "の反撃！"
               + PhotonNetwork.LocalPlayer.NickName + "に" + atk + "ダメージ！");
        }

        

        if (SaveDataManager.saveData.charaInfo.hp <= 0)
        {
            SaveDataManager.saveData.charaInfo.hp = 0;
            CharacterInfoManager.Instance.CharacterDeathEvent();
        }
        */

        SaveDataManager.Save();
        OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
        EnemyInfoManager.Instance.SendMyEnemyInfo();
       
        return true;
    }

    
}
