using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public int shieldAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerActions player = collision.GetComponent<PlayerActions>();
        if (player != null && GameState.Instance != null)
        {
            GameState.Instance.AddShield(player.playerCount, shieldAmount);
            Destroy(gameObject);
        }
    }
}
