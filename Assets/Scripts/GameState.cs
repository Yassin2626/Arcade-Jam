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
    public int playerOneHealth = MaxHealth;
    public int playerTwoHealth = MaxHealth;

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

    public Transform player1Transform;
    public Transform player2Transform;

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
    }

    private void CreateHealthBars()
    {
        _healthBarP1 = CreateHealthBar("HealthBar_P1", -300f);
        _healthBarP2 = CreateHealthBar("HealthBar_P2", 300f);
        _healthFillP1 = _healthBarP1.transform.Find("Fill").GetComponent<Image>();
        _healthFillP2 = _healthBarP2.transform.Find("Fill").GetComponent<Image>();
        _healthBarP1.SetActive(false);
        _healthBarP2.SetActive(false);
    }

    private GameObject CreateHealthBar(string name, float x)
    {
        GameObject container = new GameObject(name, typeof(RectTransform));
        container.transform.SetParent(_canvas.transform, false);
        RectTransform cr = container.GetComponent<RectTransform>();
        cr.sizeDelta = new Vector2(120, 14);
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

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(container.transform, false);
        RectTransform fr = fill.GetComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = new Vector2(1f, 1f);
        fr.sizeDelta = Vector2.zero;
        fr.pivot = new Vector2(0f, 0.5f);
        Image fi = fill.GetComponent<Image>();
        fi.color = new Color(0.2f, 0.7f, 0.2f);

        return container;
    }

    public int GetDamage(int gunIndex)
    {
        if (gunIndex < 0 || gunIndex >= _baseDamage.Length) return _baseDamage[0];
        return Mathf.RoundToInt(_baseDamage[gunIndex] * _damageMultiplier[gunIndex]);
    }

    private void Update()
    {
        switch (gameState) {
            case GameStateEnum.GetReady: {
                if (_playerOneReady && _playerTwoReady) {
                    playerOneHealth = MaxHealth;
                    playerTwoHealth = MaxHealth;
                    gameState = GameStateEnum.InMatch;
                    _readyView.SetInMatch();
                    ShowHealthBars();
                }
                break;
            }
            case GameStateEnum.InMatch: {
                if (playerOneHealth <= 0 || playerTwoHealth <= 0) {
                    gameState = GameStateEnum.GameOver;
                    HideHealthBars();
                    _readyView.SetInGameOver(playerOneHealth <= 0 ? "2" : "1");
                }
                UpdateHealthBarPosition(_healthBarP1, player1Transform, _healthFillP1, playerOneHealth);
                UpdateHealthBarPosition(_healthBarP2, player2Transform, _healthFillP2, playerTwoHealth);
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

    private void UpdateHealthBarPosition(GameObject bar, Transform player, Image fill, int health)
    {
        if (bar == null || player == null || fill == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + Vector3.up * 2.5f);
        RectTransform rt = bar.GetComponent<RectTransform>();
        rt.position = screenPos;
        float pct = Mathf.Clamp01((float)health / MaxHealth);
        fill.rectTransform.anchorMax = new Vector2(pct, 1f);
    }

    public void TakeDamage(string player, int amount) {
        switch (player) {
            case "1": {
                playerOneHealth = Mathf.Max(0, playerOneHealth - amount);
                break;
            }
            case "2": {
                playerTwoHealth = Mathf.Max(0, playerTwoHealth - amount);
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
