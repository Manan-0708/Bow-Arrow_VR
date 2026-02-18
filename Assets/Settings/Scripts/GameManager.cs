using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Orientation,
        TutorialGrab,
        TutorialPull,
        TutorialShoot,
        Ready,
        Playing,
        TimeOver
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetState(GameState.Orientation);
    }

    public void SetState(GameState newState)
    {
        Debug.Log("NEW STATE: " + newState);

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Orientation:
            case GameState.TutorialGrab:
            case GameState.TutorialPull:
            case GameState.TutorialShoot:
            case GameState.Ready:
                Time.timeScale = 1f;
                RoundTimer.Instance.StopTimer();
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                SessionTracker.Instance.StartSession(PlayerManager.Instance.currentPlayer);
                RoundTimer.Instance.StartTimer();
                break;

            case GameState.TimeOver:
                Time.timeScale = 0f;
                break;
        }

        // Notify UI + Tutorial system
        TutorialController.Instance?.OnGameStateChanged(CurrentState);
    }
}
