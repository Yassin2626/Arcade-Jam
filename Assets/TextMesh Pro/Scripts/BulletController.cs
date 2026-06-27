using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour {
    private Rigidbody2D _rigidbody2D;
    private Vector2 _direction = Vector2.right;

    public float delayBeforeShrink = 2f;
    public float shrinkDuration = 1f;
    public int speed = 80;

    public string shooterId = "";
    public int damage = 10;

    public LayerMask wallMask = 1 << 6;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine()
    {
        yield return new WaitForSeconds(delayBeforeShrink);
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / shrinkDuration);
            yield return null;
        }
        transform.localScale = targetScale;
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 dir) {
        _direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update() {
        _rigidbody2D.velocity = _direction * speed;
        if (Physics2D.OverlapCircle(transform.position, 0.05f, wallMask))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerActions player = collision.GetComponent<PlayerActions>();
        if (player == null || player.playerCount == shooterId) return;
        if (GameState.Instance != null)
            GameState.Instance.TakeDamage(player.playerCount, damage);
        Destroy(gameObject);
    }
}
