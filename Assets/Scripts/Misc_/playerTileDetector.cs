using UnityEngine;

public class PlayerTileDetector : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask tileLayer;

    private bool onLava;

    void Update()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.2f,
            Vector3.down
        );

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, tileLayer))
        {
            bool currentlyOnLava = hit.collider.CompareTag("Lava");

            if (currentlyOnLava && !onLava)
            {
                Debug.Log("🔥 DAMAGE TRIGGERED");
                Debug.Log("GameManager: " + GameManager.Instance);

                GameManager.Instance.TakeDamage();
            }
            onLava = currentlyOnLava;
        }
        else
        {
            onLava = false;
        }
    }
}