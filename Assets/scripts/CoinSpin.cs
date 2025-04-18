using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float spinSpeed = 180f; // Default: 180°/sec (3 full rotations per second)

    [Tooltip("Spin axis (normalized vector)")]
    public Vector3 spinAxis = Vector3.up; // Default: Y-axis (vertical spin)

    [Header("Bobbing Effect (Optional)")]
    public bool enableBobbing = true;
    public float bobHeight = 0.5f; // How high the coin moves up and down
    public float bobSpeed = 1f; // How fast the bobbing happens

    private Vector3 startPosition;
    private float randomOffset; // For variety if multiple coins

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Randomize bobbing phase
    }

    void Update()
    {
        // Constant spinning
        transform.Rotate(spinAxis.normalized * spinSpeed * Time.deltaTime);

        // Optional bobbing effect
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Visualize spin axis in editor (debugging)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + spinAxis.normalized * 0.5f);
        Gizmos.DrawSphere(transform.position + spinAxis.normalized * 0.5f, 0.05f);
    }
}