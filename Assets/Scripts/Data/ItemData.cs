using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]

public class ItemData : ScriptableObject
{
    public ItemType itemType;

    public Sprite icon;
}