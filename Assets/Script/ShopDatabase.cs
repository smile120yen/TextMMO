using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "ShopDatabase", menuName = "Database/ShopDatabase")]
public class ShopDatabase : Database
{
    [SerializeField]
    List<ShopItemData> _shopDatas = new List<ShopItemData>();
    public List<ShopItemData> shopDatas => _shopDatas;
     
    public override void createEnum()
    {
    }
}

[System.Serializable]
public class ShopItemData
{
    public ItemName itemName;
    public int price;

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
}