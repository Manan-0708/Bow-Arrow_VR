using UnityEngine;

public class TargetController : MonoBehaviour, IHittable
{
    private Rigidbody rb;
    private bool stopped = false;

    [SerializeField] private int health = 1;

    private TargetSpawner spawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody missing on Target!");
            enabled = false;
            return;
        }
    }

    public void SetSpawner(TargetSpawner targetSpawner)
    {
        spawner = targetSpawner;
    }

    public void GetHit()
    {
        health--;

        if (health <= 0 && !stopped)
        {
            stopped = true;

            // Let physics take over (fall down)
            rb.isKinematic = false;

            StartCoroutine(DestroyAfterDelay(1f));
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        spawner?.OnTargetDestroyed();
        Destroy(gameObject);
    }
}
