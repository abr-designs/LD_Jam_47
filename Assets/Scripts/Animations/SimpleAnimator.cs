using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SimpleAnimationScriptableObject[] states;

    private STATE _currentState = STATE.NONE;


    private Sprite[] _sprites;
    private int _currentIndex;
    private float _delay;
    private float _speed;
    private float _t;

    //Unity Functions
    //====================================================================================================================//
    
    private void LateUpdate()
    {
        if (_sprites == null)
            return;

        if (_delay == 0f)
            return;
        
        _t += Time.deltaTime * _speed;

        if (_t < _delay)
            return;

        _currentIndex++;

        if (_currentIndex >= _sprites.Length)
            _currentIndex = 0;

        spriteRenderer.sprite = _sprites[_currentIndex];
        _t = 0f;
    }

    //Simple Animator Functions
    //====================================================================================================================//
    
    public void SetState(STATE newState)
    {
        if (_currentState == newState)
            return;
        
        _currentState = newState;

        
        var stateData = states[(int) _currentState];

        _sprites = stateData.Sprites;
        _t = 0f;
        _delay = stateData.FrameTime;
        _currentIndex = 0;


        spriteRenderer.flipX = stateData.FlipX;
        spriteRenderer.sprite = _sprites[_currentIndex];
    }

    public void SetSpeed(float normalizedSpeed)
    {
        _speed = Mathf.Clamp01(normalizedSpeed);
    }
}
