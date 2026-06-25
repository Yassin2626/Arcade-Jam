using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    
    // Reference to the weapon GameObject that moves around the player
    public GameObject weapon;

    // Fixed positions around the player for the weapon to snap to
    public Transform positionRight;
    public Transform positionLeft;
    public Transform positionTop;
    public Transform positionBottom;
    
    // Reference to the player actions script to determine player index/ID
    private PlayerActions _playerActions;
    
    // Current aiming direction, defaults to facing right
    public Vector2 direction  = Vector2.right;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Cache the PlayerActions component attached to this GameObject
        _playerActions = GetComponent<PlayerActions>();
    }

    // Update is called once per frame
    private void Update() {
        // Only allow weapon aiming if the game is currently in a match
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
        
        // Get raw input values (-1, 0, or 1) using the player's specific control axis
        int horizontal = (int) Input.GetAxisRaw(GameState.Instance.horizontalAxis + _playerActions.playerCount);
        int vertical = (int) Input.GetAxisRaw(GameState.Instance.verticalAxis + _playerActions.playerCount);
            
        // Determine weapon positioning and aiming direction based on input values
        switch (horizontal) {
            // No horizontal input and no vertical input (idle/neutral)
            case 0 when vertical == 0: {
                break;   
            }
            // Aiming straight up
            case 0 when vertical == 1:{
                weapon.transform.position = positionTop.position;
                direction = Vector2.up;
                break;   
            }
            // Aiming straight down
            case 0 when vertical == -1:{
                weapon.transform.position = positionBottom.position;
                direction = Vector2.down;
                break;   
            }
            // Aiming straight right
            case 1 when vertical == 0:{
                weapon.transform.position = positionRight.position;
                direction = Vector2.right;
                break;   
            }
            // Aiming straight left
            case -1 when vertical == 0:{
                weapon.transform.position = positionLeft.position;
                direction = Vector2.left;
                break;   
            }
        }
    }
}