using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalScore;

    public ItemDataCollection itemDataCollection;
    public ItemUI itemPrefab;
    public LevelDataCollection levelDataCollection;
    private int currentLevelIndex;

    public RectTransform uiItemRoot;
    public List<RectTransform> itemSpawnSlots;
    public TextMeshProUGUI levelHint;
    public PipaUI pipa;
    public AlertPopup popup;

    private ClothingSlotUI[] clothingSlots;

    private Dictionary<string, ItemUI> spawnedItems = new();

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        clothingSlots = pipa.clothingSlots;

        SaveSystem.SaveData data = SaveSystem.LoadGame();
        if (data != null && data.currentLevelIndex < levelDataCollection.levels.Length)
        {
            currentLevelIndex = data.currentLevelIndex;
        }
        else
        {
            Debug.Log("NEW GAME");
            currentLevelIndex = 0;
            SaveSystem.SaveGame(new SaveSystem.SaveData());
        }

        totalScore = 0;
        Debug.LogError("level: " + currentLevelIndex);
        ApplyHint();
        SpawnItems();
    }

    private void ApplyHint(){
        levelHint.text = levelDataCollection.levels[currentLevelIndex].levelHint;
    }

    private void SpawnItems()
    {
        ClearExistingItems();

        if (SaveSystem.HasSave()){
            SaveSystem.SaveData data = SaveSystem.LoadGame();
            if(data.itemSpawns.Count != 0){
                SpawnFromSave();
            }
            else
            {
                ShowTutorialPopup();
                SpawnRandom();
                SaveProgress();
            }
        }
        else
        {
            ShowTutorialPopup();
            SpawnRandom();
            SaveProgress();
        }
    }

    private void ShowTutorialPopup(){
        popup.Open("Biba has chalkfull of commitments on her trip to Belgrade. Help her leave a good impression!", result => Debug.Log(result));
    }

    private void SpawnRandom()
{
    List<RectTransform> availableSlots = new(itemSpawnSlots);
    Shuffle(availableSlots);

    foreach (var itemData in itemDataCollection.items)
    {
        RectTransform chosenSlot = null;

        for (int i = 0; i < availableSlots.Count; i++)
        {
            ItemSpawnSlot slotData =
                availableSlots[i].GetComponent<ItemSpawnSlot>();

            if (slotData == null)
                continue;

            if (slotData.acceptedSize == itemData.itemSize)
            {
                chosenSlot = availableSlots[i];
                availableSlots.RemoveAt(i);
                break;
            }
        }

        if (chosenSlot == null)
        {
            Debug.LogWarning(
                $"No spawn slot available for item {itemData.itemId} " +
                $"with size {itemData.itemSize}"
            );
            continue;
        }

        ItemUI item = Instantiate(itemPrefab, uiItemRoot);
        item.SetItemData(itemData);

        int originalIndex = itemSpawnSlots.IndexOf(chosenSlot);
        item.SetOriginalSpawnSlot(originalIndex, chosenSlot);

        item.transform.SetParent(chosenSlot, false);
        item.FitToParent();

        spawnedItems.Add(itemData.itemId, item);
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
        SaveSystem.SaveData data;
        if(SaveSystem.HasSave()){
            data = SaveSystem.LoadGame();
            data.itemPlacements.Clear();
            data.itemSpawns.Clear();
        }
        else{
            data = new SaveSystem.SaveData{};
        }

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
        data.currentLevelIndex = currentLevelIndex;
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
