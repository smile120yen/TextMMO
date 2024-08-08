using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;

public static class SaveDataManager
{
    private static SaveData _saveData;
    private static string saveDataPath = Application.persistentDataPath + "/user.savedata";
    public static UnityEvent OnSaveDataUpdateBefore = new UnityEvent();
    public static UnityEvent OnSaveDataUpdate = new UnityEvent();

    public static SaveData saveData
    {
        get
        {
            //ロードする
            if (_saveData == null)
            {
                if (File.Exists(saveDataPath))
                {
                    Debug.Log("Load save data file:" + saveDataPath);
                    string json = File.ReadAllText(saveDataPath);
                    _saveData = JsonUtility.FromJson<SaveData>(json);
                }
                else
                {
                    _saveData = new SaveData();
                    Save();
                }
            }
            return _saveData;
        }
        set
        {
            _saveData = value;
        }
    }

    public static void Save()
    {
        Debug.Log("file saved:" + saveDataPath);
        string json = JsonUtility.ToJson(_saveData);
        File.WriteAllText(saveDataPath, json);
        //中身系
        OnSaveDataUpdateBefore.Invoke();
        //見た目系
        OnSaveDataUpdate.Invoke();
    }

    public static string GetJson()
    {
        return JsonUtility.ToJson(_saveData);
    }

    public static void RemoveSaveDataFile()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }
    }
}


[System.Serializable]
public class SaveData
{
    public CharaInfo charaInfo;
    public MapInfo mapInfo;

    public SaveData()
    {
        charaInfo = new CharaInfo();
        mapInfo = new MapInfo();
    }
}

[System.Serializable]
public class CharaInfo
{
    public string name = null;
    public List<ItemInfo> inventory;
    //public int lv;
    //public int exp;
    public int hp;
    public int maxHp
    {
        get
        {
            int hp = GetCurrentJobInfo().maxHp
                + itemDataEquipBody.additionalMaxHp
                + itemDataEquipHand.additionalMaxHp
                + itemDataEquipAccessory.additionalMaxHp;
            if (hp < 1) hp = 1;
            return hp;
        }
    }
    public int mp;
    public int maxMp
    {
        get
        {
            int mp = GetCurrentJobInfo().maxMp
                + itemDataEquipBody.additionalMaxMp
                + itemDataEquipHand.additionalMaxMp
                + itemDataEquipAccessory.additionalMaxMp;
            if (mp < 0) mp = 0;
            return mp;
        }
    }
    //public int nextMaxExp;
    public int currentStamina;
    public int maxStamina;
    public List<JobInfo> jobInfos;
    public JobName currentJob;
    public SerializableDateTime charaCreateTime;
    public List<string> endingflag;

    public ItemName itemNameEquipHand;
    public ItemData itemDataEquipHand
    {
        get
        {
            foreach (ItemData itemData in DatabaseManager.Instance.itemDatabase.itemDatas)
            {
                if (itemData.uniqueName == itemNameEquipHand.ToString()) return itemData;
            }
            return null;
        }
    }
    public ItemName itemNameEquipBody;

    public ItemData itemDataEquipBody
    {
        get
        {
            foreach (ItemData itemData in DatabaseManager.Instance.itemDatabase.itemDatas)
            {
                if (itemData.uniqueName == itemNameEquipBody.ToString()) return itemData;
            }
            return null;
        }
    }

    public ItemName itemNameEquipAccessory;

    public ItemData itemDataEquipAccessory
    {
        get
        {
            foreach (ItemData itemData in DatabaseManager.Instance.itemDatabase.itemDatas)
            {
                if (itemData.uniqueName == itemNameEquipAccessory.ToString()) return itemData;
            }
            return null;
        }
    }

    public List<AttackBuffInfo> attackBuffs;
    public MapInfo respawnPoint;

    public CharaInfo()
    {
        inventory = new List<ItemInfo>();
        ItemInfo returnRing = new ItemInfo(ItemName.return_ring);
        returnRing.mount = 1;
        inventory.Add(returnRing);
        /*
        lv = 1;
        exp = 0;
        nextMaxExp = 5;
        maxHp = 10;
        maxMp = 10;
        */
        currentStamina = 20;
        maxStamina = 20;
        hp = 10;
        mp = 10;
        itemNameEquipHand = ItemName.fist;
        itemNameEquipBody = ItemName.skin;
        itemNameEquipAccessory = ItemName.finger;
        attackBuffs = new List<AttackBuffInfo>();
        respawnPoint = new MapInfo();
        respawnPoint.mapName = MapName.Home;
        jobInfos = new List<JobInfo>();
        currentJob = JobName.None;
        charaCreateTime = DateTime.Now;
        endingflag = new List<string>();
    }

    public ItemInfo GetItemData(ItemName itemName)
    {
        ItemInfo info = inventory.Find( s => s.itemName == itemName);
        if (info == null)
        {
            info = new ItemInfo(itemName);
            inventory.Add(info);
        }
        return info;
    }

    public bool AddItemToInventory(ItemName itemName,int mount)
    {
        ItemInfo itemInfo = GetItemData(itemName);
        if(itemInfo.mount + mount < 0)
        {
            return false;
        }
        itemInfo.mount += mount;
        return true;
    }

    public JobInfo GetCurrentJobInfo()
    {
        JobInfo result=null;

        foreach (JobInfo jobInfo in jobInfos)
        {
            if(jobInfo.jobName == currentJob)
            {
                result = jobInfo;
                break;
            }
        }

        if (result == null)
        {
            result = new JobInfo(currentJob);
            jobInfos.Add(result);
        }
        return result;
    }
}

[System.Serializable]
public class ItemInfo
{
    public ItemName itemName;
    public int mount;
    public int useCount;
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

    public ItemInfo(ItemName itemName)
    {
        this.itemName = itemName;
        mount = 0;
    }
}


[System.Serializable]
public class MapInfo
{
    public MapName mapName;
    public MapData mapData
    {
        get
        {
            foreach (MapData mapData in DatabaseManager.Instance.mapDatabase.mapDatas)
            {
                if (mapData.uniqueName == mapName.ToString()) return mapData;
            }
            foreach (MapData mapData in DatabaseManager.Instance.mapDatabase.mapDatas)
            {
                if (mapData.uniqueName == MapName.Home.ToString()) return mapData;
            }
            return null;
        }
    }
    public MapInfo()
    {
        mapName = MapName.Home;
    }
}


[System.Serializable] 
public class AttackBuffInfo
{
    public SerializableDateTime start;
    public SerializableDateTime end;
    public AttackBuff attackBuff;

    public AttackBuffInfo(float durationSecond,AttackBuff attackBuff)
    {
        this.start = DateTime.Now;
        this.end = DateTime.Now.AddSeconds(durationSecond);
        this.attackBuff = attackBuff;
    }
}


[System.Serializable]
public class AttackBuff
{
    public string uniqueKey;
    public string name;
    public int mount;
}

[System.Serializable]
public class JobInfo{
    public JobName jobName;
    public JobData jobData
    {
        get
        {
            //ここがメインスレッド外から呼び出されるパターンがある
            foreach (JobData jobData in DatabaseManager.Instance.jobDatabase.jobDatas)
            {
                if (jobData.jobName.ToString() == jobName.ToString()) return jobData;
            }
            return null;
        }
    }
    public int lv;
    public int exp;
    public int maxHp;
    public int maxMp;
    public int nextMaxExp;

    public JobInfo(JobName jobName)
    {
        this.jobName = jobName;
        this.lv = 1;
        this.exp = 0;
        this.maxHp = jobData.initialMaxHp;
        this.maxMp = jobData.initialMaxMp;
        this.nextMaxExp = 3;
    }

}