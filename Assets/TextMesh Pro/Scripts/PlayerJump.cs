using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private PlayerActions _playerActions;
    private BoxCollider2D _boxCollider;

    public float jumpForce = 10;
    public float fallForce = 2;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    private Vector2 _gravityVector;

    private void Start() {
        _gravityVector = new Vector2(0, Physics2D.gravity.y);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

   private void Update() {
       if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

       Vector2 checkOrigin = (Vector2)transform.position + _boxCollider.offset + Vector2.down * (_boxCollider.size.y * 0.5f);
       bool grounded = Physics2D.OverlapCircle(checkOrigin, groundCheckDistance, groundMask);

       if (Input.GetButtonDown(GameState.Instance.jumpButton + _playerActions.playerCount) && grounded) {
           _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
       }

       if (_rigidbody2D.velocity.y < 0) {
           _rigidbody2D.velocity += _gravityVector * (fallForce * Time.deltaTime);
       }
   }
}