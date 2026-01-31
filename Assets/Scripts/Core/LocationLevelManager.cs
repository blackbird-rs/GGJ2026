using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationLevelManager : MonoBehaviour
{
    public List<LevelData> levelsData;
    public Transform locationHolder;
    public PipaUI pipaUI;
    // public JudgesUI judgesUI;
    public SceneLoader sceneLoader;

    private SaveSystem.SaveData saveData;
    private LevelData currentLevelData;
    private IEnumerable<ItemData> equippedItems;

    private void Start()
    {
        if (!SaveSystem.HasSave())
        {
            throw new Exception("No save data found");
        }

        saveData = SaveSystem.LoadGame();

        var levelIndex = saveData.currentLevelIndex;
        currentLevelData = levelsData.Find(x => x.levelIndex == levelIndex);

        var itemPlacements = saveData.itemPlacements;
        equippedItems = itemPlacements.Select(x => GameManager.instance.FindItemDataById(x.itemId));

        StartCoroutine(LocationFlow());
    }

    private IEnumerator LocationFlow()
    {
        locationHolder.ClearChildren();
        Instantiate(currentLevelData.levelPrefab, locationHolder);

        yield return new WaitForSeconds(1f);

        pipaUI.StartLevel(equippedItems);

        yield return new WaitForSeconds(3f);

        // judgesUI.StartLevel(currentLevelData, equippedItems);

        yield return new WaitForSeconds(5f);

        saveData.currentLevelIndex += 1;
        SaveSystem.SaveGame(saveData);
        sceneLoader.LoadSceneByIndex(1);
    }
}