using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public static RoundTimer Instance;
    public bool IsRunning => isRunning;

    [SerializeField] private float roundDuration = 120f; // 2 minutes

    private float timeRemaining;
    private bool isRunning = false;

    public bool IsTimeOver => timeRemaining <= 0f;
    public float TimeRemaining => timeRemaining;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartTimer()
    {
        timeRemaining = roundDuration;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;

            // End the session automatically when time is over
            SessionTracker.Instance.EndSession();
            GameManager.Instance.SetState(GameManager.GameState.TimeOver);
            GameUIManager.Instance.ShowGameOver((int)SessionTracker.Instance.Score);

        }
    }

    // Utility: format time as MM:SS
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    public void RestartRound()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

}
