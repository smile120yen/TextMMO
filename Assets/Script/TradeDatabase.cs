using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "TradeDatabase", menuName = "Database/TradeDatabase")]
public class TradeDatabase : ScriptableObject
{
    [SerializeField]
    List<TradeItemData> _tradeDatas = new List<TradeItemData>();
    public List<TradeItemData> tradeDatas => _tradeDatas;
}

[System.Serializable]
public class TradeItemData
{
    public List<ItemInfo> consumeItemInfos;
    public List<ItemInfo> getItemInfos;
}