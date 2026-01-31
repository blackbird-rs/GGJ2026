using UnityEngine;

[CreateAssetMenu(menuName = "ItemDataCollection")]
public class ItemDataCollection : ScriptableObject
{
    public ItemData[] items;

    public ItemData FindItemDataById(string id)
    {
        foreach(var item in items)
        {
            if(item.itemId == id){
                return item;
            }
        }
        return null;
    }
}