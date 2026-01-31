using UnityEngine;
using UnityEngine.EventSystems;

public class PipaUI : MonoBehaviour, IDropHandler
{
    public RectTransform clothingLayer;

    private ItemUI currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        ItemUI newItem = eventData.pointerDrag?.GetComponent<ItemUI>();
        if (newItem == null)
            return;

        if (currentItem != null)
        {
            currentItem.ReturnToOriginal();
        }

        currentItem = newItem;
        newItem.EquipTo(clothingLayer);
    }
}
