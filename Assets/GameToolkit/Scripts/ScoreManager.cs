using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("===== Score Settings =====")]
    [SerializeField] private Text scoreText;
    [SerializeField] private string displayFormat = "Score: {0}";

    public int CurrentScore { get; private set; }

    public void AddScore(int points)
    {
        CurrentScore += points;
        UpdateUI();
    }

    public void SubtractScore(int points)
    {
        CurrentScore = Mathf.Max(0, CurrentScore - points);
        UpdateUI();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = string.Format(displayFormat, CurrentScore);
    }
}
