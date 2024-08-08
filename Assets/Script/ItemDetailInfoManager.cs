using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetailInfoManager : SingletonMonoBehaviour<ItemDetailInfoManager>
{

    [SerializeField] GameObject ItemDetailInfo;
    [SerializeField] Transform parent;
    [SerializeField] GameObject textPrefab;
    ItemInfo itemInfo;

    private void Start()
    {
        SaveDataManager.OnSaveDataUpdate.AddListener(() => UpdateItemDetailInfo());
    }

    public void UpdateItemDetailInfo()
    {
        if (itemInfo == null) return;

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        ActionInfo closeItemInfo = new ActionInfo();
        closeItemInfo.text = "[ ���� ]";
        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionCloseItemInfo") as ActionCloseItemInfo;
        closeItemInfo.actionDatas.Add(actionData);
        AddText(closeItemInfo);

        AddText(itemInfo.itemData.name);


        if (itemInfo.itemData.maxDurability <= 0)
        {
            AddText("������:" + itemInfo.mount);
        }
        else
        {
            AddText("������:" + itemInfo.mount + " ( �c���: " + (itemInfo.itemData.maxDurability - itemInfo.useCount) + " / " + itemInfo.itemData.maxDurability + " )");
        }
        AddText(itemInfo.itemData.infoText);

        if (itemInfo.itemData.isWeapon)
        {
            ActionInfo equipItemAction = new ActionInfo();
            equipItemAction.text = "[ ��������i����j ]";
            ActionData equipActionData = new ActionData();
            equipActionData.action = ScriptableObject.CreateInstance("ActionEquipWeapon") as ActionEquipWeapon;
            equipActionData.args = new ActionArgs();
            equipActionData.args.targetItemName = itemInfo.itemName;
            equipItemAction.actionDatas.Add(equipActionData);
            AddText(equipItemAction);

            //AddText(itemInfo.itemData.equipmentType);

        }

        if (itemInfo.itemData.isArmor)
        {
            ActionInfo equipItemAction = new ActionInfo();
            equipItemAction.text = "[ ��������i�h��j ]";
            ActionData equipActionData = new ActionData();
            equipActionData.action = ScriptableObject.CreateInstance("ActionEquipArmor") as ActionEquipArmor;
            equipActionData.args = new ActionArgs();
            equipActionData.args.targetItemName = itemInfo.itemName;
            equipItemAction.actionDatas.Add(equipActionData);
            AddText(equipItemAction);
        }

        if (itemInfo.itemData.isAccessory)
        {
            ActionInfo equipItemAction = new ActionInfo();
            equipItemAction.text = "[ ��������i�A�N�Z�T���j ]";
            ActionData equipActionData = new ActionData();
            equipActionData.action = ScriptableObject.CreateInstance("ActionEquipAccessory") as ActionEquipAccessory;
            equipActionData.args = new ActionArgs();
            equipActionData.args.targetItemName = itemInfo.itemName;
            equipItemAction.actionDatas.Add(equipActionData);
            AddText(equipItemAction);
        }

        if (itemInfo.itemData.isWeapon || itemInfo.itemData.isArmor || itemInfo.itemData.isAccessory)
        {
            AddText("<br>=���������Ƃ��̐��\=");
            AddText("����:" +itemInfo.itemData.equipmentTypeName);
            AddText("�^����_���[�W: " + itemInfo.itemData.minAtk + " �` " + itemInfo.itemData.maxAtk);
            if (itemInfo.itemData.consumeMp > 0)
            {
                AddText("�U�����̏���MP: " + itemInfo.itemData.consumeMp);
            }
            if (itemInfo.itemData.staminaConsumptionIncrease > 0)
            {
                AddText("�U�����̒ǉ�����X�^�~�i: " + itemInfo.itemData.staminaConsumptionIncrease);
            }
            if (itemInfo.itemData.avoidChance > 0)
            {
                AddText("�ǉ�����m��: " + (itemInfo.itemData.avoidChance * 100).ToString("0.0") + "%");
            }
            if (itemInfo.itemData.guardChance > 0)
            {
                AddText("�ǉ��K�[�h�m��: " + (itemInfo.itemData.guardChance * 100).ToString("0.0") + "%");
                AddText("�K�[�h�ɂ��y���_���[�W��: " + itemInfo.itemData.guardPoint);
            }
            if (itemInfo.itemData.poisonDamage> 0)
            {
                AddText("�łɂ��_���[�W: " + itemInfo.itemData.poisonDamage);
                AddText("�Ōp������: " + itemInfo.itemData.poisonDuration);
                AddText("�ŕt�^�m��: " + (itemInfo.itemData.poisonChance * 100).ToString("0.0") + "%");
            }
            if (itemInfo.itemData.additionalMaxHp != 0)
            {
                AddText("�ǉ�HP: " + itemInfo.itemData.additionalMaxHp);
            }
            if (itemInfo.itemData.additionalMaxMp != 0)
            {
                AddText("�ǉ�MP: " + itemInfo.itemData.additionalMaxMp);
            }
        }

        foreach (ActionInfo actionInfo in itemInfo.itemData.actionInfos)
        {
            AddText(actionInfo);
        }
    }

    public void OpenItemDetailInfo(ItemName itemName)
    {
        this.itemInfo = SaveDataManager.saveData.charaInfo.GetItemData(itemName);
        UpdateItemDetailInfo();

        ItemDetailInfo.SetActive(true);
    }

    public void CloseItemDetailInfo()
    {
        ItemDetailInfo.SetActive(false);
    }

    private void AddText(string text)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = text;
    }

    private void AddText(ActionInfo actionInfo)
    {
        GameObject temp = Instantiate(textPrefab, parent);
        temp.GetComponent<TextObject>().text = actionInfo.text;
        temp.GetComponent<TextObject>().color = actionInfo.color;
        temp.GetComponent<TextObject>().button.onClick.AddListener(
                () => {

                    bool isCanExecute = true;
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        if (!actionData.action.CheckCanExecute(actionData.args))
                        {
                            isCanExecute = false;
                            break;
                        }
                    }


                    if (isCanExecute)
                    {
                        foreach (ActionData actionData in actionInfo.actionDatas)
                        {
                            bool isContinue = actionData.action.ExecuteAction(actionData.args);
                            if (!isContinue) break;
                        }
                    }
                });
    }
}
