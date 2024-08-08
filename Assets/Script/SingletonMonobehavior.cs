//  SingletonMonoBehaviour.cs
//  http://kan-kikuchi.hatenablog.com/entry/SingletonMonoBehaviour
//
//  Created by kan.kikuchi on 2016.04.19.

using UnityEngine;

/// <summary>
/// MonoBehaviour���p�����A���������\�b�h��������V���O���g���ȃN���X
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviourWithInit where T : MonoBehaviourWithInit
{

    //�C���X�^���X
    private static T _instance;

    //�C���X�^���X���O������Q�Ƃ���p(getter)
    public static T Instance
    {
        get
        {
            //�C���X�^���X���܂�����Ă��Ȃ�
            if (_instance == null)
            {

                //�V�[��������C���X�^���X���擾
                _instance = (T)FindObjectOfType(typeof(T));

                //�V�[�����ɑ��݂��Ȃ��ꍇ�̓G���[
                if (_instance == null)
                {
                    Debug.LogError(typeof(T) + " is nothing");
                }
                //���������ꍇ�͏�����
                else
                {
                    _instance.InitIfNeeded();
                }
            }
            return _instance;
        }
    }

    //=================================================================================
    //������
    //=================================================================================

    protected sealed override void Awake()
    {
        //���݂��Ă���C���X�^���X�������ł���Ζ��Ȃ�
        if (this == Instance)
        {
            return;
        }

        //��������Ȃ��ꍇ�͏d�����đ��݂��Ă���̂ŁA�G���[
        Debug.LogError(typeof(T) + " is duplicated");
    }

}

/// <summary>
/// ���������\�b�h�������MonoBehaviour
/// </summary>
public class MonoBehaviourWithInit : MonoBehaviour
{

    //�������������ǂ����̃t���O(��x����������������Ȃ��悤�ɂ��邽��)
    private bool _isInitialized = false;

    /// <summary>
    /// �K�v�Ȃ珉��������
    /// </summary>
    public void InitIfNeeded()
    {
        if (_isInitialized)
        {
            return;
        }
        Init();
        _isInitialized = true;
    }

    /// <summary>
    /// ������(Awake�������̑O�̏��A�N�Z�X�A�ǂ��炩�̈�x�����s���Ȃ�)
    /// </summary>
    protected virtual void Init() { }

    //sealed override���邽�߂�virtual�ō쐬
    protected virtual void Awake() { }

}