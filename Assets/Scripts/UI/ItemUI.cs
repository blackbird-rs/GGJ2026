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
    public Image image;

    private ItemData itemData;
    private ClothingSlotUI currentSlot;
    private RectTransform imageRect;


    private int originalSpawnSlotIndex = -1;

    private Vector2 pointerOffset;

    private bool wasDroppedOnFigure;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        imageRect = image.rectTransform;

    }

    public void SetItemData(ItemData data)
    {
        itemData = data;
        image.sprite = data.icon;
        image.SetNativeSize();
    }

    public ItemData GetItemData() => itemData;

    public void SetOriginalSpawnSlot(int slotIndex)
    {
        originalSpawnSlotIndex = slotIndex;
    }

    public int GetOriginalSpawnSlot() => originalSpawnSlotIndex;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        canvasGroup.blocksRaycasts = false;
        wasDroppedOnFigure = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );

        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        if(currentSlot != null && currentSlot.additionalLayer != null){
            var color = currentSlot.additionalLayer.color; 
            color.a = 0f;
            currentSlot.additionalLayer.color = color;
        }

        rectTransform.localPosition = localPoint - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!wasDroppedOnFigure)
        {
            ReturnToOriginal();
        }
    }

    public void EquipTo(RectTransform target, ClothingSlotUI slot)
    {
        wasDroppedOnFigure = true;
        currentSlot = slot;

        if(itemData.additionalItem != null){
            slot.additionalLayer.sprite = itemData.additionalItem;
            var color = currentSlot.additionalLayer.color; 
            color.a = 1f;
            currentSlot.additionalLayer.color = color;
        }

        transform.SetParent(target, false);
        FitToParent();
    }

    public void ReturnToOriginal()
    {
        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }

        RectTransform spawnSlot =
            GameManager.Instance.itemSpawnSlots[originalSpawnSlotIndex];

        transform.SetParent(spawnSlot, false);
        FitToParent();
    }

    
public void FitToParent()
{
    RectTransform parent = rectTransform.parent as RectTransform;
    if (parent == null)
        return;

    Vector2 slotSize = parent.rect.size;
    Vector2 imageSize = imageRect.rect.size;

    if (imageSize.x <= 0 || imageSize.y <= 0)
        return;

    float scale = Mathf.Min(
        slotSize.x / imageSize.x,
        slotSize.y / imageSize.y
    );

    rectTransform.anchoredPosition = Vector2.zero;
    rectTransform.localScale = Vector3.one * scale;
}

}
