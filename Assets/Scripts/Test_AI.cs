using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_AI : MonoBehaviour
{
    [SerializeField]
    private Sprite ForwardSprite;
    [SerializeField]
    private Sprite LeftSprite;
    [SerializeField]
    private Sprite RightSprite;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    
    [SerializeField]
    private Transform mainTransform;
    
    [SerializeField]
    private new Rigidbody rigidbody;
    

    [SerializeField]
    private float forwardForce;
    [SerializeField]
    private float turnForce;
    
    private bool replaying;
    private int targetIndex;
    private int currentIndex;
    private IReadOnlyList<RB_Move.InputEvent> _inputEvents;
    private float _t;
    private float lastDeltaTime;

    private bool _forward, _left, _right;


    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!replaying)
            return;

        NextFrame();
        
        /*if (rigidbody is null)
            return;

        var forward = mainTransform.forward.normalized;
        
        if(_forward)
            rigidbody.AddForce(forward * forwardForce, ForceMode.Impulse);

        if (_right)
        {
            mainTransform.rotation *= Quaternion.Euler(Vector3.up * (turnForce * Time.deltaTime));
            rigidbody.velocity = mainTransform.forward.normalized * rigidbody.velocity.magnitude;
            
            spriteRenderer.sprite = RightSprite;
        }
        else if(_left)
        {
            mainTransform.rotation *= Quaternion.Euler(Vector3.up * (-turnForce * Time.deltaTime));
            rigidbody.velocity = mainTransform.forward.normalized * rigidbody.velocity.magnitude;
            
            spriteRenderer.sprite = LeftSprite;
        }
        else
        {
            spriteRenderer.sprite = ForwardSprite;
        }
        
        Debug.DrawLine( mainTransform.position,  mainTransform.position + rigidbody.velocity.normalized, Color.green);*/

        

    }

    private void FixedUpdate()
    {
        mainTransform.position = rigidbody.position;
    }

    //Others
    //====================================================================================================================//
    public void PlayBack(IReadOnlyList<RB_Move.InputEvent> inputEvents)
    {
        _inputEvents = inputEvents;
        targetIndex = 1;
        currentIndex = 0;
        _t = 0;

        replaying = true;
    }

    private void NextFrame()
    {
        _t += Time.deltaTime;
        
        
        var target = _inputEvents[targetIndex];
        var current = _inputEvents[currentIndex];

        var time = target.time - current.time;

        if (time < 0) 
            time = lastDeltaTime;

        var t = _t / time;

        if (t >= 1f)
        {
            lastDeltaTime = time;
            currentIndex++;
            targetIndex++;

            if (targetIndex >= _inputEvents.Count)
            {
                targetIndex = 0;
            }

            if (currentIndex >= _inputEvents.Count)
            {
                currentIndex = 0;
            }

            switch (_inputEvents[currentIndex].sprite)
            {
                case RB_Move.SPRITE.FORWARD:
                    spriteRenderer.sprite = ForwardSprite;
                    break;
                case RB_Move.SPRITE.LEFT:
                    spriteRenderer.sprite = LeftSprite;
                    break;
                case RB_Move.SPRITE.RIGHT:
                    spriteRenderer.sprite = RightSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _t = 0f;
            return;
        }

        rigidbody.position = Vector3.Lerp(current.position, target.position, t);
        mainTransform.forward = Vector3.Lerp(current.direction, target.direction, t);


        /*_t += Time.deltaTime;

        if (_t < _inputEvents[currentIndex].time)
            return;


        switch (_inputEvents[currentIndex].key)
        {
            case RB_Move.KEY.FORWARD:
                _forward = _inputEvents[currentIndex].state;
                break;
            case RB_Move.KEY.LEFT:
                _left = _inputEvents[currentIndex].state;
                break;
            case RB_Move.KEY.RIGHT:
                _right = _inputEvents[currentIndex].state;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentIndex++;

        if (currentIndex >= _inputEvents.Count)
            currentIndex = 0;*/

    }

    private void OnDrawGizmosSelected()
    {
        if (!replaying)
            return;

        for (int i = 1; i < _inputEvents.Count; i++)
        {
            var temp = _inputEvents[i - 1];
            Gizmos.DrawWireSphere(temp.position, 0.3f);
            Gizmos.DrawLine(_inputEvents[i].position, temp.position);
        }
        Gizmos.DrawLine(_inputEvents[_inputEvents.Count - 1].position, _inputEvents[0].position);
    }

    //====================================================================================================================//
    
}
