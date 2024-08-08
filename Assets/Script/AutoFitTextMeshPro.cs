using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AutoFitTextMeshPro : MonoBehaviour
{
    TextMeshProUGUI text;
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.OnPreRenderText += OnPreRenderText;
        rectTransform = GetComponent<RectTransform>();

        OnPreRenderText(text.textInfo);
        text.ForceMeshUpdate();
    }

    // Update is called once per frame
    void OnPreRenderText(TMP_TextInfo info)
    {
        float preferredHeight = info.textComponent.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    }
}
