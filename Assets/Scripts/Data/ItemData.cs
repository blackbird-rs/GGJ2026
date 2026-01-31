using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]

public class ItemData : ScriptableObject
{
    public string itemId; 
    
    public ItemType itemType;

    public Sprite icon;

    public Sprite clothingItem;
    public Sprite additionalItem;
}