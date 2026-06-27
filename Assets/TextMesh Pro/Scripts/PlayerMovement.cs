using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 5;
    private Rigidbody2D _rigidbody2D;
    private PlayerActions _playerActions;
    private float _lastHorizontal;

    private void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();
    }

    private void Update() {
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
        float horizontal = Input.GetAxisRaw(GameState.Instance.horizontalAxis + _playerActions.playerCount);
        _rigidbody2D.velocity = new Vector2(horizontal * speed, _rigidbody2D.velocity.y);
        _lastHorizontal = horizontal;
    }

    private void LateUpdate() {
        if (_lastHorizontal > 0f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (_lastHorizontal < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
