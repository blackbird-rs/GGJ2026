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
    public JudgesUI judgesUI;
    public SceneLoader sceneLoader;
    public ItemDataCollection itemDataCollection;

    private void Start()
    {
        if (!SaveSystem.HasSave())
        {
            throw new Exception("No save data found");
        }

        var saveData = SaveSystem.LoadGame();

        var levelIndex = saveData.currentLevelIndex;
        var currentLevelData = levelsData.Find(x => x.levelIndex == levelIndex);

        var itemPlacements = saveData.itemPlacements;
        var equippedItems = itemPlacements.Select(x => itemDataCollection.FindItemDataById(x.itemId));

        locationHolder.ClearChildren();
        Instantiate(currentLevelData.levelPrefab, locationHolder);
        pipaUI.gameObject.SetActive(false);
        judgesUI.gameObject.SetActive(false);
        JudgyAudioManager.Instance.FadeToState(levelIndex);

        StartCoroutine(LocationFlow(currentLevelData, equippedItems));
    }

    private IEnumerator LocationFlow(LevelData currentLevelData, IEnumerable<ItemData> equippedItems)
    {
        yield return new WaitForSeconds(1f);

        pipaUI.StartLevel(equippedItems);

        yield return new WaitForSeconds(3f);

        var averageScore = judgesUI.StartLevel(currentLevelData, equippedItems);

        yield return new WaitForSeconds(5f);

        var saveData = SaveSystem.LoadGame();
        saveData.levelScores[saveData.currentLevelIndex] = averageScore;
        saveData.currentLevelIndex += 1;
        saveData.itemPlacements.Clear();
        saveData.itemSpawns.Clear();
        SaveSystem.SaveGame(saveData);
        JudgyAudioManager.Instance.FadeToDefaultState();

        sceneLoader.LoadSceneByIndex(1);
    }
}