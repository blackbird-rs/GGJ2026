using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ItemUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerEnterHandler,
    ICanvasRaycastFilter
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Image image;
    public Animation shakeAnim;
    private bool blockShake;

    private ItemData itemData;
    private ClothingSlotUI currentSlot;
    private RectTransform imageRect;


    private int originalSpawnSlotIndex = -1;
    private RectTransform originalSpawnSlot;

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
        if(data.clothingItem == null){
            image.sprite = data.icon;
        }
        else{
            image.sprite = data.clothingItem;
        }
        image.SetNativeSize();
    }

    public ItemData GetItemData() => itemData;

    public void SetOriginalSpawnSlot(int slotIndex, RectTransform slot)
    {
        originalSpawnSlotIndex = slotIndex;
        originalSpawnSlot = slot;
    }

    public int GetOriginalSpawnSlot() => originalSpawnSlotIndex;

    public void OnBeginDrag(PointerEventData eventData)
    {
        JudgyAudioManager.Instance.Take();

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

        if(itemData.clothingItem != null) {
            image.sprite = itemData.clothingItem;
        }

        rectTransform.localPosition = localPoint - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        JudgyAudioManager.Instance.PutOn();

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
        if(itemData.clothingItem != null){
            image.sprite = itemData.icon;
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

        transform.SetParent(originalSpawnSlot, false);
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
    rectTransform.localScale = Vector3.one;

    imageRect.localScale = Vector3.one * scale;
}

public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
{
    if (image == null || image.sprite == null)
        return false;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        image.rectTransform,
        screenPoint,
        eventCamera,
        out Vector2 localPoint
    );

    Rect rect = image.rectTransform.rect;

    float x = (localPoint.x - rect.x) / rect.width;
    float y = (localPoint.y - rect.y) / rect.height;

    if (x < 0f || x > 1f || y < 0f || y > 1f)
        return false;

    Texture2D tex = image.sprite.texture;
    Rect spriteRect = image.sprite.textureRect;

    int texX = Mathf.FloorToInt(spriteRect.x + x * spriteRect.width);
    int texY = Mathf.FloorToInt(spriteRect.y + y * spriteRect.height);

    Color color = tex.GetPixel(texX, texY);

    return color.a > 0.1f;
}

public void OnPointerEnter(PointerEventData eventData)
{
    shakeAnim.Play();
}

}
