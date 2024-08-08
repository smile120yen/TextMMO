using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShopOpenBuyItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenShopBuyMenu");

        MapInfoWindowManager.Instance.additionalActions.Clear();

        foreach(ShopItemData shopItemData in args.shopDatabase.shopDatas)
        {
            ActionInfo actionInfo = new ActionInfo();
            actionInfo.text = "[ "+ shopItemData.itemData.name +"‚ð”ƒ‚¤("+shopItemData.price+"G) ]";

            ActionData removeMoneyAction = new ActionData();
            removeMoneyAction.action = ScriptableObject.CreateInstance("ActionItemAdjustment") as ActionItemAdjustment;
            removeMoneyAction.args = new ActionArgs();
            removeMoneyAction.args.targetItemName = ItemName.money;
            removeMoneyAction.args.mount = -shopItemData.price;
            actionInfo.actionDatas.Add(removeMoneyAction);

            ActionData addItemAction = new ActionData();
            addItemAction.action = ScriptableObject.CreateInstance("ActionItemAdjustment") as ActionItemAdjustment;
            addItemAction.args = new ActionArgs();
            addItemAction.args.targetItemName = shopItemData.itemName;
            addItemAction.args.mount = 1;
            actionInfo.actionDatas.Add(addItemAction);

            MapInfoWindowManager.Instance.additionalActions.Add(actionInfo);
        }
        
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
