using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsistencyJudge : Judge
{
    private IEnumerable<ItemData> previousItems = new List<ItemData>();
    private IEnumerable<ItemData> olderItems = new List<ItemData>();

    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        float totalScore = 0; // 3-6
        float bonusMultiplier = 1; // 1-1.5
        foreach (var item in items)
        {
            if (previousItems.Contains(item) && olderItems.Contains(item))
            {
                totalScore += 2;
                bonusMultiplier = Mathf.Min(bonusMultiplier, 1.5f);
            }
            else if (previousItems.Contains(item) || olderItems.Contains(item))
            {
                totalScore += 1.5f;
                bonusMultiplier = Mathf.Min(bonusMultiplier, 1.25f);
            }
            else
            {
                totalScore += 1;
            }
        }

        olderItems = previousItems;
        previousItems = items;

        return totalScore * bonusMultiplier / 6; // 0.5-1.5
    }
}