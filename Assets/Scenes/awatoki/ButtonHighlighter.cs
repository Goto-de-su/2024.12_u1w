using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Vector3 defaultScale;
    private Image buttonImage;
    public Color highlightColor = Color.yellow;

    void Start()
    {
        defaultScale = transform.localScale;
        buttonImage = GetComponent<Image>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = defaultScale * 1.2f; // 選択されたら拡大
        if (buttonImage != null)
            buttonImage.color = highlightColor; // 色を変更
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = defaultScale; // 元のサイズに戻す
        if (buttonImage != null)
            buttonImage.color = Color.white; // 色を元に戻す
    }
}
