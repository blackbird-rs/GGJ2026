using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int totalScore;

    public ItemDataCollection itemDataCollection;
    public ItemUI itemPrefab;

    public RectTransform uiItemRoot;
    public List<RectTransform> itemSpawnSlots;
    public PipaUI pipa;

    private ClothingSlotUI[] clothingSlots;

    private Dictionary<string, ItemUI> spawnedItems = new();

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        clothingSlots = pipa.clothingSlots;

        if (SaveSystem.HasSave())
        {
            SaveSystem.SaveData data = SaveSystem.LoadGame();
        }
        else
        {
            Debug.Log("NEW GAME");
        }

        totalScore = 0;
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

        foreach (var itemData in itemDataCollection.items)
        {
            RectTransform spawnSlot = shuffledSlots[slotCursor];

            ItemUI item = Instantiate(itemPrefab, uiItemRoot);
            item.SetItemData(itemData);

            int originalIndex = itemSpawnSlots.IndexOf(spawnSlot);
            item.SetOriginalSpawnSlot(originalIndex, spawnSlot);

            item.transform.SetParent(spawnSlot, false);
            item.FitToParent();

            spawnedItems.Add(itemData.itemId, item);
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
            ItemData itemData = itemDataCollection.FindItemDataById(spawn.itemId);
            if (itemData == null)
                continue;

            RectTransform spawnSlot = itemSpawnSlots[spawn.slotIndex];

            ItemUI item = Instantiate(itemPrefab, uiItemRoot);
            item.SetItemData(itemData);
            item.SetOriginalSpawnSlot(spawn.slotIndex, spawnSlot);

            item.transform.SetParent(spawnSlot, false);
            item.FitToParent();

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
        SaveSystem.SaveData data = new SaveSystem.SaveData{};

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
}
