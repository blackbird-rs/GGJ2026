using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using TMPro;
using UnityEngine;

public class JudgesUI: MonoBehaviour
{
    public List<Judge> judges;
    public TMP_Text averageScoreText;

    public float StartLevel(LevelData currentLevelData, IEnumerable<ItemData> equippedItems)
    {
        foreach (var judge in judges)
        {
            judge.ClearText();
        }
        averageScoreText.text = "";

        gameObject.SetActive(true);

        var scores = judges.Select(x => x.GetScore(currentLevelData, equippedItems)).ToList();
        var averageScore = (float)scores.Sum() / scores.Count;
        StartCoroutine(JudgesFlow(scores, averageScore));
        return averageScore;
    }

    private IEnumerator JudgesFlow(List<int> scores, float averageScore)
    {
        AudioManager.Instance.PlayApplause();

        yield return new WaitForSeconds(0.5f);

        judges[0].StartLevel(scores[0]);

        yield return new WaitForSeconds(0.5f);

        judges[1].StartLevel(scores[1]);

        yield return new WaitForSeconds(0.5f);

        judges[2].StartLevel(scores[2]);

        yield return new WaitForSeconds(0.5f);

        averageScoreText.text = $"{averageScore:0.0}";
    }
}