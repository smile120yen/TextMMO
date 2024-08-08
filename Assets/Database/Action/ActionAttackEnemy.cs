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
            ChatMenuManager.Instance.AddText(">���Ȃ��͂��łɎ���ł���");
            return false;
        }

        if (EnemyInfoManager.Instance.GetEnemy(args.targetID).currentHp <= 0)
        {
            ChatMenuManager.Instance.AddText(">"
                + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�͂��łɑ��₦�Ă���");
            return false;
        }

        int consumeStamina = 1;
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipHand.staminaConsumptionIncrease;
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipBody.staminaConsumptionIncrease;
        

        if (SaveDataManager.saveData.charaInfo.currentStamina < consumeStamina)
        {
            ChatMenuManager.Instance.AddText(">�U���ɕK�v�ȃX�^�~�i������܂���i�X�^�~�i��" + consumeStamina + "�K�v�j");
            return false;
        }

        int consumeMp = SaveDataManager.saveData.charaInfo.itemDataEquipHand.consumeMp;
        int currentMp = SaveDataManager.saveData.charaInfo.mp;

        if (consumeMp > currentMp)
        {
            ChatMenuManager.Instance.AddText(">�U���ɕK�v��MP������܂���i����MP:"+consumeMp+"�j");
            return false;
        }


        return true;
    }

    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("Attack:" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"("+ args.targetID+")");

        //�X�^�~�i����
        int consumeStamina = 1;

        //����ɂ��ǉ��X�^�~�i����
        int weaponStaminaIncrease = SaveDataManager.saveData.charaInfo.itemDataEquipHand.staminaConsumptionIncrease;

        //�W���u�␳���ʗp�ϐ�
        EquipmentType weaponEquipmentType = SaveDataManager.saveData.charaInfo.itemDataEquipHand.equipmentType;
        EquipmentType armorEquipmentType = SaveDataManager.saveData.charaInfo.itemDataEquipBody.equipmentType;

        //�W���u�ɂ�����X�^�~�i�␳
        foreach (MultipleWithType multipleWithType in SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.staminaConsumeMultipleWithTypes)
        {
            if (weaponEquipmentType == multipleWithType.equipmentType)
            {
                weaponStaminaIncrease = (int)Mathf.Ceil(weaponStaminaIncrease * multipleWithType.multiple);
                break;
            }
        }
        consumeStamina += weaponStaminaIncrease;

        //�h��ɂ��ǉ��X�^�~�i����
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipBody.staminaConsumptionIncrease;

        //�A�N�Z�T���ɂ��ǉ��X�^�~�i����
        consumeStamina += SaveDataManager.saveData.charaInfo.itemDataEquipAccessory.staminaConsumptionIncrease;

        SaveDataManager.saveData.charaInfo.currentStamina -= consumeStamina;

        //MP����
        SaveDataManager.saveData.charaInfo.mp -= SaveDataManager.saveData.charaInfo.itemDataEquipHand.consumeMp;

        //�_���[�W����
        int minAtk = SaveDataManager.saveData.charaInfo.itemDataEquipHand.minAtk;
        int maxAtk = SaveDataManager.saveData.charaInfo.itemDataEquipHand.maxAtk;
        int damage = UnityEngine.Random.Range(minAtk, maxAtk+1);

        //�_���[�W�o�t
        foreach(AttackBuffInfo attackBuffInfo in SaveDataManager.saveData.charaInfo.attackBuffs)
        {
            damage += attackBuffInfo.attackBuff.mount;
        }


        //�W���u�ɂ��_���[�W�␳
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


        ChatMenuManager.Instance.SendTextRPCInSameMap(">" + PhotonNetwork.LocalPlayer.NickName + "��" 
        + SaveDataManager.saveData.charaInfo.itemDataEquipHand.name + "��"
        + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName+"��"+ damage + "�_���[�W�I");

        //�Ŕ���
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

                ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�͓łɂȂ����I");
            }
        }

        //�U�����A�N�V����
        foreach(ActionData actionData in SaveDataManager.saveData.charaInfo.itemDataEquipHand.equipAttackActions)
        {
            actionData.action.ExecuteAction(actionData.args);
        }


        //�A�C�e�������
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


            //�h���b�v�A�C�e������
            //�����}�b�v�̕ʂ̃v���C���[�����������Ƃ����Ⴆ��悤�ɂ�����
            //�������A�����ɓG��|�����Ƃ���2�d�ɏ�������Ă��܂�
            //���[�J����HP�����Ă����������ǂ������肷��悤�ɂ���H
            //EnemyInfoManager�ŏ�������
            /*
            ChatMenuManager.Instance.SendTextRPCInSameMap(">"
                + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�����������I");

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
                ChatMenuManager.Instance.SendTextRPCInSameMapWithHighlight(">" + SaveDataManager.saveData.charaInfo.name + "�̓��x���A�b�v�����I�iLv" + SaveDataManager.saveData.charaInfo.lv + "�j");
                ChatMenuManager.Instance.AddText(">�ő�HP���������� " + (SaveDataManager.saveData.charaInfo.maxHp - 2) + " -> " + SaveDataManager.saveData.charaInfo.maxHp);
            }
            */
            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return true;
        }


        //�G�̍U���C�x���g�͂����EnemyInfo�ɈڐA
        /*
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
            + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�̔����I"
            + PhotonNetwork.LocalPlayer.NickName + "�͍U�����Ђ��Ƃ��킵���I",Color.yellow);

            SaveDataManager.Save();
            OtherPlayerCharaInfoManager.Instance.SendMyCharaInfo();
            EnemyInfoManager.Instance.SendMyEnemyInfo();
            return true;
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


        int atk = EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyAtk;
        atk -= guardPoint;
        if (atk < 0) atk = 0;

        SaveDataManager.saveData.charaInfo.hp -= atk;

        if (guardPoint > 0)
        {          
            ChatMenuManager.Instance.AddText(">"
            + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�̔����I"
            + PhotonNetwork.LocalPlayer.NickName + "�͖h�䂵��" + atk + "�_���[�W���󂯂�", Color.yellow);
        }
        else
        {
            ChatMenuManager.Instance.AddText(">"
               + EnemyInfoManager.Instance.GetEnemy(args.targetID).enemyData.enemyName + "�̔����I"
               + PhotonNetwork.LocalPlayer.NickName + "��" + atk + "�_���[�W�I");
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
