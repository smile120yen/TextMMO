using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ActionStaminaAdjustmentWithEquipBonus : Action
{
    int mount, bonus;

    public override bool CheckCanExecute(ActionArgs args)
    {
        bonus = 0;
        bonus -= SaveDataManager.saveData.charaInfo.equipItemHand.itemData.staminaConsumptionIncrease;
        bonus -= SaveDataManager.saveData.charaInfo.equipItemBody.itemData.staminaConsumptionIncrease;
        mount = args.mount + bonus;

        if (SaveDataManager.saveData.charaInfo.currentStamina + mount < 0)
        {
            ChatMenuManager.Instance.AddText(">スタミナが足りません（スタミナが" + (-1 * mount) + "必要）");
            return false;
        }

        return true;
    }

    public override bool ExecuteAction(ActionArgs args)
    {   

        Debug.Log("Stamina Ajastment With Bonus default:"+args.mount+",bonus:"+bonus+",mount:"+ mount);
        SaveDataManager.saveData.charaInfo.currentStamina += mount;
        SaveDataManager.Save();

        return true;
    }
}
*/