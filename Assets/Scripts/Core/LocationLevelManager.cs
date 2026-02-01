using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationLevelManager : MonoBehaviour
{
    public LevelDataCollection levelDataCollection;
    public Transform locationHolder;
    public PipaUI pipaUI;
    public JudgesUI judgesUI;
    public SceneLoader sceneLoader;
    public ItemDataCollection itemDataCollection;
    public AlertPopup endGamePopup;

    private void Start()
    {
        if (!SaveSystem.HasSave())
        {
            throw new Exception("No save data found");
        }

        var saveData = SaveSystem.LoadGame();

        var levelIndex = saveData.currentLevelIndex;
        var currentLevelData = levelDataCollection.levels.First(x => x.levelIndex == levelIndex);

        var itemPlacements = saveData.itemPlacements;
        var equippedItems = itemPlacements.Select(x => itemDataCollection.FindItemDataById(x.itemId));

        locationHolder.ClearChildren();
        Instantiate(currentLevelData.levelPrefab, locationHolder);
        judgesUI.gameObject.SetActive(false);
        JudgyAudioManager.Instance.FadeToState(levelIndex);

        StartCoroutine(LocationFlow(currentLevelData, equippedItems));
    }

    private IEnumerator LocationFlow(LevelData currentLevelData, IEnumerable<ItemData> equippedItems)
    {
        pipaUI.StartLevel(equippedItems);

        yield return new WaitForSeconds(3f);

        var averageScore = judgesUI.StartLevel(currentLevelData, equippedItems);

        yield return new WaitForSeconds(8f);

        var saveData = SaveSystem.LoadGame();
        saveData.levelScores[saveData.currentLevelIndex] = averageScore;
        saveData.currentLevelIndex += 1;
        saveData.itemPlacements.Clear();
        saveData.itemSpawns.Clear();
        SaveSystem.SaveGame(saveData);
        JudgyAudioManager.Instance.FadeToDefaultState();

        if (saveData.currentLevelIndex < levelDataCollection.levels.Length)
        {
            BackToDressingRoom();
        }
        else
        {
            var finalScore = saveData.levelScores.Average();
            var finalText = finalScore switch
            {
                < 5.0f => "You didn't fit in, but at least you made it memorable...",
                < 6.5f => "You managed to fit in just fine",
                _ => "You understood the vibe... And improved it!",
            };
            SaveSystem.ClearSave();
            endGamePopup.Open(finalText, BackToDressingRoom, $"{finalScore:0.0}");
        }
    }

    private void BackToDressingRoom(bool _ = false)
    {
        sceneLoader.LoadSceneByIndex(1);
    }
}