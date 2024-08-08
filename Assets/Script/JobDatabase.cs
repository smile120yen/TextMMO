using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "JobDatabase", menuName = "Database/JobDatabase")]
public class JobDatabase : Database
{
    [SerializeField]
    List<JobData> _jobDatas = new List<JobData>();
    public List<JobData> jobDatas => _jobDatas;

    public override void createEnum()
    {
        Debug.LogError("Cant Create JobDatabase Enum. Plese Edit JobDatabase Script.");
    }
}

[System.Serializable]
public class JobData
{
    public JobName jobName;
    public string name;
    public string discription;
    public int lvupAddHp;
    public int lvupAddMp;
    public int initialMaxHp;
    public int initialMaxMp;
    public List<MultipleWithType> damageMultipleWithTypes;
    public List<MultipleWithType> staminaConsumeMultipleWithTypes;
    public float guardChanceMultiple;
    public float avoidChanceMultiple;
}

[System.Serializable]
public class MultipleWithType
{
    public EquipmentType equipmentType;
    public float multiple;
}

[System.Serializable]
public enum JobName
{
    None = 0,
    SwordMan = 1,
    Magician = 2,
    Fairy = 3
}