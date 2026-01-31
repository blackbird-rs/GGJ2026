using UnityEngine;

public class ClothingSlotUI : MonoBehaviour
{
    public ItemType acceptedItemType;
    public RectTransform clothingLayer;

    private ItemUI currentItem;
    public ItemUI GetCurrentItem() => currentItem;

    public RectTransform uiItemRoot;
    public ItemUI itemPrefab;

    public bool CanAccept(ItemData itemData)
    {
        return itemData.itemType == acceptedItemType;
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

    public void EquipFromData(ItemData newItemData)
    {
        currentItem = null;
        ItemUI item = Instantiate(itemPrefab, uiItemRoot);
        item.SetItemData(newItemData);

        currentItem = item;
        item.EquipTo(clothingLayer, this);
    }


    public void ClearSlot()
    {
        currentItem = null;
    }

    public bool TryEquip(ItemData newItem)
    {
        if (!CanAccept(newItem))
        {
            return false;
        }
        
        EquipFromData(newItem);
        return true;
    }
}
