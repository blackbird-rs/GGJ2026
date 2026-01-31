using UnityEngine;
using UnityEngine.EventSystems;

public class PipaUI : MonoBehaviour, IDropHandler
{
    public ClothingSlotUI[] clothingSlots;

    public void OnDrop(PointerEventData eventData)
    {
        ItemUI newItem = eventData.pointerDrag?.GetComponent<ItemUI>();
        if (newItem == null)
            return;

        foreach (var slot in clothingSlots)
        {
            if (slot.CanAccept(newItem))
            {
                slot.Equip(newItem);
                return;
            }
        }

        newItem.ReturnToOriginal();
    }

}
