using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsistencyJudge : Judge
{
    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        var saveData = SaveSystem.LoadGame();
        float totalScore = 0; // 3-6
        float bonusMultiplier = 1; // 1-1.5
        foreach (var item in items)
        {
            if (saveData.oldItems.Contains(item.itemId) && saveData.olderItems.Contains(item.itemId))
            {
                totalScore += 2;
                bonusMultiplier = Mathf.Min(bonusMultiplier, 1.5f);
            }
            else if (saveData.oldItems.Contains(item.itemId) || saveData.olderItems.Contains(item.itemId))
            {
                totalScore += 1.5f;
                bonusMultiplier = Mathf.Min(bonusMultiplier, 1.25f);
            }
            else
            {
                totalScore += 1;
            }
        }

        saveData.olderItems = saveData.oldItems;
        saveData.oldItems = items.Select(x => x.itemId).ToList();
        SaveSystem.SaveGame(saveData);

        return totalScore * bonusMultiplier / 6; // 0.5-1.5
    }
}