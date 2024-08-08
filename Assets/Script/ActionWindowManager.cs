using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionWindowManager : MonoBehaviour
{
    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform parent;
    
    private void Start()
    {
        UpdateAction();
        SaveDataManager.OnSaveDataUpdate.AddListener(() => UpdateAction());
    }

    private void UpdateAction()
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        MapInfo mapInfo = SaveDataManager.saveData.mapInfo;

        foreach (ActionInfo actionInfo in mapInfo.mapData.mapMovementActions)
        {
            GameObject temp = Instantiate(textPrefab, parent);
            temp.GetComponent<TextObject>().text = actionInfo.text;
            temp.GetComponent<TextObject>().button.onClick.AddListener(
                () => {
                    foreach (ActionData actionData in actionInfo.actionDatas)
                    {
                        bool isContinue = actionData.action.ExecuteAction(actionData.args);
                        if (!isContinue) break;
                    }
                });
        }
    }

}
