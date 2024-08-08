using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowControll : MonoBehaviour
{
    [SerializeField] GameObject target;

    public void Open()
    {
        target.SetActive(true);
        SoundManager.Instance.Play(SoundName.ti);
    }

    public void Close()
    {
        target.SetActive(false);
        SoundManager.Instance.Play(SoundName.ti);
    }
}
