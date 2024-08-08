using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndingWindowManager : SingletonMonoBehaviour<EndingWindowManager>
{
    [SerializeField] public List<EndingData> endingDatas;

    public void OpenEndingWindow(string key)
    {
        TimeSpan time = DateTime.Now - SaveDataManager.saveData.charaInfo.charaCreateTime.Value;
        EndingData endingData = endingDatas.Find(s => s.key == key);

        if (endingData != null)
        {
            endingData.window.SetActive(true);
            endingData.clearTimeText.text = "クリアタイム:" + (int)time.TotalHours + "時間" + time.Minutes + "分" + time.Seconds + "." + (time.Milliseconds/10).ToString("00");
        }

        endingData.flavorText.text = endingData.flavorText.text.Replace("[name]", SaveDataManager.saveData.charaInfo.name);
    }
}

[System.Serializable]
public class EndingData
{
    public string key;
    public GameObject window;
    public TextMeshProUGUI clearTimeText;
    public TextMeshProUGUI flavorText;
}