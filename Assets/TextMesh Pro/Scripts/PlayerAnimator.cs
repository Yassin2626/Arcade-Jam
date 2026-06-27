using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private string _currentState;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_animator == null || _rb == null) return;
        if (GameState.Instance == null || GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        float h = Mathf.Abs(_rb.velocity.x);
        float v = _rb.velocity.y;
        string targetState;

        if (v > 0.5f)
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
