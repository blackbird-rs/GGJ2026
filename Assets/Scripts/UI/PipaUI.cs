using System.Collections.Generic;
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

    public void StartLevel(IEnumerable<ItemData> equippedItems)
    {
        foreach (var newItem in equippedItems)
        {
            foreach (var slot in clothingSlots)
            {
                slot.TryEquip(newItem);
            }
        }
    }

}
