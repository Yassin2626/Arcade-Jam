using System;
using TMPro;
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

    public int playerOneCharacterIndex = 1;
    public int playerOneGunIndex = 2;
    public int playerTwoCharacterIndex = 1;
    public int playerTwoGunIndex = 2;

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
        RoundTransition,
        GameOver,
    }

    public GameStateEnum gameState;

    private Canvas _canvas;
    private Canvas _overlayCanvas;
    private TextMeshProUGUI _roundOverText;
    private float _roundTimer;
    private GameObject _healthBarP1;
    private GameObject _healthBarP2;
    private Image _healthFillP1;
    private Image _healthFillP2;
    private Image _shieldFillP1;
    private Image _shieldFillP2;
    private Image _lostHealthFillP1;
    private Image _lostHealthFillP2;

    public Transform player1Transform;
    public Transform player2Transform;

    private GameObject _potionPrefab;
    private GameObject _healthPotionPrefab;
    private float _nextSpawnTime = 0f;
    private float _nextHealthSpawnTime = 0f;

    private int _currentRound = 1;
    private int _playerOneRoundWins = 0;
    private int _playerTwoRoundWins = 0;
    private const int _roundsNeededToWin = 2;
    private SpriteRenderer _backgroundRenderer;

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
        CreateOverlayCanvas();
        CreateHealthBars();
        CreatePotionPrefab();
        CreateHealthPotionPrefab();
        CreateBackground();
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
        bc.size = new Vector2(0.25f, 0.25f);
        _potionPrefab.transform.localScale = Vector3.one * 0.25f;
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
        bc.size = new Vector2(0.2f, 0.2f);
        _healthPotionPrefab.transform.localScale = Vector3.one * 0.2f;
        _healthPotionPrefab.GetComponent<HealthPickup>().healAmount = 25;
        _healthPotionPrefab.SetActive(false);
    }

    private void CreateBackground()
    {
        GameObject bg = new GameObject("RoundBackground", typeof(SpriteRenderer));
        _backgroundRenderer = bg.GetComponent<SpriteRenderer>();
        _backgroundRenderer.sortingOrder = -100;
    }

    private void SetRoundBackground()
    {
        string bgName = _currentRound == 1 ? "bg_round1" : "bg_round2";
        Sprite bgSprite = Resources.Load<Sprite>(bgName);
        if (bgSprite != null)
        {
            _backgroundRenderer.sprite = bgSprite;
            float camHeight = 2f * Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;
            float scale = Mathf.Max(camWidth / bgSprite.bounds.size.x, camHeight / bgSprite.bounds.size.y);
            _backgroundRenderer.transform.localScale = Vector3.one * scale;
        }
    }

    private void StartNextRound()
    {
        _currentRound++;
        playerOneHealth = MaxHealth;
        playerTwoHealth = MaxHealth;
        playerOneShield = 0;
        playerTwoShield = 0;
        foreach (ShieldPickup p in FindObjectsOfType<ShieldPickup>()) Destroy(p.gameObject);
        foreach (HealthPickup p in FindObjectsOfType<HealthPickup>()) Destroy(p.gameObject);
        SetRoundBackground();

        gameState = GameStateEnum.RoundTransition;
        _roundTimer = 2.5f;
        string winner = _playerOneRoundWins > _playerTwoRoundWins ? "1" : "2";
        _roundOverText.text = $"<size=64>ROUND {_currentRound - 1} OVER</size>\n<size=36>PLAYER {winner} WINS THE ROUND</size>\n<size=28>ROUND {_currentRound} INCOMING</size>";
        _roundOverText.gameObject.SetActive(true);
    }

    private void CreateOverlayCanvas()
    {
        GameObject go = new GameObject("HealthBarCanvas", typeof(Canvas), typeof(CanvasScaler));
        _overlayCanvas = go.GetComponent<Canvas>();
        _overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _overlayCanvas.sortingOrder = 100;

        GameObject rtGo = new GameObject("RoundOverText", typeof(RectTransform), typeof(TextMeshProUGUI));
        rtGo.transform.SetParent(go.transform, false);
        _roundOverText = rtGo.GetComponent<TextMeshProUGUI>();
        _roundOverText.fontSize = 52;
        _roundOverText.alignment = TextAlignmentOptions.Center;
        _roundOverText.color = new Color(0.95f, 0.85f, 0.5f);
        _roundOverText.fontStyle = FontStyles.Bold;
        _roundOverText.rectTransform.sizeDelta = new Vector2(800, 300);
        _roundOverText.rectTransform.anchoredPosition = Vector2.zero;
        _roundOverText.gameObject.SetActive(false);
    }

    private void CreateHealthBars()
    {
        _healthBarP1 = CreateHealthBar("HealthBar_P1", true);
        _healthBarP2 = CreateHealthBar("HealthBar_P2", false);
        _healthFillP1 = _healthBarP1.transform.Find("HealthFill").GetComponent<Image>();
        _healthFillP2 = _healthBarP2.transform.Find("HealthFill").GetComponent<Image>();
        _lostHealthFillP1 = _healthBarP1.transform.Find("LostHealthFill").GetComponent<Image>();
        _lostHealthFillP2 = _healthBarP2.transform.Find("LostHealthFill").GetComponent<Image>();
        _shieldFillP1 = _healthBarP1.transform.Find("ShieldFill").GetComponent<Image>();
        _shieldFillP2 = _healthBarP2.transform.Find("ShieldFill").GetComponent<Image>();
        _healthBarP1.SetActive(false);
        _healthBarP2.SetActive(false);
    }

    private Sprite GenerateRoundedRectSprite(int width, int height, float radius)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color white = Color.white;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float cx, cy;
                if (x < radius && y >= height - radius)
                { cx = radius; cy = height - 1 - radius; }
                else if (x >= width - radius && y >= height - radius)
                { cx = width - 1 - radius; cy = height - 1 - radius; }
                else if (x < radius && y < radius)
                { cx = radius; cy = radius; }
                else if (x >= width - radius && y < radius)
                { cx = width - 1 - radius; cy = radius; }
                else { tex.SetPixel(x, y, white); continue; }
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(radius - dist + 0.5f);
                tex.SetPixel(x, y, new Color(white.r, white.g, white.b, alpha));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private GameObject CreateHealthBar(string name, bool isLeft)
    {
        GameObject bar = new GameObject(name, typeof(RectTransform));
        bar.transform.SetParent(_overlayCanvas.transform, false);
        RectTransform rt = bar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420, 42);
        if (isLeft)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20f, -20f);
        }
        else
        {
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-20f, -20f);
        }

        Sprite rounded = GenerateRoundedRectSprite(420, 42, 8);
        Image bg = bar.AddComponent<Image>();
        bg.sprite = rounded;
        bg.color = new Color(0.06f, 0.04f, 0.04f);
        bar.AddComponent<Mask>();

        GameObject healthFill = new GameObject("HealthFill", typeof(RectTransform), typeof(Image));
        healthFill.transform.SetParent(bar.transform, false);
        RectTransform hfr = healthFill.GetComponent<RectTransform>();
        hfr.anchorMin = Vector2.zero;
        hfr.anchorMax = new Vector2(1f, 1f);
        hfr.sizeDelta = Vector2.zero;
        hfr.pivot = new Vector2(0f, 0.5f);
        hfr.GetComponent<Image>().color = new Color(0.15f, 0.9f, 0.25f);

        GameObject lostFill = new GameObject("LostHealthFill", typeof(RectTransform), typeof(Image));
        lostFill.transform.SetParent(bar.transform, false);
        RectTransform lfr = lostFill.GetComponent<RectTransform>();
        lfr.anchorMin = Vector2.zero;
        lfr.anchorMax = Vector2.zero;
        lfr.sizeDelta = Vector2.zero;
        lfr.pivot = new Vector2(0f, 0.5f);
        lfr.GetComponent<Image>().color = new Color(0.85f, 0.12f, 0.12f);

        GameObject shieldFill = new GameObject("ShieldFill", typeof(RectTransform), typeof(Image));
        shieldFill.transform.SetParent(bar.transform, false);
        RectTransform sfr = shieldFill.GetComponent<RectTransform>();
        sfr.anchorMin = new Vector2(0.7f, 0f);
        sfr.anchorMax = new Vector2(0.7f, 1f);
        sfr.sizeDelta = Vector2.zero;
        sfr.pivot = new Vector2(0f, 0.5f);
        sfr.GetComponent<Image>().color = new Color(0.15f, 0.5f, 0.95f);

        return bar;
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
                    _currentRound = 1;
                    _playerOneRoundWins = 0;
                    _playerTwoRoundWins = 0;
                    playerOneHealth = MaxHealth;
                    playerTwoHealth = MaxHealth;
                    playerOneShield = 0;
                    playerTwoShield = 0;
                    gameState = GameStateEnum.InMatch;
                    SetRoundBackground();
                    _readyView.SetInMatch();
                    ShowHealthBars();
                    _nextSpawnTime = Time.time + UnityEngine.Random.Range(10f, 15f);
                    _nextHealthSpawnTime = Time.time + UnityEngine.Random.Range(12f, 17f);
                }
                break;
            }
            case GameStateEnum.InMatch: {
                if (playerOneHealth <= 0 || playerTwoHealth <= 0) {
                    string roundWinner = playerOneHealth <= 0 ? "2" : "1";
                    if (roundWinner == "1") _playerOneRoundWins++;
                    else _playerTwoRoundWins++;
                    if (_playerOneRoundWins >= _roundsNeededToWin || _playerTwoRoundWins >= _roundsNeededToWin)
                    {
                        gameState = GameStateEnum.GameOver;
                        HideHealthBars();
                        _readyView.SetInGameOver(_playerOneRoundWins > _playerTwoRoundWins ? "1" : "2");
                    }
                    else
                    {
                        StartNextRound();
                    }
                }
                UpdateHealthBars();
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
            case GameStateEnum.RoundTransition: {
                UpdateHealthBars();
                _roundTimer -= Time.deltaTime;
                if (_roundTimer <= 0f)
                {
                    _roundOverText.gameObject.SetActive(false);
                    gameState = GameStateEnum.InMatch;
                    _nextSpawnTime = Time.time + UnityEngine.Random.Range(5f, 8f);
                    _nextHealthSpawnTime = Time.time + UnityEngine.Random.Range(7f, 10f);
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
        UpdateHealthBars();
    }

    private void HideHealthBars()
    {
        if (_healthBarP1 != null) _healthBarP1.SetActive(false);
        if (_healthBarP2 != null) _healthBarP2.SetActive(false);
    }

    private void UpdateHealthBars()
    {
        UpdateHealthBarFill(_healthFillP1, _lostHealthFillP1, _shieldFillP1, playerOneHealth, playerOneShield);
        UpdateHealthBarFill(_healthFillP2, _lostHealthFillP2, _shieldFillP2, playerTwoHealth, playerTwoShield);
    }

    private void UpdateHealthBarFill(Image healthFill, Image lostHealthFill, Image shieldFill, int health, int shield)
    {
        if (healthFill == null) return;
        float hp = Mathf.Clamp01((float)health / MaxHealth);
        float sp = Mathf.Clamp01((float)shield / MaxShield);
        healthFill.rectTransform.anchorMax = new Vector2(hp * 0.7f, 1f);
        lostHealthFill.rectTransform.anchorMin = new Vector2(hp * 0.7f, 0f);
        lostHealthFill.rectTransform.anchorMax = new Vector2(0.7f, 1f);
        shieldFill.rectTransform.anchorMin = new Vector2(0.7f, 0f);
        shieldFill.rectTransform.anchorMax = new Vector2(0.7f + sp * 0.3f, 1f);
        shieldFill.gameObject.SetActive(shield > 0);
    }

    private void UpdateHealthBarPosition(GameObject bar, Transform player, Image healthFill, Image lostHealthFill, Image shieldFill, int health, int shield)
    {
        if (bar == null || player == null || healthFill == null) return;
        bar.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(player.position + Vector3.up * 2.5f);
        float hp = Mathf.Clamp01((float)health / MaxHealth);
        float sp = Mathf.Clamp01((float)shield / MaxShield);
        healthFill.rectTransform.anchorMax = new Vector2(hp * 0.7f, 1f);
        lostHealthFill.rectTransform.anchorMin = new Vector2(hp * 0.7f, 0f);
        lostHealthFill.rectTransform.anchorMax = new Vector2(0.7f, 1f);
        shieldFill.rectTransform.anchorMin = new Vector2(0.7f, 0f);
        shieldFill.rectTransform.anchorMax = new Vector2(0.7f + sp * 0.3f, 1f);
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
