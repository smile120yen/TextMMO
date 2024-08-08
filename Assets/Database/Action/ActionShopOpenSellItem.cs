using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShopOpenSellItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenShopSellMenu");

        MapInfoWindowManager.Instance.additionalActions.Clear();

        foreach (ShopItemData shopItemData in args.shopDatabase.shopDatas)
        {
            ActionInfo actionInfo = new ActionInfo();
            actionInfo.text = "[ "+ shopItemData.itemData.name+ "‚ð1ŒÂ”„‚é("+shopItemData.price+"G) ]";

            ActionData removeItemAction = new ActionData();
            removeItemAction.action = new ActionItemAdjustment();
            removeItemAction.args = new ActionArgs();
            removeItemAction.args.targetItemName = shopItemData.itemName;
            removeItemAction.args.mount = -1;
            actionInfo.actionDatas.Add(removeItemAction);

            ActionData addMoneyAction = new ActionData();
            addMoneyAction.action = new ActionItemAdjustment();
            addMoneyAction.args = new ActionArgs();
            addMoneyAction.args.targetItemName = ItemName.money;
            addMoneyAction.args.mount = shopItemData.price;
            actionInfo.actionDatas.Add(addMoneyAction);

            MapInfoWindowManager.Instance.additionalActions.Add(actionInfo);
        }
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
