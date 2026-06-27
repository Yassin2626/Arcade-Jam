using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // Static singleton instance allowing global access to the GameState
    public static GameState Instance { get; private set; }
    
    // Reference to the UI script handling player ready statuses and health visuals
    private ReadyView _readyView;
    
    // Ready state tracking for both players during the preparation phase
    private bool _playerOneReady = false;
    private bool _playerTwoReady = false;

    // Remaining health points for each player
    public int playerOneHealth = 3;
    public int playerTwoHealth = 3;
    
    // Player character and gun selection indices
    public int playerOneCharacterIndex = 0;
    public int playerOneGunIndex = 0;
    public int playerTwoCharacterIndex = 0;
    public int playerTwoGunIndex = 0;

    // Input axis and button string prefixes used to map multiplayer controls dynamically
    public string horizontalAxis = "Horizontal_";
    public string verticalAxis = "Vertical_";
    public string jumpButton = "Jump_";
    public string actionX = "Action_X_";
    public string actionB = "Action_B_";
    public string actionY = "Action_Y_";
    public string actionRB = "Action_RB_";
    public string actionLB = "Action_LB_";

    // Defines the possible operational states of the match flow
    public enum GameStateEnum {
        GetReady,
        InMatch,
        GameOver,
    }
    
    // Current active state of the game session
    public GameStateEnum gameState;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        // Enforce the Singleton pattern to prevent duplicate GameState instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Establish this object as the persistent global instance
        Instance = this;
        
        // Initialize the game loop into the pre-match ready phase
        gameState = GameStateEnum.GetReady;
        
        // Cache the ReadyView component responsible for user interface updates
        _readyView = gameObject.GetComponent<ReadyView>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Monitor core match transitions based on current game phases
        switch (gameState) {
            
            // Check if both players are flagged as ready to begin gameplay
            case GameStateEnum.GetReady: {
                if (_playerOneReady && _playerTwoReady) {
                    // Transition to active match state
                    gameState = GameStateEnum.InMatch;
                    
                    // Notify the UI to hide lobby options and display game elements
                    _readyView.SetInMatch();
                }
                break;
            }
            
            // Monitor player health values to determine match termination conditions
            case GameStateEnum.InMatch: {
                if (playerOneHealth <= 0 || playerTwoHealth <= 0) {
                    // Transition to end match state
                    gameState = GameStateEnum.GameOver;
                    
                    // Trigger game over screen, passing the winning player number ("1" or "2")
                    _readyView.SetInGameOver(playerOneHealth <= 0 ? "2" : "1");
                }
                break;
            }
            case GameStateEnum.GameOver:
            break; 
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Deducts life from the specified player and updates corresponding UI components
    public void TakeDamage(string player) {
        switch (player) {
            case "1": {
                playerOneHealth--;
                _readyView.UpdatePlayerHealth(player, playerOneHealth);
                break;
            }
            case "2": {
                playerTwoHealth--;
                _readyView.UpdatePlayerHealth(player, playerTwoHealth);
                break;
            }
        }
    }

    // Commits the ready status flag for a player and triggers the corresponding visual state
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