using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [SerializeField] private float teleportLevel = -10f;
    [SerializeField] private float groundLevel;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (transform.position.y <= teleportLevel)
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        Vector3 position = rb.position;
        position.y = groundLevel;
     
        rb.position = position;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
