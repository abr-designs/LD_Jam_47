using UnityEngine;

[CreateAssetMenu(fileName = "Simple Animation", menuName = "Create/Custom/Simple Animation")]
public class SimpleAnimationScriptableObject : ScriptableObject
{

    public Sprite[] Sprites => sprites;

    public float FrameTime => frameTime;

    public bool FlipX => flipX;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private float frameTime;

    [SerializeField]
    private bool flipX;
    
    
    /*private int _currentIndex;

    public bool Init(out Sprite sprite)
    {
        _t = 0f;
        _currentIndex = 0;

        sprite = sprites[_currentIndex];
        return flipX;
    }

    public bool NextFrame(float t, out Sprite sprite)
    {
        sprite = null;
        _t += deltaTime;

        if (_t <= frameTime) 
            return false;
        
        
        _currentIndex++;

        if (_currentIndex >= sprites.Length)
            _currentIndex = 0;

        sprite = sprites[_currentIndex];
            
        return true;

    }*/
}
