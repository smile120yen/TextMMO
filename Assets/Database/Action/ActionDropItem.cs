using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActionDropItem : Action
{
    public override bool ExecuteAction(ActionArgs args)
    {
        Debug.Log("ActionDropItem:" + args.targetItemName.ToString());

        

        float random = Random.Range(0, 1.0f);

        if(random > args.eventProbability)
        {
            //�C�x���g���������Ȃ�
            return false;
        }

        ItemData itemData = args.targetItemData;
        SaveDataManager.saveData.charaInfo.AddItemToInventory(args.targetItemName, 1);

        int count = SaveDataManager.saveData.charaInfo.GetItemData(args.targetItemName).mount;

        ChatMenuManager.Instance.AddText(">���b�L�[�I" + args.targetItemData.name + "����ɓ��ꂽ�I(����"+ count+ "��)",Color.yellow);

        SaveDataManager.Save();

        return true;
    }
}
