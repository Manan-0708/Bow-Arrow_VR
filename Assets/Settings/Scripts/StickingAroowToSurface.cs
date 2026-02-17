using UnityEngine;

public class StickingArrowToSurface : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider myCollider;
    [SerializeField] private GameObject stickingArrow;

    private bool hasHitTarget = false;

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        myCollider.isTrigger = true;

        GameObject arrow = Instantiate(stickingArrow);
        arrow.transform.position = transform.position;
        arrow.transform.forward = transform.forward;

        if (collision.collider.attachedRigidbody != null)
        {
            arrow.transform.parent = collision.collider.attachedRigidbody.transform;
        }

        IHittable hittable = collision.collider.GetComponentInParent<IHittable>();

        if (hittable != null)
        {
            hasHitTarget = true;

            hittable.GetHit();

            // ✅ NEW: Register hit (you can change points later)
            SessionTracker.Instance.OnHit(20);

            VRCSVLogger.Log(
                "ArrowHit",
                gameObject.name,
                "-",
                collision.collider.name
            );
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!hasHitTarget)
        {
            // ✅ NEW: Register miss
            SessionTracker.Instance.OnMiss();

            VRCSVLogger.Log(
                "ArrowMissed",
                gameObject.name,
                "-",
                "No target hit"
            );
        }
    }
}
