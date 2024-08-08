using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOpenJobChange : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("ActionShopOpenJobChange");

        MapInfoWindowManager.Instance.additionalActions.Clear();


        ActionInfo space = new ActionInfo();
        space.text = " ";
        MapInfoWindowManager.Instance.additionalActions.Add(space);

        foreach (JobChangeData jobChangeData in args.jobChangeDatas)
        {
            ActionInfo discription = new ActionInfo();
            discription.text = jobChangeData.jobData.name + "<br>" + jobChangeData.jobData.discription;
            MapInfoWindowManager.Instance.additionalActions.Add(discription);

            ActionInfo actionInfo = new ActionInfo();
            actionInfo.text = "[ "+ jobChangeData.jobData.name +"Ç…ì]êE ("+ jobChangeData.price +"G) ]<br> ";

            ActionData removeMoneyAction = new ActionData();
            removeMoneyAction.action = ScriptableObject.CreateInstance("ActionItemAdjustment") as ActionItemAdjustment;
            removeMoneyAction.args = new ActionArgs();
            removeMoneyAction.args.targetItemName = ItemName.money;
            removeMoneyAction.args.mount = -jobChangeData.price;
            actionInfo.actionDatas.Add(removeMoneyAction);

            ActionData changeJobAction = new ActionData();
            changeJobAction.action = ScriptableObject.CreateInstance("ActionChangeJob") as ActionChangeJob;
            changeJobAction.args = new ActionArgs();
            changeJobAction.args.targetJob = jobChangeData.jobName;
            actionInfo.actionDatas.Add(changeJobAction);

            MapInfoWindowManager.Instance.additionalActions.Add(actionInfo);
        }
        
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
