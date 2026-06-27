using TMPro;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    public GameObject weapon;
    private PlayerActions _playerActions;
    public Vector2 direction = Vector2.right;
    public float aimDistance = 0.75f;
    private TextMeshPro _dirIndicator;

    private void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        SpriteRenderer sr = weapon.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        GameObject dirObj = new GameObject("DirIndicator", typeof(TextMeshPro));
        dirObj.transform.SetParent(weapon.transform, false);
        dirObj.transform.localPosition = Vector3.zero;
        _dirIndicator = dirObj.GetComponent<TextMeshPro>();
        _dirIndicator.fontSize = 3;
        _dirIndicator.alignment = TextAlignmentOptions.Center;
        _dirIndicator.color = Color.white;
        _dirIndicator.text = ">";
    }

    private void Update() {
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        float h = Input.GetAxisRaw(GameState.Instance.horizontalAxis + _playerActions.playerCount);
        float v = Input.GetAxisRaw(GameState.Instance.verticalAxis + _playerActions.playerCount);

        Vector2 inputDir = new Vector2(h, v);
        if (inputDir.magnitude > 0.2f)
        {
            direction = inputDir.normalized;
            weapon.transform.position = (Vector2)transform.position + direction * aimDistance;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _dirIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void ResetDirection()
    {
        direction = Vector2.right;
        weapon.transform.position = (Vector2)transform.position + direction * aimDistance;
        weapon.transform.rotation = Quaternion.identity;
        if (_dirIndicator != null)
            _dirIndicator.transform.rotation = Quaternion.identity;
    }
}
