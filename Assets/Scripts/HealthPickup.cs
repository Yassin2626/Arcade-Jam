using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerActions player = collision.GetComponent<PlayerActions>();
        if (player != null && GameState.Instance != null)
        {
            GameState.Instance.HealPlayer(player.playerCount, healAmount);
            Destroy(gameObject);
        }
    }
}
