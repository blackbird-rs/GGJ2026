using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Judge : MonoBehaviour
{
    public TMP_Text scoreText;

    public int GetScore(LevelData levelData, IEnumerable<ItemData> items)
    {
        float normalizedScore = GetNormalizedScore(levelData, items);
        normalizedScore = Mathf.Clamp(normalizedScore, 0, 1);
        return Mathf.CeilToInt(10 * normalizedScore);
    }

    protected abstract float GetNormalizedScore(LevelData levelData, IEnumerable<ItemData> items);

    public void StartLevel(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = score.ToString();
    }
}