using UnityEngine;

public class BombCollision : MonoBehaviour
{
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float explosionVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb"))
        {
            // Spawn explosion
            Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);

            // Play sound
            AudioSource.PlayClipAtPoint(explosionSound, other.transform.position, explosionVolume);

            // Disable or destroy the bomb
            Destroy(other.gameObject);

            // Trigger Game Over
            GameUIController.instance.TriggerGameOver(); // Assuming you add this method
        }
    }
}
