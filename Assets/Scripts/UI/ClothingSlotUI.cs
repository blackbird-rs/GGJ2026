using UnityEngine;

public class ClothingSlotUI : MonoBehaviour
{
    public ItemType acceptedItemType;
    public RectTransform clothingLayer;

    private ItemUI currentItem;
    public ItemUI GetCurrentItem() => currentItem;

    public bool CanAccept(ItemUI item)
    {
        return item.GetItemData().itemType == acceptedItemType;
    }

    public void Equip(ItemUI newItem)
    {
        if (currentItem != null)
        {
            currentItem.ReturnToOriginal();
        }

        currentItem = newItem;
        newItem.EquipTo(clothingLayer, this);
    }


    public void ClearSlot()
    {
        currentItem = null;
    }

    public bool TryEquip(ItemData newItem)
    {
        throw new System.NotImplementedException();
    }
}
