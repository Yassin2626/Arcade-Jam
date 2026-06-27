using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private SpriteRenderer _handRight;
    private SpriteRenderer _handLeft;
    private Sprite _originalRight;
    private Sprite _originalLeft;
    private Sprite _gunHand;
    private PlayerActions _playerActions;
    private int _lastGunIndex = -1;

    private void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        FindHands(transform);

        if (_handRight != null) _originalRight = _handRight.sprite;
        if (_handLeft != null) _originalLeft = _handLeft.sprite;

        _gunHand = Resources.Load<Sprite>("gun_hand");
    }

    private void FindHands(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "pixil-frame-0 (1)_6" && _handLeft == null)
                _handLeft = child.GetComponent<SpriteRenderer>();
            else if (child.name == "pixil-frame-0 (1)_6 (1)" && _handRight == null)
                _handRight = child.GetComponent<SpriteRenderer>();
            FindHands(child);
        }
    }

    private void Update()
    {
        if (GameState.Instance == null) return;
        int gunIdx = _playerActions.playerCount == "1"
            ? GameState.Instance.playerOneGunIndex
            : GameState.Instance.playerTwoGunIndex;

        if (gunIdx == _lastGunIndex) return;
        _lastGunIndex = gunIdx;

        if (gunIdx == 0 && _gunHand != null)
        {
            if (_handLeft != null) _handLeft.sprite = _gunHand;
            if (_handRight != null) _handRight.sprite = _gunHand;
        }
        else
        {
            if (_handLeft != null && _originalLeft != null) _handLeft.sprite = _originalLeft;
            if (_handRight != null && _originalRight != null) _handRight.sprite = _originalRight;
        }
    }
}
