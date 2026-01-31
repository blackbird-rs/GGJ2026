using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevelIndex;
    public int totalScore;

    public LevelData[] levels;
    public ItemUI itemPrefab;
    public List<RectTransform> itemPlacementSlots; 
    private LevelData currentLevelData;


    public void Awake() {
        if(instance != null){
            Destroy(gameObject);
            return;
        }

        instance = this;
        currentLevelIndex = 0;
        totalScore = 0;
        LoadLevel(currentLevelIndex);
        DontDestroyOnLoad(gameObject);
    }

    private void LoadLevel(int levelIndex){
        currentLevelData = levels[levelIndex];
        SpawnItems();
    }
    
    private void SpawnItems()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("No LevelData");
            return;
        }

        Shuffle(itemPlacementSlots);

        int slotIndex = 0;

        foreach (var entry in currentLevelData.itemScores)
        {
            if (slotIndex >= itemPlacementSlots.Count)
            {
                Debug.LogWarning("Not enough spawn slots");
                break;
            }

            RectTransform slot = itemPlacementSlots[slotIndex];

            ItemUI item = Instantiate(itemPrefab, slot);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            item.SetItemData(entry.item);

            slotIndex++;
        }
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
