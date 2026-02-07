using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private GameObject midPointVisual, arrowPrefab, arrowSpawnPoint;

    [SerializeField]
    private float arrowMaxSpeed = 10;


    public void PrepareArrow()
    {
        midPointVisual.SetActive(true);
    }

    public void ReleaseArrow(float strength)
    {
        midPointVisual.SetActive(false);

        GameObject arrow = Instantiate(arrowPrefab);

        Vector3 spawnPos = arrowSpawnPoint.transform.position
                 + midPointVisual.transform.forward * 0.02f;

        arrow.transform.position = spawnPos;
        arrow.transform.rotation = midPointVisual.transform.rotation;


        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        Vector3 shootDir = midPointVisual.transform.forward;
        rb.linearVelocity = shootDir * (strength * arrowMaxSpeed);


    }
}