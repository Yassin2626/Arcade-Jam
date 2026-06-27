using System;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class PlayerActions : MonoBehaviour {
        public string playerCount = "1";
        private Vector3 _start;
        private Rigidbody2D _rigidbody;
        private PlayerWeapon _playerWeapon;
    private Sprite _bulletGunSprite;
    private AudioSource _audioSource;
    private AudioClip _sickleSound;
    private AudioClip _gunSound;

        public GameObject xObject;
        public Color bulletColor;
        public LayerMask layersToExclude;
        public float spawnInterval = 2f;
        public float currentTime = 0f;
        private bool _canShoot = true;
        private int _activeSickles;

        private void Start()
        {
            _start = gameObject.transform.position;
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerWeapon = GetComponent<PlayerWeapon>();
            _bulletGunSprite = Resources.Load<Sprite>("bullet_gun");
            _audioSource = gameObject.GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _sickleSound = Resources.Load<AudioClip>("sickle_throw");
            _gunSound = Resources.Load<AudioClip>("gun_shoot");
            if (GetComponent<PlayerAnimator>() == null)
            gameObject.AddComponent<PlayerAnimator>();
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

                    int gunIdx = playerCount == "1" ? GameState.Instance.playerOneGunIndex : GameState.Instance.playerTwoGunIndex;

                    if (gunIdx == 1)
                    {
                        SpawnSickles();
                    }
                    else
                    {
                        if (_gunSound != null)
                            _audioSource.PlayOneShot(_gunSound);
                        currentTime = spawnInterval;
                        _canShoot = false;
                        Transform spawnPoint = _playerWeapon.weapon.transform;
                        GameObject newObject = Instantiate(xObject, spawnPoint.position, spawnPoint.rotation);
                        newObject.transform.localScale = Vector3.one * 0.03f;
                        SpriteRenderer sr = newObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
                        sr.color = bulletColor;
                        if (_bulletGunSprite != null)
                            sr.sprite = _bulletGunSprite;
                        newObject.GetComponent<Rigidbody2D>().excludeLayers = layersToExclude;
                        BulletController bullet = newObject.GetComponent<BulletController>();
                        bullet.shooterId = playerCount;
                        bullet.damage = GameState.Instance.GetDamage(gunIdx);
                        bullet.SetDirection(_playerWeapon.direction);
                    }
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

                if (!_canShoot && _activeSickles <= 0) {
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
    }

    private void SpawnSickles()
    {
        if (_sickleSound != null)
            _audioSource.PlayOneShot(_sickleSound);
        _canShoot = false;
        _activeSickles = 2;
        Vector2 dir = _playerWeapon.direction;
        Transform wp = _playerWeapon.weapon.transform;

        for (int i = 0; i < 2; i++)
        {
            GameObject go = new GameObject("Sickle", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(SickleController));
            go.transform.position = wp.position;
            go.transform.localScale = Vector3.one * 0.18f;

            BoxCollider2D bc = go.GetComponent<BoxCollider2D>();
            bc.isTrigger = true;
            bc.size = new Vector2(1f, 1f);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            SickleController sc = go.GetComponent<SickleController>();
            sc.shooterId = playerCount;
            sc.damage = GameState.Instance.GetDamage(1);
            float yOff = i == 0 ? 0.4f : -0.4f;
            sc.Init(this, dir, yOff);
        }
    }

    public void OnSickleReturned()
    {
        _activeSickles--;
        if (_activeSickles <= 0)
        {
            _activeSickles = 0;
            _canShoot = true;
        }
    }

    public void ResetToStart()
    {
        foreach (SickleController sc in FindObjectsOfType<SickleController>())
            if (sc.shooterId == playerCount)
                Destroy(sc.gameObject);
        _activeSickles = 0;
        transform.position = _start;
        if (_rigidbody != null)
            _rigidbody.velocity = Vector2.zero;
        if (_playerWeapon != null)
        {
            _playerWeapon.direction = Vector2.right;
            _playerWeapon.weapon.transform.position = (Vector2)transform.position + Vector2.right * _playerWeapon.aimDistance;
        }
        _canShoot = true;
        currentTime = 0f;
    }
}
