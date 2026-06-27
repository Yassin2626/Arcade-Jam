using System;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    private ReadyView _readyView;

    private bool _playerOneReady = false;
    private bool _playerTwoReady = false;

    public const int MaxHealth = 100;
    public const int MaxShield = 50;
    public int playerOneHealth = MaxHealth;
    public int playerTwoHealth = MaxHealth;
    public int playerOneShield = 0;
    public int playerTwoShield = 0;

    public int playerOneCharacterIndex = 0;
    public int playerOneGunIndex = 0;
    public int playerTwoCharacterIndex = 0;
    public int playerTwoGunIndex = 0;

    private int[] _baseDamage = { 10, 10, 10 };
    private float[] _damageMultiplier = { 1f, 1f, 2f };

    public string horizontalAxis = "Horizontal_";
    public string verticalAxis = "Vertical_";
    public string jumpButton = "Jump_";
    public string actionX = "Action_X_";
    public string actionB = "Action_B_";
    public string actionY = "Action_Y_";
    public string actionRB = "Action_RB_";
    public string actionLB = "Action_LB_";

    public enum GameStateEnum {
        GetReady,
        InMatch,
        GameOver,
    }

    public GameStateEnum gameState;

    private Canvas _canvas;
    private GameObject _healthBarP1;
    private GameObject _healthBarP2;
    private Image _healthFillP1;
    private Image _healthFillP2;
    private Image _shieldFillP1;
    private Image _shieldFillP2;

    public Transform player1Transform;
    public Transform player2Transform;

    private GameObject _potionPrefab;
    private GameObject _healthPotionPrefab;
    private float _nextSpawnTime = 0f;
    private float _nextHealthSpawnTime = 0f;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameState = GameStateEnum.GetReady;
        _readyView = gameObject.GetComponent<ReadyView>();
        _canvas = FindObjectOfType<Canvas>();
        CreateHealthBars();
        CreatePotionPrefab();
        CreateHealthPotionPrefab();
    }

    private void CreatePotionPrefab()
    {
        _potionPrefab = new GameObject("ShieldPickup", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(ShieldPickup));
        Sprite potionSprite = Resources.Load<Sprite>("shield_potion");
        SpriteRenderer sr = _potionPrefab.GetComponent<SpriteRenderer>();
        if (potionSprite != null) sr.sprite = potionSprite;
        sr.sortingOrder = 10;
        BoxCollider2D bc = _potionPrefab.GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.5f, 0.5f);
        _potionPrefab.transform.localScale = Vector3.one * 0.5f;
        _potionPrefab.GetComponent<ShieldPickup>().shieldAmount = 50;
        _potionPrefab.SetActive(false);
    }

    private void CreateHealthPotionPrefab()
    {
        _healthPotionPrefab = new GameObject("HealthPickup", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(HealthPickup));
        Sprite potionSprite = Resources.Load<Sprite>("health_potion");
        SpriteRenderer sr = _healthPotionPrefab.GetComponent<SpriteRenderer>();
        if (potionSprite != null) sr.sprite = potionSprite;
        sr.sortingOrder = 10;
        BoxCollider2D bc = _healthPotionPrefab.GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.5f, 0.5f);
        _healthPotionPrefab.transform.localScale = Vector3.one * 0.5f;
        _healthPotionPrefab.GetComponent<HealthPickup>().healAmount = 25;
        _healthPotionPrefab.SetActive(false);
    }

    private void CreateHealthBars()
    {
        _healthBarP1 = CreateHealthBar("HealthBar_P1");
        _healthBarP2 = CreateHealthBar("HealthBar_P2");
        _healthFillP1 = _healthBarP1.transform.Find("HealthFill").GetComponent<Image>();
        _healthFillP2 = _healthBarP2.transform.Find("HealthFill").GetComponent<Image>();
        _shieldFillP1 = _healthBarP1.transform.Find("ShieldFill").GetComponent<Image>();
        _shieldFillP2 = _healthBarP2.transform.Find("ShieldFill").GetComponent<Image>();
        _healthBarP1.SetActive(false);
        _healthBarP2.SetActive(false);
    }

    private GameObject CreateHealthBar(string name)
    {
        GameObject container = new GameObject(name, typeof(RectTransform));
        container.transform.SetParent(_canvas.transform, false);
        RectTransform cr = container.GetComponent<RectTransform>();
        cr.sizeDelta = new Vector2(120, 22);
        cr.anchorMin = new Vector2(0.5f, 0.5f);
        cr.anchorMax = new Vector2(0.5f, 0.5f);
        cr.pivot = new Vector2(0.5f, 0.5f);

        GameObject bg = new GameObject("BG", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(container.transform, false);
        RectTransform br = bg.GetComponent<RectTransform>();
        br.anchorMin = Vector2.zero;
        br.anchorMax = Vector2.one;
        br.sizeDelta = Vector2.zero;
        Image bi = bg.GetComponent<Image>();
        bi.color = new Color(0.3f, 0.08f, 0.08f);

        GameObject shieldFill = new GameObject("ShieldFill", typeof(RectTransform), typeof(Image));
        shieldFill.transform.SetParent(container.transform, false);
        RectTransform sfr = shieldFill.GetComponent<RectTransform>();
        sfr.anchorMin = new Vector2(0f, 0.65f);
        sfr.anchorMax = new Vector2(0f, 1f);
        sfr.sizeDelta = Vector2.zero;
        sfr.pivot = new Vector2(0f, 0.5f);
        Image sfi = shieldFill.GetComponent<Image>();
        sfi.color = new Color(0.1f, 0.3f, 0.8f);

        GameObject healthFill = new GameObject("HealthFill", typeof(RectTransform), typeof(Image));
        healthFill.transform.SetParent(container.transform, false);
        RectTransform hfr = healthFill.GetComponent<RectTransform>();
        hfr.anchorMin = new Vector2(0f, 0f);
        hfr.anchorMax = new Vector2(0f, 0.65f);
        hfr.sizeDelta = Vector2.zero;
        hfr.pivot = new Vector2(0f, 0.5f);
        Image hfi = healthFill.GetComponent<Image>();
        hfi.color = new Color(0.2f, 0.7f, 0.2f);

        return container;
    }

    public int GetDamage(int gunIndex)
    {
        if (gunIndex < 0 || gunIndex >= _baseDamage.Length) return _baseDamage[0];
        return Mathf.RoundToInt(_baseDamage[gunIndex] * _damageMultiplier[gunIndex]);
    }

    public void AddShield(string player, int amount)
    {
        switch (player)
        {
            case "1": playerOneShield = Mathf.Min(MaxShield, playerOneShield + amount); break;
            case "2": playerTwoShield = Mathf.Min(MaxShield, playerTwoShield + amount); break;
        }
    }

    public void HealPlayer(string player, int amount)
    {
        switch (player)
        {
            case "1": playerOneHealth = Mathf.Min(MaxHealth, playerOneHealth + amount); break;
            case "2": playerTwoHealth = Mathf.Min(MaxHealth, playerTwoHealth + amount); break;
        }
    }

    private void SpawnPotion()
    {
        float x = UnityEngine.Random.Range(-7f, 7f);
        float y = UnityEngine.Random.Range(-3f, 3f);
        Vector3 pos = new Vector3(x, y, 0);
        GameObject potion = Instantiate(_potionPrefab, pos, Quaternion.identity);
        potion.SetActive(true);
    }

    private void SpawnHealthPotion()
    {
        float x = UnityEngine.Random.Range(-7f, 7f);
        float y = UnityEngine.Random.Range(-3f, 3f);
        Vector3 pos = new Vector3(x, y, 0);
        GameObject potion = Instantiate(_healthPotionPrefab, pos, Quaternion.identity);
        potion.SetActive(true);
    }

    private void Update()
    {
        switch (gameState) {
            case GameStateEnum.GetReady: {
                if (_playerOneReady && _playerTwoReady) {
                    playerOneHealth = MaxHealth;
                    playerTwoHealth = MaxHealth;
                    playerOneShield = 0;
                    playerTwoShield = 0;
                    gameState = GameStateEnum.InMatch;
                    _readyView.SetInMatch();
                    ShowHealthBars();
                    _nextSpawnTime = Time.time + UnityEngine.Random.Range(10f, 15f);
                    _nextHealthSpawnTime = Time.time + UnityEngine.Random.Range(12f, 17f);
                }
                break;
            }
            case GameStateEnum.InMatch: {
                if (playerOneHealth <= 0 || playerTwoHealth <= 0) {
                    gameState = GameStateEnum.GameOver;
                    HideHealthBars();
                    _readyView.SetInGameOver(playerOneHealth <= 0 ? "2" : "1");
                }
                UpdateHealthBarPosition(_healthBarP1, player1Transform, _healthFillP1, _shieldFillP1, playerOneHealth, playerOneShield);
                UpdateHealthBarPosition(_healthBarP2, player2Transform, _healthFillP2, _shieldFillP2, playerTwoHealth, playerTwoShield);
                if (Time.time >= _nextSpawnTime)
                {
                    SpawnPotion();
                    _nextSpawnTime = Time.time + UnityEngine.Random.Range(10f, 15f);
                }
                if (Time.time >= _nextHealthSpawnTime)
                {
                    SpawnHealthPotion();
                    _nextHealthSpawnTime = Time.time + UnityEngine.Random.Range(10f, 15f);
                }
                break;
            }
            case GameStateEnum.GameOver:
            break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ShowHealthBars()
    {
        if (_healthBarP1 != null) _healthBarP1.SetActive(true);
        if (_healthBarP2 != null) _healthBarP2.SetActive(true);
    }

    private void HideHealthBars()
    {
        if (_healthBarP1 != null) _healthBarP1.SetActive(false);
        if (_healthBarP2 != null) _healthBarP2.SetActive(false);
    }

    private void UpdateHealthBarPosition(GameObject bar, Transform player, Image healthFill, Image shieldFill, int health, int shield)
    {
        if (bar == null || player == null || healthFill == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + Vector3.up * 2.5f);
        RectTransform rt = bar.GetComponent<RectTransform>();
        rt.position = screenPos;
        float hp = Mathf.Clamp01((float)health / MaxHealth);
        float sp = Mathf.Clamp01((float)shield / MaxShield);
        healthFill.rectTransform.anchorMax = new Vector2(hp, 0.65f);
        shieldFill.rectTransform.anchorMax = new Vector2(sp, 1f);
        shieldFill.gameObject.SetActive(shield > 0);
    }

    public void TakeDamage(string player, int amount) {
        switch (player) {
            case "1": {
                int remaining = amount;
                if (playerOneShield > 0)
                {
                    int shieldAbsorb = Mathf.Min(playerOneShield, remaining);
                    playerOneShield -= shieldAbsorb;
                    remaining -= shieldAbsorb;
                }
                playerOneHealth = Mathf.Max(0, playerOneHealth - remaining);
                break;
            }
            case "2": {
                int remaining = amount;
                if (playerTwoShield > 0)
                {
                    int shieldAbsorb = Mathf.Min(playerTwoShield, remaining);
                    playerTwoShield -= shieldAbsorb;
                    remaining -= shieldAbsorb;
                }
                playerTwoHealth = Mathf.Max(0, playerTwoHealth - remaining);
                break;
            }
        }
    }

    public void SetReady(string player) {
        switch (player) {
            case "1": {
                _playerOneReady = true;
                _readyView.SetReady(player);
                break;
            }
            case "2": {
                _playerTwoReady = true;
                _readyView.SetReady(player);
                break;
            }
        }
    }

    public void SetCharacterSelection(string player, int index) {
        switch (player) {
            case "1": playerOneCharacterIndex = index; break;
            case "2": playerTwoCharacterIndex = index; break;
        }
    }

    public void SetGunSelection(string player, int index) {
        switch (player) {
            case "1": playerOneGunIndex = index; break;
            case "2": playerTwoGunIndex = index; break;
        }
    }
}
