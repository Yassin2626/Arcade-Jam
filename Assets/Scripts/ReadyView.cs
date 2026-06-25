using System.Collections;
using System.Collections.Generic;
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
    
    // Visual indicator color applied to panels when a player becomes ready
    public Color backgroundColor = Color.green;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Enforce correct UI panel visibility at initial match launch
        startScreen.SetActive(true);
        inMatchScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    // Updates a specific player's UI indicators to reflect their ready status
    public void SetReady(string player) {
        switch (player) {
            case "1": {
                backgroundPlayerOne.color = backgroundColor;
                readyTextMeshProPlayerOne.text = "Player 1 ready!";
                break;   
            }
            case "2":{
                backgroundPlayerTwo.color = backgroundColor;
                readyTextMeshProPlayerTwo.text = "Player 2 ready!";
                break;   
            }
        }
    }

    // Disables pre-game UI panels and enables active gameplay overlays
    public void SetInMatch() {
        startScreen.SetActive(false);
        inMatchScreen.SetActive(true);
    }

    // Displays the post-game summary panel and announces the winning player
    public void SetInGameOver(string player) {
        startScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        playerWins.text = "Player " + player + " wins!";
    }

    // Refreshes the on-screen health point value for the specified player
    public void UpdatePlayerHealth(string player, int health) {
        switch (player) {
            case "1": {
                healthPlayerOne.text = health.ToString();
                break;   
            }
            case "2":{
                healthPlayerTwo.text = health.ToString();
                break;   
            }
        }
    }
}