using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStaminaAdjustment : Action
{
    public override bool CheckCanExecute(ActionArgs args)
    {
        if (SaveDataManager.saveData.charaInfo.currentStamina + args.mount < 0)
        {
            ChatMenuManager.Instance.AddText(">�X�^�~�i������܂���i�X�^�~�i��" + (-1 * args.mount) + "�K�v�j");
            return false;
        }

        return true;
    }

    public override bool ExecuteAction(ActionArgs args)
    {      
        Debug.Log("Stamina Ajastment:"+args.mount);
        SaveDataManager.saveData.charaInfo.currentStamina += args.mount;
        SaveDataManager.Save();

        return true;
    }
}
