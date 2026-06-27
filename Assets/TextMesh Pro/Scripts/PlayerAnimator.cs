using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private PlayerJump _jump;
    private string _currentState;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _jump = GetComponent<PlayerJump>();
    }

    private void Update()
    {
        if (_animator == null || _rb == null || _jump == null) return;
        if (GameState.Instance == null || GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        float h = Mathf.Abs(_rb.velocity.x);
        string targetState;

        if (!_jump.IsGrounded)
            targetState = "jumping";
        else if (h > 0.3f)
            targetState = "walking";
        else
            targetState = "idle";

        if (_currentState != targetState)
        {
            _animator.Play(targetState, 0, 0f);
            _currentState = targetState;
        }
    }
}
