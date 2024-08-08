using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/ItemDatabase")]
public class ItemDatabase : Database
{
    [SerializeField]
    List<ItemData> _itemDatas = new List<ItemData>();
    public List<ItemData> itemDatas => _itemDatas;

    public override void createEnum()
    {
        //Enumの項目をstring、その数値をintでまとめる
        Dictionary<string, int> itemDict = new Dictionary<string, int>();

        foreach (ItemData itemData in itemDatas)
        {
            if (Enum.TryParse(itemData.uniqueName, out ItemName result))
            {
                itemDict.Add(itemData.uniqueName, (int)result);
            }
            else
            {
                // 失敗した場合こっち
                // ここで、今Enumにも、itemDictの中にもない数値を新しい数値として与える
                int count = 0;
                while (true)
                {

                    Debug.Log(itemData.uniqueName + "," + count);
                    bool isFind = false;
                    foreach (ItemName value in Enum.GetValues(typeof(ItemName)))
                    {
                        Debug.Log((int)value);
                        if (count == (int)value)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    foreach (KeyValuePair<string, int> keyValuePair in itemDict)
                    {
                        Debug.Log((int)keyValuePair.Value);
                        if (count == (int)keyValuePair.Value)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        break;
                    }

                    count++;

                }

                itemDict.Add(itemData.uniqueName, count);
            }

        }
#if UNITY_EDITOR
        //Enum作成
        EnumCreator.Create(
            enumName: "ItemName",    //enumの名前
            itemDict: itemDict,                //enumの項目
            exportPath: "Assets/Database/Enum/ItemName.cs"  //作成したファイルのパスをAssetsから拡張子まで指定
        );
#endif

    }
}

[System.Serializable]
public class ItemData
{
    [SerializeField] public string name;
    [SerializeField] public string uniqueName;
    [SerializeField] public string infoText;
    [SerializeField] public List<ActionInfo> actionInfos;
    [SerializeField] public List<ActionData> equipAttackActions;
    [SerializeField] public List<ActionData> equipDefenceActions;
    [SerializeField] public List<ActionData> equipItemBonusActions;
    [SerializeField] public List<ItemBonus> itemBonuses;
    [SerializeField] public EquipmentType equipmentType;
    [SerializeField] public bool isWeapon;
    [SerializeField] public bool isArmor;
    [SerializeField] public bool isAccessory;
    [SerializeField] public int minAtk;
    [SerializeField] public int maxAtk;
    [SerializeField] public float avoidChance;
    [SerializeField] public float guardChance;
    [SerializeField] public int guardPoint;
    [SerializeField] public int staminaConsumptionIncrease;
    [SerializeField] public int consumeMp;
    [SerializeField] public int maxDurability;
    [SerializeField] public float poisonChance;
    [SerializeField] public int poisonDuration;
    [SerializeField] public int poisonDamage;
    [SerializeField] public int additionalMaxHp;
    [SerializeField] public int additionalMaxMp;

    public string equipmentTypeName
    {
        get
        {
            switch (equipmentType)
            {
                case EquipmentType.None:
                    return "分類なし";
                case EquipmentType.Sword:
                    return "剣";
                case EquipmentType.Mace:
                    return "槌";
                case EquipmentType.Wand:
                    return "杖";
                case EquipmentType.Bow:
                    return "弓";
            }
            return "未定義";
        }
    }
}

[System.Serializable]
public class ItemBonus
{
    public ItemName itemName;
    public ItemData ItemData
    {
        get
        {
            foreach (ItemData itemData in DatabaseManager.Instance.itemDatabase.itemDatas)
            {
                if (itemData.uniqueName == itemName.ToString()) return itemData;
            }
            return null;
        }
    }
    public float chance;
    public int bonusMax;
    public int bonusMin;
}

public enum EquipmentType
{
    None = 0,
    Sword = 1,
    Wand = 2,
    Mace = 3,
    Bow = 4
}