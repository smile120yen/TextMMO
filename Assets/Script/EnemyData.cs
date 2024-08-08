using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyDataObject", menuName = "DataObject/EnemyDataObject")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int enemyMaxHp;
    public int enemyAtk;
    public List<DropItemData> dropItemDatas;
    public List<ActionData> deathActions;
}

[System.Serializable]
public class DropItemData
{
    public ItemName itemName;
    public ItemData itemData
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
    public float dropChance;
    public int dropMaxMount;
    public int dropMinMount;

    DropItemData()
    {
        itemName = ItemName.None;
    }
}