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

    private ItemData itemData;
    private ClothingSlotUI currentSlot;

    private int originalSpawnSlotIndex = -1;

    private Vector2 pointerOffset;

    private bool wasDroppedOnFigure;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;
        image.sprite = data.icon;
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

        transform.SetParent(target, false);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    public void ReturnToOriginal()
    {
        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }

        RectTransform spawnSlot =
            GameManager.instance.itemSpawnSlots[originalSpawnSlotIndex];

        transform.SetParent(spawnSlot, false);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }
}
