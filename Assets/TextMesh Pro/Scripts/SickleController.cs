using UnityEngine;

public class SickleController : MonoBehaviour
{
    private Vector2 _direction;
    private bool _returning;
    private float _distanceTraveled;
    private PlayerActions _owner;
    private Rigidbody2D _rb;

    public float speed = 10f;
    public float returnSpeed = 12f;
    public float maxDistance = 5f;
    public float rotationSpeed = 1080f;
    public int damage = 20;
    public string shooterId = "";

    public LayerMask wallMask = 1 << 6;

    public void Init(PlayerActions owner, Vector2 dir, float yOffset)
    {
        _owner = owner;
        _direction = dir.normalized;
        Vector3 pos = transform.position;
        pos.y += yOffset;
        transform.position = pos;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite loaded = Resources.Load<Sprite>("sickle_projectile");
        if (loaded != null)
            sr.sprite = loaded;
        sr.sortingOrder = 20;
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_returning)
        {
            if (_owner == null)
            {
                Destroy(gameObject);
                return;
            }
            Vector3 target = _owner.transform.position;
            Vector2 toTarget = (target - transform.position).normalized;
            _rb.velocity = toTarget * returnSpeed;
            if (Vector3.Distance(transform.position, target) < 0.4f)
            {
                _owner.OnSickleReturned();
                Destroy(gameObject);
            }
            return;
        }

        if (Physics2D.OverlapCircle(transform.position, 0.1f, wallMask))
        {
            _returning = true;
            _rb.velocity = Vector2.zero;
            return;
        }

        _rb.velocity = _direction * speed;
        _distanceTraveled += speed * Time.fixedDeltaTime;
        if (_distanceTraveled >= maxDistance)
        {
            _returning = true;
            _rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerActions player = collision.GetComponent<PlayerActions>();
        if (player != null && player.playerCount != shooterId)
        {
            if (GameState.Instance != null)
                GameState.Instance.TakeDamage(player.playerCount, damage);
        }
    }
}
