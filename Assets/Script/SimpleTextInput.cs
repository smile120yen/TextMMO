using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class SimpleTextInput : MonoBehaviour
{
    [SerializeField] UnityEvent<string> OnTextSend;
    TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void OnSubmit(string str)
    {
        OnTextSend.Invoke(inputField.text);
        inputField.text = "";
        inputField.ActivateInputField();
    }

}
