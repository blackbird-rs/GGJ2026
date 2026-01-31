using System.Collections.Generic;
using UnityEngine;

public class CreativityJudge : Judge
{
    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        float totalScore = 0; // 3-6
        float bonusMultiplier = 1; // 1-2
        foreach (var item in items)
        {
            var conformity = levelData.GetConformityForItem(item);
            var creativity = levelData.GetCreativityForItem(item);
            totalScore += 1 + creativity;
            bonusMultiplier = conformity switch
            {
                < 0f => Mathf.Min(bonusMultiplier, 2),
                < 0.5f => Mathf.Min(bonusMultiplier, 1.5f),
                _ => bonusMultiplier
            };
        }

        return totalScore * bonusMultiplier / 12;
    }
}