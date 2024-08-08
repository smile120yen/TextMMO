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
        closeItemInfo.text = "[ 閉じる ]";
        ActionData actionData = new ActionData();
        actionData.action = ScriptableObject.CreateInstance("ActionCloseItemInfo") as ActionCloseItemInfo;
        closeItemInfo.actionDatas.Add(actionData);
        AddText(closeItemInfo);

        AddText(itemInfo.itemData.name);


        if (itemInfo.itemData.maxDurability <= 0)
        {
            AddText("所持数:" + itemInfo.mount);
        }
        else
        {
            AddText("所持数:" + itemInfo.mount + " ( 残り回数: " + (itemInfo.itemData.maxDurability - itemInfo.useCount) + " / " + itemInfo.itemData.maxDurability + " )");
        }
        AddText(itemInfo.itemData.infoText);

        if (itemInfo.itemData.isWeapon)
        {
            ActionInfo equipItemAction = new ActionInfo();
            equipItemAction.text = "[ 装備する（武器） ]";
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
            equipItemAction.text = "[ 装備する（防具） ]";
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
            equipItemAction.text = "[ 装備する（アクセサリ） ]";
            ActionData equipActionData = new ActionData();
            equipActionData.action = ScriptableObject.CreateInstance("ActionEquipAccessory") as ActionEquipAccessory;
            equipActionData.args = new ActionArgs();
            equipActionData.args.targetItemName = itemInfo.itemName;
            equipItemAction.actionDatas.Add(equipActionData);
            AddText(equipItemAction);
        }

        if (itemInfo.itemData.isWeapon || itemInfo.itemData.isArmor || itemInfo.itemData.isAccessory)
        {
            AddText("<br>=装備したときの性能=");
            AddText("分類:" +itemInfo.itemData.equipmentTypeName);
            AddText("与えるダメージ: " + itemInfo.itemData.minAtk + " ～ " + itemInfo.itemData.maxAtk);
            if (itemInfo.itemData.consumeMp > 0)
            {
                AddText("攻撃時の消費MP: " + itemInfo.itemData.consumeMp);
            }
            if (itemInfo.itemData.staminaConsumptionIncrease > 0)
            {
                AddText("攻撃時の追加消費スタミナ: " + itemInfo.itemData.staminaConsumptionIncrease);
            }
            if (itemInfo.itemData.avoidChance > 0)
            {
                AddText("追加回避確率: " + (itemInfo.itemData.avoidChance * 100).ToString("0.0") + "%");
            }
            if (itemInfo.itemData.guardChance > 0)
            {
                AddText("追加ガード確率: " + (itemInfo.itemData.guardChance * 100).ToString("0.0") + "%");
                AddText("ガードによる軽減ダメージ量: " + itemInfo.itemData.guardPoint);
            }
            if (itemInfo.itemData.poisonDamage> 0)
            {
                AddText("毒によるダメージ: " + itemInfo.itemData.poisonDamage);
                AddText("毒継続時間: " + itemInfo.itemData.poisonDuration);
                AddText("毒付与確率: " + (itemInfo.itemData.poisonChance * 100).ToString("0.0") + "%");
            }
            if (itemInfo.itemData.additionalMaxHp != 0)
            {
                AddText("追加HP: " + itemInfo.itemData.additionalMaxHp);
            }
            if (itemInfo.itemData.additionalMaxMp != 0)
            {
                AddText("追加MP: " + itemInfo.itemData.additionalMaxMp);
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
