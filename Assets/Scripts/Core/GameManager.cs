using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelIndex;
    public int totalScore;

    public LevelData[] levels;
    public ItemUI itemPrefab;

    public RectTransform uiItemRoot;
    public List<RectTransform> itemSpawnSlots;
    public PipaUI pipa;

    private ClothingSlotUI[] clothingSlots;
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
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        clothingSlots = pipa.clothingSlots;

        if (SaveSystem.HasSave())
        {
            SaveSystem.SaveData data = SaveSystem.LoadGame();
            currentLevelIndex = data.currentLevelIndex;
            Debug.Log($"CONTINUE GAME - Level {currentLevelIndex}");
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
        ClearExistingItems();

        if (SaveSystem.HasSave())
            SpawnFromSave();
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

        int slotCursor = 0;

        foreach (var entry in currentLevelData.itemScores)
        {
            RectTransform spawnSlot = shuffledSlots[slotCursor];

            ItemUI item = Instantiate(itemPrefab, uiItemRoot);
            item.SetItemData(entry.item);

            int originalIndex = itemSpawnSlots.IndexOf(spawnSlot);
            item.SetOriginalSpawnSlot(originalIndex);

            item.transform.SetParent(spawnSlot, false);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            spawnedItems.Add(entry.item.itemId, item);
            slotCursor++;
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

            RectTransform spawnSlot = itemSpawnSlots[spawn.slotIndex];

            ItemUI item = Instantiate(itemPrefab, uiItemRoot);
            item.SetItemData(itemData);
            item.SetOriginalSpawnSlot(spawn.slotIndex);

            item.transform.SetParent(spawnSlot, false);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            spawnedItems.Add(itemData.itemId, item);
        }

        foreach (var slot in clothingSlots)
        {
            slot.ClearSlot();
        }

        foreach (var placement in saveData.itemPlacements)
        {
            if (!spawnedItems.TryGetValue(placement.itemId, out ItemUI item))
                continue;

            if (placement.clothingSlotIndex < 0 ||
                placement.clothingSlotIndex >= clothingSlots.Length)
                continue;

            clothingSlots[placement.clothingSlotIndex].Equip(item);
        }
    }

    private void ClearExistingItems()
    {
        foreach (var item in spawnedItems.Values)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawnedItems.Clear();
    }

    public void SaveProgress()
    {
        SaveSystem.SaveData data = new SaveSystem.SaveData
        {
            currentLevelIndex = currentLevelIndex
        };

        foreach (var pair in spawnedItems)
        {
            ItemUI item = pair.Value;

            data.itemSpawns.Add(new SaveSystem.ItemSpawnData
            {
                itemId = item.GetItemData().itemId,
                slotIndex = item.GetOriginalSpawnSlot()
            });
        }

        for (int i = 0; i < clothingSlots.Length; i++)
        {
            ItemUI equippedItem = clothingSlots[i].GetCurrentItem();
            if (equippedItem == null)
                continue;

            data.itemPlacements.Add(new SaveSystem.ItemPlacementData
            {
                itemId = equippedItem.GetItemData().itemId,
                clothingSlotIndex = i
            });
        }

        SaveSystem.SaveGame(data);
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
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
