using UnityEngine;

public class ClothingSlotUI : MonoBehaviour
{
    public ItemType acceptedItemType;
    public RectTransform clothingLayer;

    private ItemUI currentItem;
    public ItemUI GetCurrentItem() => currentItem;

    public RectTransform uiItemRoot;

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

    public void EquipFromData(ItemData newItemData)
    {
        currentItem = null;
        ItemUI item = Instantiate(itemPrefab, uiItemRoot);
        item.SetItemData(newItemData);
        item.SetOriginalSpawnSlot(clothingLayer);

        currentItem = item;
        item.EquipTo(clothingLayer, this);
    }


    public void ClearSlot()
    {
        currentItem = null;
    }
}
