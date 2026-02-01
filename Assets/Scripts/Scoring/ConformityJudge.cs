using System.Collections.Generic;
using System.Linq;

public class ConformityJudge : Judge
{
    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        if (!items.Any())
        {
            return 0.5f;
        }

        float totalScore = 0; // 0-3
        float bonusMultiplier = 1; // 1-2
        foreach (var item in items)
        {
            var conformity = levelData.GetConformityForItem(item);
            totalScore += 0.5f * (1 + conformity);
            if (conformity >= 0.5f)
            {
                bonusMultiplier *= 1.26f;
            }
        }

        return totalScore * bonusMultiplier / items.Count() / 2; // 0.0-1.0
    }
}