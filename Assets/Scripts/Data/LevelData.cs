using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public List<ItemScoreEntry> itemScores = new();

    public int GetScoreForItem(ItemData item)
    {
        foreach (var entry in itemScores)
        {
            if (entry.item == item)
                return entry.scoreValue;
        }

        return 0;
    }

    [Serializable]
    public class ItemScoreEntry
    {
        public ItemData item;
        public int scoreValue;
    }
}
