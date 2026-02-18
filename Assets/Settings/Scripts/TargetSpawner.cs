using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private float spawnDelay = 2f;   // time before next target spawns
    [SerializeField] private int maxActiveTargets = 4;

    private int activeTargets = 0;

    private void Start()
    {
        SpawnInitialTargets();
    }

    private void SpawnInitialTargets()
    {
        foreach (Transform point in spawnPoints)
        {
            SpawnTargetAt(point);
        }
    }

    private void SpawnTargetAt(Transform point)
    {
        if (activeTargets >= maxActiveTargets) return;

        GameObject target = Instantiate(targetPrefab, point.position, point.rotation);
        MovingTarget movingTarget = target.GetComponent<MovingTarget>();
        if (movingTarget != null)
        {
            movingTarget.SetSpawner(this);
        }

        if (movingTarget == null)
        {
            Debug.LogError("Target prefab is missing MovingTarget script!");
        }

        activeTargets++;
    }

    public void OnTargetDestroyed()
    {
        activeTargets--;
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (spawnPoints.Count > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnTargetAt(randomPoint);
        }
    }
}
