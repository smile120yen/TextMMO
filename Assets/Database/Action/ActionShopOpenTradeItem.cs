using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShopOpenTradeItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("OpenShopTradeItem");

        MapInfoWindowManager.Instance.additionalActions.Clear();

        foreach (TradeItemData tradeItemData in args.tradeDatabase.tradeDatas)
        {
            ActionInfo actionInfo = new ActionInfo();

            string actionText = "[ ";
            bool init = false;
            foreach(ItemInfo itemIfo in tradeItemData.consumeItemInfos)
            {
                if (init)
                {
                    actionText += "‚Æ";
                }
                init = true;
                actionText += itemIfo.itemData.name;
                actionText += itemIfo.mount+"ŒÂ";
            }

            actionText += " => ";

            init = false;
            foreach (ItemInfo itemIfo in tradeItemData.getItemInfos)
            {
                if (init)
                {
                    actionText += "‚Æ";
                }
                init = true;
                actionText += itemIfo.itemData.name;
                actionText += itemIfo.mount + "ŒÂ";
            }

            actionText += " ]";
            actionInfo.text = actionText;

            ActionData tradeAction = new ActionData();
            tradeAction.action = new ActionTradeItem();
            tradeAction.args = new ActionArgs();

            foreach (ItemInfo itemInfo in tradeItemData.consumeItemInfos)
            {
                ItemInfo consumeItem = new ItemInfo(itemInfo.itemName);
                consumeItem.mount = itemInfo.mount;

                tradeAction.args.consumeItemInfo.Add(consumeItem);
            }

            foreach (ItemInfo itemInfo in tradeItemData.getItemInfos)
            {
                ItemInfo getItem = new ItemInfo(itemInfo.itemName);
                getItem.mount = itemInfo.mount;

                tradeAction.args.getItemInfo.Add(getItem);
            }

            actionInfo.actionDatas.Add(tradeAction);

            MapInfoWindowManager.Instance.additionalActions.Add(actionInfo);
        }
        MapInfoWindowManager.Instance.UpdateMapInfo();

        return true;
    }
}
