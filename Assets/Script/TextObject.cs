using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextObject : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] TextMeshProUGUI parenttext;
    //[SerializeField] TextMeshProUGUI childText;
    public string text
    {
        set
        {
            parenttext.text = value;
            //childText.text = value;
        }
    }
    Graphic graphic;
    Camera camera;
    RectTransform rect;
    //RectTransform childRect;
    [SerializeField] Vector2 screenPos;
    [SerializeField] float y;
    [SerializeField] GameObject child;

    ContentSizeFitter contentSizeFitter;

    public Color color
    {
        set
        {
            parenttext.color = value;
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);

        graphic = GetComponent<Graphic>();
        camera = graphic.canvas.worldCamera;
        rect = GetComponent<RectTransform>();
        //childRect = child.GetComponent<RectTransform>();
    }

    private Vector3 cashPosition;

    
    void LateUpdate()
    {
        Canvas.ForceUpdateCanvases();

        cashPosition = transform.position;
        screenPos = RectTransformUtility.WorldToScreenPoint(camera, cashPosition);
        //screenPos.y = Mathf.Floor(screenPos.y) + 0.5f;

        //RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, camera, out Vector3 worldPoint);
        //transform.position = worldPoint;

        y = Mathf.Floor(screenPos.y) - screenPos.y + 0.5f;
        parenttext.margin = new Vector4(0, -y, 0,0);
    }


    public void OnClick()
    {
        SoundManager.Instance.Play(SoundName.ti);
    }
}
