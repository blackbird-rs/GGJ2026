using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string levelHint;
    public GameObject levelPrefab;
    public AudioClipSettings audioClipSettings;
    public List<ItemScoreEntry> itemScores = new();

    public float GetConformityForItem(ItemData item) =>
        itemScores.FirstOrDefault(entry => entry.item.itemId == item.itemId)?.conformity ?? 0;

    public float GetCreativityForItem(ItemData item) =>
        itemScores.FirstOrDefault(entry => entry.item.itemId == item.itemId)?.creativity ?? 0;

    [Serializable]
    public class ItemScoreEntry
    {
        public ItemData item;
        [Range(-1, 1)] public float conformity;
        [Range(0, 1)] public float creativity;
    }
}
