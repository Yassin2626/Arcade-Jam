using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public int shieldAmount = 50;
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
            int currentShield = player.playerCount == "1" ? GameState.Instance.playerOneShield : GameState.Instance.playerTwoShield;
            if (currentShield < GameState.MaxShield)
            {
                GameState.Instance.AddShield(player.playerCount, shieldAmount);
                Destroy(gameObject);
            }
        }
    }
}
