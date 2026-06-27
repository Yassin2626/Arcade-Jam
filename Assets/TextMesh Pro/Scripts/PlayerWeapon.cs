using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    public GameObject weapon;
    private PlayerActions _playerActions;
    public Vector2 direction = Vector2.right;
    public float aimDistance = 0.75f;
    private SpriteRenderer _weaponSr;
    private int _weaponIndex = -1;

    private void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        _weaponSr = weapon.GetComponent<SpriteRenderer>();
        if (_weaponSr == null) return;

        int gunIdx = _playerActions.playerCount == "1"
            ? GameState.Instance.playerOneGunIndex
            : GameState.Instance.playerTwoGunIndex;

        SetWeaponSprite(gunIdx);
    }

    private void SetWeaponSprite(int gunIdx)
    {
        _weaponIndex = gunIdx;
        Sprite weaponSprite = gunIdx == 0
            ? Resources.Load<Sprite>("gun")
            : Resources.Load<Sprite>("The-Sickle");
        if (weaponSprite != null)
        {
            _weaponSr.sprite = weaponSprite;
            _weaponSr.enabled = true;
        }
        _weaponSr.sortingOrder = 10;
    }

    private void Update() {
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;

        int gunIdx = _playerActions.playerCount == "1"
            ? GameState.Instance.playerOneGunIndex
            : GameState.Instance.playerTwoGunIndex;

        if (gunIdx != _weaponIndex)
        {
            SetWeaponSprite(gunIdx);
        }

        float h = Input.GetAxisRaw(GameState.Instance.horizontalAxis + _playerActions.playerCount);
        float v = Input.GetAxisRaw(GameState.Instance.verticalAxis + _playerActions.playerCount);

        Vector2 inputDir = new Vector2(h, v);
        if (inputDir.magnitude > 0.2f)
        {
            direction = inputDir.normalized;
            weapon.transform.position = (Vector2)transform.position + direction * aimDistance;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void ResetDirection()
    {
        direction = Vector2.right;
        weapon.transform.position = (Vector2)transform.position + direction * aimDistance;
        weapon.transform.rotation = Quaternion.identity;
    }
}
