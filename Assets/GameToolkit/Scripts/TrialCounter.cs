using UnityEngine;
using UnityEngine.UI;

public class TrialCounter : MonoBehaviour
{
    [Header("===== Trial Settings =====")]
    [SerializeField] private Text trialText;
    [Range(5, 200)]
    [SerializeField] private int totalTrials = 30;
    [SerializeField] private string displayFormat = "Trial {0} / {1}";

    public int CurrentTrial { get; private set; }
    public int TotalTrials { get => totalTrials; set => totalTrials = value; }
    public bool HasRemaining => CurrentTrial < totalTrials;

    public void NextTrial()
    {
        CurrentTrial++;
        UpdateUI();
    }

    public void ResetTrials()
    {
        CurrentTrial = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (trialText != null)
            trialText.text = string.Format(displayFormat, CurrentTrial, totalTrials);
    }
}
