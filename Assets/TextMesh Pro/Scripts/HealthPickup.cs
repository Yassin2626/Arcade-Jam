using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 25;
    private float _lifetime = 15f;

    private void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0f) Destroy(gameObject);
    }

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
