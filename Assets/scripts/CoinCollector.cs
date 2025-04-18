using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Notify Game Controller
            GameUIController.instance.CollectCoin();

            // Visual/Audio feedback
            Destroy(other.gameObject);

            // Optional: Add particle effect
            // Instantiate(collectEffect, other.transform.position, Quaternion.identity);
        }
    }
}