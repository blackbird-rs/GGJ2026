using UnityEngine;

public static class Extensions
{
    public static void ClearChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
}