using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : MonoBehaviour {
    public string playerCount = "1";
    private Vector3 _start;
    private Rigidbody2D _rigidbody;
    private PlayerWeapon _playerWeapon;

    public GameObject xObject;
    public Color bulletColor;
    public LayerMask layersToExclude;
    public float spawnInterval = 2f;
    public float currentTime = 0f;
    private bool _canShoot = true;

    private void Start()
    {
        _start = gameObject.transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerWeapon = GetComponent<PlayerWeapon>();
        if (GameState.Instance != null)
        {
            if (playerCount == "1")
                GameState.Instance.player1Transform = transform;
            else if (playerCount == "2")
                GameState.Instance.player2Transform = transform;
        }
    }

    private void Update() {
        switch (GameState.Instance.gameState) {
            case GameState.GameStateEnum.GetReady: {
                break;
            }
            case GameState.GameStateEnum.InMatch: {
                if (Input.GetButtonDown(GameState.Instance.actionX + playerCount)) {
                    if (!_canShoot) return;
                    currentTime = spawnInterval;
                    _canShoot = false;

                    Transform spawnPoint = _playerWeapon.weapon.transform;
                    GameObject newObject = Instantiate(xObject, spawnPoint.position, spawnPoint.rotation);
                    newObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = bulletColor;
                    newObject.GetComponent<Rigidbody2D>().excludeLayers = layersToExclude;
                    BulletController bullet = newObject.GetComponent<BulletController>();
                    bullet.shooterId = playerCount;
                    int gunIdx = playerCount == "1" ? GameState.Instance.playerOneGunIndex : GameState.Instance.playerTwoGunIndex;
                    bullet.damage = GameState.Instance.GetDamage(gunIdx);
                    bullet.SetDirection(_playerWeapon.direction);
                }

                if (Input.GetButtonDown(GameState.Instance.actionB + playerCount)) {
                    Debug.Log(GameState.Instance.actionB + playerCount + " B button Pressed");
                }

                if (Input.GetButtonDown(GameState.Instance.actionY + playerCount)) {
                    Debug.Log(GameState.Instance.actionY + playerCount + " Y button Pressed");
                }

                if (Input.GetButtonDown(GameState.Instance.actionRB + playerCount)) {
                    Debug.Log(GameState.Instance.actionRB + playerCount + " Right Bumper button Pressed");
                }

                if (Input.GetButtonDown(GameState.Instance.actionLB + playerCount)) {
                    Debug.Log(GameState.Instance.actionLB + playerCount + " Left Bumper button Pressed");
                }

                if (!_canShoot) {
                    currentTime -= Time.deltaTime;
                    if (currentTime < 0) {
                        _canShoot = true;
                    }
                }
                break;
            }
            case GameState.GameStateEnum.GameOver: {
                if (Input.GetButtonDown(GameState.Instance.jumpButton + playerCount)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.tag) {
            case "Death": {
                transform.position = _start;
                _rigidbody.velocity = Vector2.zero;
                GameState.Instance.TakeDamage(playerCount, 25);
                break;
            }
        }
    }
}
