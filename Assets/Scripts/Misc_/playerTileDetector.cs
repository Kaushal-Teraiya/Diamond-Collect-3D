using UnityEngine;

public class PlayerTileDetector : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private float respawnHeight = 1.5f;
    [SerializeField] private float fallHeight = -5f;

    Rigidbody rb;

    Vector3 lastSafePosition;
    bool hasSafePosition;
    bool onLava;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastSafePosition = rb.position;
    }

    void Update()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.2f,
            Vector3.down
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            rayDistance,
            tileLayer))
        {
            if (hit.collider.CompareTag("Grass"))
            {
                onLava = false;

                // SAVE THE PLAYER'S ACTUAL POSITION.
                // Only save it while safely standing on Grass.
                lastSafePosition = hit.collider.bounds.center;
                lastSafePosition.y = respawnHeight;
                hasSafePosition = true;
            }
            else if (hit.collider.CompareTag("Lava"))
            {
                if (!onLava)
                {
                    onLava = true;

                    Lava lava = hit.collider.GetComponent<Lava>();
                    if (lava != null)
                    {
                        lava.DamagePlayer();
                        Teleport();
                    }
                }
            }
        }
        else
        {
            onLava = false;
        }

        if (rb.position.y <= fallHeight)
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        if (!hasSafePosition)
        {
            Debug.LogWarning("NO SAFE POSITION SAVED!");
            return;
        }

        Vector3 position = lastSafePosition;
        position.y = respawnHeight;

        rb.position = position;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Physics.SyncTransforms();

        onLava = false;

        Debug.Log("RESPAWNED AT: " + position);
    }
}