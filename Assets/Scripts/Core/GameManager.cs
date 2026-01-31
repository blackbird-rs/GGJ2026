using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelIndex;
    public int totalScore;

    public LevelData[] levels;
    public ItemUI itemPrefab;
    public List<RectTransform> itemSpawnSlots;

    private LevelData currentLevelData;
    private Dictionary<string, ItemUI> spawnedItems = new();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    private void InitializeGame()
    {
        if (SaveSystem.HasSave())
        {
            SaveSystem.SaveData data = SaveSystem.LoadGame();
            currentLevelIndex = data.currentLevelIndex;
            Debug.LogError("CONTINUE GAME - Level Index: " + currentLevelIndex);
        }
        else
        {
            currentLevelIndex = 0;
            Debug.Log("NEW GAME");
        }

        totalScore = 0;
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int levelIndex)
    {
        currentLevelData = levels[levelIndex];
        SpawnItems();
    }

    private void SpawnItems()
    {
        if (currentLevelData == null)
            return;

        if (SaveSystem.HasSave())
        {
            SpawnFromSave();
        }
        else
        {
            SpawnRandom();
            SaveProgress();
        }
    }

    private void SpawnRandom()
    {
        List<RectTransform> shuffledSlots = new(itemSpawnSlots);
        Shuffle(shuffledSlots);

        int slotIndex = 0;

        foreach (var entry in currentLevelData.itemScores)
        {
            RectTransform slot = shuffledSlots[slotIndex];

            ItemUI item = Instantiate(itemPrefab, slot);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.SetItemData(entry.item);

            spawnedItems.Add(entry.item.itemId, item);

            slotIndex++;
        }
    }
    
    private void SpawnFromSave()
    {
        SaveSystem.SaveData saveData = SaveSystem.LoadGame();
        if (saveData == null)
            return;

        foreach (var spawn in saveData.itemSpawns)
        {
            ItemData itemData = FindItemDataById(spawn.itemId);
            if (itemData == null)
                continue;

            RectTransform slot = itemSpawnSlots[spawn.slotIndex];

            ItemUI item = Instantiate(itemPrefab, slot);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.SetItemData(itemData);

            spawnedItems.Add(itemData.itemId, item);
        }
    }


    private void SaveProgress()
    {
        SaveSystem.SaveData data = new SaveSystem.SaveData();
        data.currentLevelIndex = currentLevelIndex;

        for (int i = 0; i < itemSpawnSlots.Count; i++)
        {
            foreach (Transform child in itemSpawnSlots[i])
            {
                ItemUI item = child.GetComponent<ItemUI>();
                if (item == null)
                    continue;

                data.itemSpawns.Add(new SaveSystem.ItemSpawnData
                {
                    itemId = item.getItemId(),
                    slotIndex = i
                });
            }
        }

        SaveSystem.SaveGame(data);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private ItemData FindItemDataById(string id)
    {
        foreach (var level in levels)
        {
            foreach (var entry in level.itemScores)
            {
                if (entry.item.itemId == id)
                    return entry.item;
            }  
        }
        return null;
    }

}
