using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState { StateA, StateB }

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("===== Rule Switch =====")]
    [SerializeField] private float switchInterval = 10f;
    [SerializeField] private Color stateAColor = Color.yellow;
    [SerializeField] private Color stateBColor = Color.red;

    [Header("===== Food Spawn =====")]
    [SerializeField] private GameObject[] foodPrefabs;
    [SerializeField] private Transform spawnPoint;

    [Header("===== Signal Light =====")]
    [SerializeField] private SpriteRenderer signalLight;

    [Header("===== DLC A =====")]
    [SerializeField] private bool enableDragMode = false;

    [Header("===== Toolkit References =====")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TrialCounter trialCounter;
    [SerializeField] private CountdownTimer countdownTimer;
    [SerializeField] private ReactionTimeRecorder reactionTimeRecorder;

    [Header("===== Audio =====")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    private AudioSource audioSource;

    [Header("===== Gameplay UI =====")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private Text modeText;

    [Header("===== Result Panel =====")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultStatsText;

    public GameState CurrentState { get; private set; } = GameState.StateA;
    public bool DidSwitchThisTrial { get; private set; }
    public bool EnableDragMode => enableDragMode;

    private float switchTimer;
    private GameState previousState;
    private GameObject currentFood;
    private bool gameRunning;

    // Result tracking
    private int hitCount;
    private int correctRejectionCount;
    private int falseAlarmCount;
    private int missCount;
    private int totalCorrectFood;
    private int totalDistractors;
    private List<float> interceptRTs = new List<float>();
    private List<float> maintainRTs = new List<float>();
    private List<float> switchRTs = new List<float>();
    private List<string> collectedDataLines = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        scoreManager.ResetScore();
        trialCounter.ResetTrials();
        reactionTimeRecorder.ClearRecords();
        hitCount = 0;
        correctRejectionCount = 0;
        falseAlarmCount = 0;
        missCount = 0;
        totalCorrectFood = 0;
        totalDistractors = 0;
        interceptRTs.Clear();
        maintainRTs.Clear();
        switchRTs.Clear();
        collectedDataLines.Clear();

        Debug.Log("@DATA,Trial,State,IsSwitch,FoodType,Action,RT(ms),Correct");
        countdownTimer.StartCountdown(OnCountdownFinished);
    }

    private void OnCountdownFinished()
    {
        gameRunning = true;
        switchTimer = switchInterval;
        UpdateSignalLight();
        SpawnNextFood();
    }

    private void Update()
    {
        if (!gameRunning) return;

        switchTimer -= Time.deltaTime;
        if (switchTimer <= 0f)
        {
            SwitchState();
            switchTimer = switchInterval;
        }
    }

    private void SwitchState()
    {
        previousState = CurrentState;
        CurrentState = (CurrentState == GameState.StateA) ? GameState.StateB : GameState.StateA;
        UpdateSignalLight();
    }

    private void UpdateSignalLight()
    {
        if (signalLight != null)
            signalLight.color = (CurrentState == GameState.StateA) ? stateAColor : stateBColor;

        if (modeText != null)
        {
            if (CurrentState == GameState.StateA)
            {
                modeText.text = "PASS: Fries";
                modeText.color = Color.yellow;
            }
            else
            {
                modeText.text = "PASS: Nugget";
                modeText.color = new Color(1f, 0.4f, 0.4f);
            }
        }
    }

    public void OnTrialComplete(string foodTag, string action, bool correct)
    {
        float rt = reactionTimeRecorder.StopTimingMs();
        DidSwitchThisTrial = (CurrentState != previousState) && (trialCounter.CurrentTrial > 0);

        // Track SDT categories
        switch (action)
        {
            case "Delivered":
                if (correct) { hitCount++; totalCorrectFood++; }
                else { totalDistractors++; missCount++; }
                break;
            case "Discarded":
                correctRejectionCount++;
                totalDistractors++;
                interceptRTs.Add(rt);
                break;
            case "FalseAlarm":
                falseAlarmCount++;
                totalCorrectFood++;
                interceptRTs.Add(rt);
                break;
            case "Miss":
                if (!correct) { missCount++; totalCorrectFood++; }
                else { totalDistractors++; }
                break;
        }

        if (DidSwitchThisTrial)
            switchRTs.Add(rt);
        else
            maintainRTs.Add(rt);

        // Play sound feedback
        if (correct && correctSound != null)
            audioSource.PlayOneShot(correctSound);
        else if (!correct && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        // ===== TODO CSV: Build @DATA CSV string and Debug.Log =====
        // Format: "@DATA," + Trial,State,IsSwitch,FoodType,Action,RT,Correct
        // Hint: use string.Format with the variables above (trialCounter.CurrentTrial,
        //       CurrentState, DidSwitchThisTrial, foodTag, action, rt, correct)
        // Also add the line to collectedDataLines for CSV export

        trialCounter.NextTrial();

        if (!trialCounter.HasRemaining)
        {
            EndGame();
            return;
        }

        SpawnNextFood();
    }

    private void SpawnNextFood()
    {
        if (foodPrefabs == null || foodPrefabs.Length == 0) return;

        int index = Random.Range(0, foodPrefabs.Length);
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : new Vector3(-7f, 0f, 0f);
        currentFood = Instantiate(foodPrefabs[index], pos, Quaternion.identity);

        previousState = CurrentState;
        reactionTimeRecorder.StartTiming();
    }

    private void EndGame()
    {
        gameRunning = false;

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        // Calculate
        int totalCount = hitCount + correctRejectionCount + falseAlarmCount + missCount;
        int correctCount = hitCount + correctRejectionCount;
        float accuracy = (totalCount > 0) ? (correctCount * 100f / totalCount) : 0f;
        float faRate = (totalCorrectFood > 0) ? (falseAlarmCount * 100f / totalCorrectFood) : 0f;
        float interceptRT = AverageOf(interceptRTs);
        float maintainAvg = AverageOf(maintainRTs);
        float switchAvg = AverageOf(switchRTs);
        float switchCost = switchAvg - maintainAvg;

        // Build result text
        string stats = "";
        stats += string.Format("Score: {0}\n", scoreManager.CurrentScore);
        stats += "\n";
        stats += "--- Response Matrix ---\n";
        stats += string.Format("  Hit (Correct Delivery):     {0}\n", hitCount);
        stats += string.Format("  Correct Rejection:          {0}\n", correctRejectionCount);
        stats += string.Format("  False Alarm:                {0}\n", falseAlarmCount);
        stats += string.Format("  Miss:                       {0}\n", missCount);
        stats += "\n";
        stats += "--- Key Metrics ---\n";
        stats += string.Format("  Accuracy:          {0:F1}%\n", accuracy);
        stats += string.Format("  False Alarm Rate:  {0:F1}%\n", faRate);
        stats += string.Format("  Intercept RT:      {0:F0} ms\n", interceptRT);
        stats += string.Format("  Switch Cost:       {0:+0;-0} ms\n", switchCost);

        if (resultStatsText != null)
            resultStatsText.text = stats;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        Debug.Log("=== Game Over ===\n" + stats);
    }

    private float AverageOf(List<float> list)
    {
        if (list.Count == 0) return 0f;
        float sum = 0f;
        foreach (float v in list) sum += v;
        return sum / list.Count;
    }

    public void NotifyFoodDestroyed(GameObject food)
    {
        if (food == currentFood)
            currentFood = null;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ExportCSV()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = "CrazyDriveThru_" + timestamp + ".csv";
        string path = System.IO.Path.Combine(Application.dataPath, "..", fileName);

        var lines = new List<string>();
        lines.Add("Trial,State,IsSwitch,FoodType,Action,RT(ms),Correct");

        // Collect @DATA lines from Unity log via Application.logMessageReceived
        // Since we already logged them, re-export from stored data
        foreach (string line in collectedDataLines)
            lines.Add(line);

        System.IO.File.WriteAllLines(path, lines.ToArray());
        Debug.Log("CSV exported to: " + System.IO.Path.GetFullPath(path));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
