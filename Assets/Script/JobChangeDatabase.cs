using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
[System.Serializable]
[CreateAssetMenu(fileName = "JobChangeDatabase", menuName = "Database/JobChangeDatabase")]
public class JobChangeDatabase : Database
{
    [SerializeField]
    List<ShopItemData> _Job = new List<ShopItemData>();
    public List<ShopItemData> shopDatas => _shopDatas;
     
    public override void createEnum()
    {
    }
}*/


[System.Serializable]
public class JobChangeData
{
    public JobName jobName;
    public JobData jobData{
        get{
            foreach (JobData jobData in DatabaseManager.Instance.jobDatabase.jobDatas)
            {
                if (jobData.jobName == jobName) return jobData;
            }
            return null;
        }
    }
    public int price;
}