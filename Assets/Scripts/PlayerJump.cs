using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    // Reference to the 2D Rigidbody component for physics-based movement and jumping
    private Rigidbody2D _rigidbody2D;
    
    // Reference to the player actions script to determine player index/ID
    private PlayerActions _playerActions;
    
    // Dimensions for the capsule overlap area used in ground detection
    public float capsuleHeight = 0.25f;
    public float capsuleRadius = 0.08f;

    // The position from which the ground check detection originates
    public Transform feetCollider;
    
    // Layer mask to define what objects are considered solid ground
    public LayerMask groundMask;
    
    // Boolean tracking whether the player is currently grounded
    private bool _groundCheck;

    // Upward force applied when the player jumps
    public float jumpForce = 10;
    
    // Multiplier applied to gravity to make falling faster and feel less floaty
    public float fallForce = 2;
    
    // Cached global gravity vector scaled to the project physics settings
    private Vector2 _gravityVector;
    
    // Sets gravity vector and connects components 
    private void Start() {
        // Cache the global gravity direction and magnitude
        _gravityVector = new Vector2(0, Physics2D.gravity.y);
        
        // Cache the Rigidbody2D and PlayerActions components attached to this GameObject
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();
    }

    // Update is called once per frame
   private void Update() {
       // Only allow jumping mechanics if the game is currently in a match
       if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
       
       // Checks if player is touching ground using a horizontal capsule overlap shape
       _groundCheck = Physics2D.OverlapCapsule(feetCollider.position, 
           new Vector2(capsuleHeight, capsuleRadius), CapsuleDirection2D.Horizontal,
           0, groundMask);

       // Checks if player is trying to jump/can jump based on user input and ground status
       if (Input.GetButtonDown(GameState.Instance.jumpButton + _playerActions.playerCount) && _groundCheck) {
           // Set upward velocity while maintaining the current horizontal velocity
           _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
       }

       // Checks if the gravity should be getting faster (applies extra downward force when falling)
       if(_rigidbody2D.velocity.y < 0) {
           // Increase falling speed smoothly over time using deltaTime
           _rigidbody2D.velocity += _gravityVector * (fallForce * Time.deltaTime);
       }
   }
}