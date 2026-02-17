using System;
using System.IO;
using UnityEngine;

public class SessionTracker : MonoBehaviour
{
    public static SessionTracker Instance;

    private string sessionPath;

    private string sessionId;
    private string playerName;
    private string startTime;
    private string endTime;

    private int arrowsShot = 0;
    private int hits = 0;
    private int misses = 0;
    private float score = 0f;
    private float bowHoldTime = 0f;

    private float bowGrabStartTime = 0f;
    private bool isBowHeld = false;

    private bool sessionEnded = false;   // ✅ IMPORTANT

    // -------- UI READONLY ACCESS --------
    public int ArrowsShot => arrowsShot;
    public int Hits => hits;
    public int Misses => misses;
    public float Score => score;
    public float Accuracy => arrowsShot > 0 ? (float)hits / arrowsShot : 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        sessionPath = Path.Combine(Application.persistentDataPath, "sessiondetails.csv");

        if (!File.Exists(sessionPath))
        {
            File.WriteAllText(sessionPath,
                "Player,SessionID,StartTime,EndTime,ArrowsShot,Hits,Misses,Accuracy,Score,BowHoldTime\n");
        }
    }

    // -------- SESSION CONTROL --------

    public void StartSession(string player)
    {
        sessionEnded = false;   // ✅ Reset protection

        playerName = player;
        sessionId = "S_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        startTime = DateTime.Now.ToString("o");

        arrowsShot = 0;
        hits = 0;
        misses = 0;
        score = 0f;
        bowHoldTime = 0f;

        Debug.Log($"Session started for {playerName} | {sessionId}");
    }

    public void EndSession()
    {
        if (sessionEnded) return;   // ✅ Prevent double write
        sessionEnded = true;

        endTime = DateTime.Now.ToString("o");

        float accuracy = arrowsShot > 0 ? (float)hits / arrowsShot : 0f;

        string line =
            $"{playerName},{sessionId},{startTime},{endTime}," +
            $"{arrowsShot},{hits},{misses},{accuracy},{score},{bowHoldTime}\n";

        File.AppendAllText(sessionPath, line);

        FileDataManager.Instance.UpdateLeaderboard(
            playerName,
            Mathf.RoundToInt(score),
            accuracy,
            hits,
            arrowsShot
        );

        Debug.Log("Session saved to sessiondetails.csv");
    }

    // -------- EVENTS FROM GAMEPLAY --------

    public void OnArrowShot()
    {
        if (sessionEnded) return;
        arrowsShot++;
    }

    public void OnHit(int points)
    {
        if (sessionEnded) return;
        hits++;
        score += points;
    }

    public void OnMiss()
    {
        if (sessionEnded) return;
        misses++;
    }

    public void OnBowGrab()
    {
        if (sessionEnded) return;
        bowGrabStartTime = Time.time;
        isBowHeld = true;
    }

    public void OnBowRelease()
    {
        if (sessionEnded) return;

        if (isBowHeld)
        {
            bowHoldTime += Time.time - bowGrabStartTime;
            isBowHeld = false;
        }
    }
}
