using UnityEngine;

public class SessionTracker : MonoBehaviour
{
    public static SessionTracker Instance;

    public PlayerData currentPlayer;
    public int arrowsShot;
    public int hits;
    public int misses;

    private void Awake()
    {
        Instance = this;
    }

    public void RecordShot(bool wasHit)
    {
        arrowsShot++;

        if (wasHit) hits++;
        else misses++;

        currentPlayer.arrowsShot++;
        currentPlayer.hits = hits;
        currentPlayer.misses = misses;
        currentPlayer.accuracy = (float)hits / arrowsShot * 100f;

        DontDestroyOnLoad(gameObject);


        GameDataManager.Instance.SaveCSV();
    }
}
