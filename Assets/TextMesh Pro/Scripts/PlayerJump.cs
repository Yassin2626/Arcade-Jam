using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private PlayerActions _playerActions;

    public float capsuleHeight = 0.25f;
    public float capsuleRadius = 0.08f;
    public Transform feetCollider;
    public LayerMask groundMask;

    public bool IsGrounded { get; private set; }

    public float jumpForce = 10;
    public float fallForce = 2;
    private Vector2 _gravityVector;

    private void Start() {
        _gravityVector = new Vector2(0, Physics2D.gravity.y);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();
    }

   private void Update() {
       if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

       IsGrounded = Physics2D.OverlapCapsule(feetCollider.position,
           new Vector2(capsuleHeight, capsuleRadius), CapsuleDirection2D.Horizontal,
           0, groundMask);

       if (Input.GetButtonDown(GameState.Instance.jumpButton + _playerActions.playerCount) && IsGrounded) {
           _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
       }

       if (_rigidbody2D.velocity.y < 0) {
           _rigidbody2D.velocity += _gravityVector * (fallForce * Time.deltaTime);
       }
   }
}