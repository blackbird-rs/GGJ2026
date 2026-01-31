using System.Collections.Generic;

public class ConformityJudge : Judge
{
    protected override float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        float totalScore = 0; // 0-3
        float bonusMultiplier = 1; // 1-2
        foreach (var item in items)
        {
            var conformity = levelData.GetConformityForItem(item);
            totalScore += conformity;
            if (conformity >= 0.5f)
            {
                bonusMultiplier *= 1.26f;
            }
        }

        return totalScore * bonusMultiplier / 6;
    }
}