using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour, IHittable
{
    private bool canMove;
    private Rigidbody rb;
    private bool stopped = false;

    private Vector3 nextposition;
    private Vector3 originPosition;
    private float baseHeight;   // <-- IMPORTANT FIX: stable Y reference

    [SerializeField]
    private int health = 1;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float arriveThreshold = 0.2f, movementRadius = 2f, speed = 1f;

    [Header("Vertical (Up-Down) Movement")]
    [SerializeField] private float verticalHeight = 0.05f;
    [SerializeField] private float verticalSpeed = 1f;

    [SerializeField] private float destroyDelay = 1f;
    private TargetSpawner spawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody missing on MovingTarget!");
            enabled = false;
            return;
        }

        // Capture starting position
        originPosition = transform.position;

        // VERY IMPORTANT: store only the original Y level
        baseHeight = transform.position.y;

        nextposition = GetNewMovementPosition();
    }

    private Vector3 GetNewMovementPosition()
    {
        return originPosition + (Vector3)Random.insideUnitCircle * movementRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only play sound if hit by an Arrow
        if (collision.gameObject.CompareTag("Arrow"))
        {
            audioSource?.Play();
        }
    }


    public void GetHit()
    {
        health--;

        if (health <= 0 && !stopped)
        {
            stopped = true;

            // Let physics take over (fall down)
            rb.isKinematic = false;

            // NEW: destroy after delay
            StartCoroutine(DestroyAfterDelay(destroyDelay));
        }
    }
    public void SetSpawner(TargetSpawner targetSpawner)
    {
        spawner = targetSpawner;
    }


    private void FixedUpdate()
    {
        if (!stopped && canMove) return;

        // -------- YOUR ORIGINAL WANDERING MOVEMENT (STABLE) --------
        if (Vector3.Distance(transform.position, nextposition) < arriveThreshold)
        {
            nextposition = GetNewMovementPosition();
        }

        Vector3 direction = nextposition - transform.position;
        Vector3 horizontalMove = direction.normalized * Time.fixedDeltaTime * speed;

        // Move ONLY in X/Z with Rigidbody (keep current Y for now)
        rb.MovePosition(new Vector3(
            transform.position.x + horizontalMove.x,
            transform.position.y,
            transform.position.z + horizontalMove.z
        ));

        // -------- CLEAN, DRIFT-FREE UP–DOWN MOTION --------
        float verticalOffset = Mathf.Sin(Time.time * verticalSpeed) * verticalHeight;

        // Force Y to always oscillate around original spawn height
        transform.position = new Vector3(
            transform.position.x,
            baseHeight + verticalOffset,
            transform.position.z
        );
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        spawner?.OnTargetDestroyed();
        Destroy(gameObject);
    }


}
