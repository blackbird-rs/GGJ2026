using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public static class SaveSystem
{
    private const string SAVE_KEY = "DressUp_SaveData";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData LoadGame()
    {
        if (!HasSave())
            return null;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteAll();

    }

    [Serializable]
    public class SaveData
    {
        public int currentLevelIndex;
        public List<ItemSpawnData> itemSpawns = new();
        public List<ItemPlacementData> itemPlacements = new();
        public Dictionary<int, float> levelScores = new();

        [FormerlySerializedAs("previousItems")] public List<ItemData> oldItems = new();
        public List<ItemData> olderItems = new();

        public void AddScore(int levelIndex, float score)
        {
            levelScores[levelIndex] = score;
        }
    }

    [Serializable]
    public class ItemSpawnData
    {
        public string itemId;
        public int slotIndex;
    }

    [Serializable]
    public class ItemPlacementData
    {
        public string itemId;
        public int clothingSlotIndex;
    }
}
