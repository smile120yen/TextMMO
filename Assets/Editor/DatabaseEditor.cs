using UnityEditor;
using UnityEngine;

// �g������N���X���w�肷��
[CustomEditor(typeof(Database),true)]
// �p���N���X�� Editor ��ݒ肷��
public class DatabaseEditor : Editor
{
    // GUI�̕\���֐����I�[�o�[���C�h����
    public override void OnInspectorGUI()
    {
        // target��ϊ����đΏۃX�N���v�g�̎Q�Ƃ��擾����
        Database transformPaste = target as Database;
        // public�֐������s����{�^���̍쐬
        if (GUILayout.Button("Enum��������"))
        {
            transformPaste.createEnum();
        }
        // ���̃C���X�y�N�^�[������\��
        base.OnInspectorGUI();
    }
}