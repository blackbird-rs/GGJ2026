using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreativityJudge : Judge
{
    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        if (!items.Any())
        {
            return 0.5f;
        }

        float totalScore = 0; // 3-6
        float bonusMultiplier = 1; // 1-1.5
        foreach (var item in items)
        {
            var conformity = levelData.GetConformityForItem(item);
            var creativity = levelData.GetCreativityForItem(item);
            totalScore += 1 + creativity;
            bonusMultiplier = conformity switch
            {
                < 0f => Mathf.Max(bonusMultiplier, 1.5f),
                < 0.95f => Mathf.Max(bonusMultiplier, 1.25f),
                _ => bonusMultiplier
            };
        }

        return totalScore * bonusMultiplier / items.Count() / 2; // 0.5-1.5
    }
}