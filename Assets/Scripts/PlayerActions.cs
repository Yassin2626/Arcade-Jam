using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerActions : MonoBehaviour {
    // Identifier string for the player (e.g., "1", "2") to map multiplayer inputs
    public string playerCount = "1";
    
    // Stores the initial spawn position of the player
    private Vector3 _start;
    
    // Reference to the 2D Rigidbody component for resetting velocity on respawn
    private Rigidbody2D _rigidbody;
    
    // Reference to the PlayerWeapon component to fetch aiming direction and spawn point
    private PlayerWeapon _playerWeapon;
    
    // The bullet/projectile prefab to instantiate when shooting
    public GameObject xObject;
    
    // Custom color applied to the spawned projectile's sprite
    public Color bulletColor; 
    
    // Physics layers that the spawned projectile should ignore
    public LayerMask layersToExclude;
    
    // Cooldown duration between weapon shots
    public float spawnInterval = 2f;
    
    // Tracks the remaining cooldown time before the player can shoot again
    public float currentTime = 0f;
    
    // Flag checking if the weapon is off cooldown and ready to fire
    private bool _canShoot = true;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Record the initial starting position for respawning purposes
        _start = gameObject.transform.position;
        
        // Cache the Rigidbody2D and PlayerWeapon components attached to this GameObject
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerWeapon = GetComponent<PlayerWeapon>();
    }

    // Update is called once per frame
    private void Update() {
        // Execute input logic based on the current state of the game manager
        switch (GameState.Instance.gameState) {
            
            // Handles input during the pre-match preparation phase
            case GameState.GameStateEnum.GetReady: {
                // If the player presses their designated jump button, set their ready status
                if (Input.GetButtonDown(GameState.Instance.jumpButton + playerCount)) {
                    GameState.Instance.SetReady(playerCount);
                }
                break;
            }
            
            // Handles input during an active gameplay match
            case GameState.GameStateEnum.InMatch: {
                
                // Primary action button (X Button) to handle weapon shooting
                if (Input.GetButtonDown(GameState.Instance.actionX + playerCount)) {
                    // Prevent shooting if the weapon is still cooling down
                    if (!_canShoot) return;
                    
                    // Reset the cooldown timer and lock shooting
                    currentTime = spawnInterval;
                    _canShoot = false;
                    
                    // Locate the weapon position to use as the spawn point location
                    Transform spawnPoint = _playerWeapon.weapon.transform;
                    
                    // Instantiate the projectile at the weapon's position and rotation
                    GameObject newObject = Instantiate(xObject, spawnPoint.position, spawnPoint.rotation);
                    
                    // Color the projectile's child sprite component to match the designated player color
                    newObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = bulletColor;
                    
                    // Configure the projectile's 2D Rigidbody layer exclusions
                    newObject.GetComponent<Rigidbody2D>().excludeLayers = layersToExclude;
                    
                    // Pass the current aiming direction from the PlayerWeapon script to the projectile
                    newObject.GetComponent<BulletController>().SetDirection(_playerWeapon.direction);
                }

                // Debug tracking for secondary interaction buttons (B, Y, RB, LB)
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

                // Process active weapon cooldown timer
                if (!_canShoot) {
                    currentTime -= Time.deltaTime;
                    // Unlock weapon shooting once the timer expires
                    if (currentTime < 0) {
                        _canShoot = true;
                    }
                }
                break;
            }
            
            // Handles input when the match concludes
            case GameState.GameStateEnum.GameOver: {
                // Restart the current scene if the player presses their jump button
                if (Input.GetButtonDown(GameState.Instance.jumpButton + playerCount)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    // Triggered when the player enters a 2D trigger zone
    private void OnTriggerEnter2D(Collider2D collision) { 
        // Route behavior depending on the tag of the overlapping object
        switch (collision.tag) {
            // Handles player death hazards
            case "Death": {
                // Reset player position back to the recorded starting coordinates
                transform.position = _start;
                
                // Kill any residual physical movement momentum instantly
                _rigidbody.velocity = Vector2.zero;
                
                // Report the death to the global game state to decrease health/lives
                GameState.Instance.TakeDamage(playerCount);
                break;
            }
        }
    }
}