using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOpenAttackMenu : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenAttackMenu");

        MapInfoWindowManager.Instance.additionalActions.Clear();

        ActionInfo actionInfo = new ActionInfo();
        actionInfo.text = "[ ìSçzêŒÇ1å¬îÑÇÈ(8G) ]";

        ActionData removeItemAction = new ActionData();
        removeItemAction.action = new ActionItemAdjustment();
        removeItemAction.args = new ActionArgs();
        removeItemAction.args.targetItemName = ItemName.iron_ore;
        removeItemAction.args.mount = -1;
        actionInfo.actionDatas.Add(removeItemAction);

        ActionData addMoneyAction = new ActionData();
        addMoneyAction.action = new ActionItemAdjustment();
        addMoneyAction.args = new ActionArgs();
        addMoneyAction.args.targetItemName = ItemName.money;
        addMoneyAction.args.mount = 8;
        actionInfo.actionDatas.Add(addMoneyAction);

        MapInfoWindowManager.Instance.additionalActions.Add(actionInfo);
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
