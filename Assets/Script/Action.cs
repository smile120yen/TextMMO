using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : ScriptableObject
{
    
    public virtual bool CheckCanExecute(ActionArgs args)
    {
        return true;
    }
    public virtual bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("execute action");
        return true;
    }
}

[System.Serializable]
public class ActionArgs
{
    public int targetID;
    public int mount;
    public MapName targetMapName;
    public ItemName targetItemName;
    public ItemData targetItemData {
        get
        {
            foreach (ItemData itemData in DatabaseManager.Instance.itemDatabase.itemDatas)
            {
                if (itemData.uniqueName == targetItemName.ToString()) return itemData;
            }
            return null;
        }
    }
    public List<ItemInfo> consumeItemInfo;
    public List<ItemInfo> getItemInfo;
    public bool forceExecute;
    //public CharaInfo charaInfo;
    public ShopDatabase shopDatabase;
    public TradeDatabase tradeDatabase;
    public float eventProbability;
    public string userId;
    public string emoteMessage;
    public AttackBuff attackBuff;
    public float attackBuffDuration;
    public JobName targetJob;
    public List<JobChangeData> jobChangeDatas;
    public string key;
    public ActionArgs()
    {
        consumeItemInfo = new List<ItemInfo>();
        getItemInfo = new List<ItemInfo>();
    }
}