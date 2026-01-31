using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ItemUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image image;
    private string itemId;

    private Transform originalParent;
    private Vector2 originalPosition;

    private bool wasDroppedOnFigure;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void SetItemData(ItemData data){
        image.sprite = data.icon;
        itemId = data.itemId;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        wasDroppedOnFigure = false;

        // Move to top-level canvas so it follows cursor cleanly
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!wasDroppedOnFigure)
        {
            ReturnToOriginal();
        }
    }

    public void EquipTo(Transform target)
    {
        wasDroppedOnFigure = true;
        transform.SetParent(target);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void ReturnToOriginal()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    public string getItemId(){
        return itemId;
    }
}
