using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyView : MonoBehaviour {

    // UI Panels for different game states
    public GameObject startScreen;
    public GameObject inMatchScreen;
    public GameObject gameOverScreen;

    // UI elements for Player 1 lobby status
    public Image backgroundPlayerOne;
    public TextMeshProUGUI readyTextMeshProPlayerOne;

    // UI elements for Player 2 lobby status
    public Image backgroundPlayerTwo;
    public TextMeshProUGUI readyTextMeshProPlayerTwo;

    // Text elements to display current health points during the match
    public TextMeshProUGUI healthPlayerOne;
    public TextMeshProUGUI healthPlayerTwo;

    // Text element displaying the final winner on the game over screen
    public TextMeshProUGUI playerWins;

    public Color playerOneColor = new Color(0.47f, 0.82f, 0.55f);
    public Color playerTwoColor = new Color(0.93f, 0.51f, 0.47f);
    public Color goldColor = new Color(0.83f, 0.69f, 0.22f);
    public Color sandColor = new Color(0.96f, 0.87f, 0.70f);
    public Color darkSand = new Color(0.76f, 0.67f, 0.50f);
    public Color darkBg = new Color(0.10f, 0.06f, 0.04f);

    private void Start()
    {
        if (GetComponent<SelectionUI>() == null)
            gameObject.AddComponent<SelectionUI>();

        startScreen.SetActive(true);
        inMatchScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        StyleInMatchScreen();
        StyleGameOverScreen();
    }

    private void StyleInMatchScreen()
    {
        if (inMatchScreen == null) return;
        Image bg = inMatchScreen.GetComponent<Image>();
        if (bg != null) bg.color = new Color(0.10f, 0.06f, 0.04f, 0.85f);

        Image[] panels = inMatchScreen.GetComponentsInChildren<Image>(true);
        foreach (Image p in panels)
        {
            if (p.gameObject == inMatchScreen) continue;
            if (p.name.Contains("Background") || p.name.Contains("Panel"))
            {
                p.color = new Color(0.2f, 0.15f, 0.08f, 0.7f);
            }
        }

        TextMeshProUGUI[] texts = inMatchScreen.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI t in texts)
        {
            if (t.name.Contains("One") && !t.name.Contains("Text"))
            {
                t.color = goldColor;
                t.fontStyle = FontStyles.Bold;
            }
            else if (t.name.Contains("Two") && !t.name.Contains("Text"))
            {
                t.color = goldColor;
                t.fontStyle = FontStyles.Bold;
            }
        }

        Image[] profiles = inMatchScreen.GetComponentsInChildren<Image>(true);
        foreach (Image p in profiles)
        {
            if (p.name.Contains("Profile"))
            {
                if (p.name.Contains("One"))
                    p.color = playerOneColor;
                else if (p.name.Contains("Two"))
                    p.color = playerTwoColor;
            }
        }
    }

    private void StyleGameOverScreen()
    {
        if (gameOverScreen == null) return;
        Image bg = gameOverScreen.GetComponent<Image>();
        if (bg != null) bg.color = new Color(0.05f, 0.03f, 0.0f, 0.95f);

        TextMeshProUGUI[] texts = gameOverScreen.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI t in texts)
        {
            if (t.name.Contains("Game Over") || t.text.Contains("GAME OVER"))
            {
                t.color = goldColor;
                t.fontStyle = FontStyles.Bold;
            }
            else if (t.name.Contains("Wins"))
            {
                t.color = sandColor;
                t.fontStyle = FontStyles.Bold;
            }
            else if (t.name.Contains("Prompt") || t.text.Contains("Jump"))
            {
                t.color = darkSand;
            }
        }
    }

    public void SetReady(string player) {
        switch (player) {
            case "1": {
                if (backgroundPlayerOne != null)
                    backgroundPlayerOne.color = new Color(0.2f, 0.4f, 0.2f, 0.8f);
                if (readyTextMeshProPlayerOne != null)
                {
                    readyTextMeshProPlayerOne.text = "Player 1 ready!";
                    readyTextMeshProPlayerOne.color = playerOneColor;
                }
                break;
            }
            case "2":{
                if (backgroundPlayerTwo != null)
                    backgroundPlayerTwo.color = new Color(0.2f, 0.4f, 0.2f, 0.8f);
                if (readyTextMeshProPlayerTwo != null)
                {
                    readyTextMeshProPlayerTwo.text = "Player 2 ready!";
                    readyTextMeshProPlayerTwo.color = playerTwoColor;
                }
                break;
            }
        }
    }

    public void SetInMatch() {
        startScreen.SetActive(false);
        inMatchScreen.SetActive(true);
        HideSelectionUI();
    }

    public void SetInGameOver(string player) {
        startScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        HideSelectionUI();
        if (playerWins != null)
        {
            playerWins.text = "PLAYER " + player + " VICTORIOUS!";
            playerWins.color = player == "1" ? playerOneColor : playerTwoColor;
        }
    }

    private void HideSelectionUI()
    {
        GameObject panel = GameObject.Find("EgyptianSelectionUI");
        if (panel != null)
            panel.SetActive(false);
    }

    public void UpdatePlayerHealth(string player, int health) {
        switch (player) {
            case "1": {
                if (healthPlayerOne != null)
                    healthPlayerOne.text = health.ToString();
                break;
            }
            case "2":{
                if (healthPlayerTwo != null)
                    healthPlayerTwo.text = health.ToString();
                break;
            }
        }
    }
}