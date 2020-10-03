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
    private int currentIndex;
    private List<RB_Move.ButtonEvent> _buttonEvents;
    private float _t;

    private bool _forward, _left, _right;


    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!replaying)
            return;

        NextFrame();
        
        if (rigidbody is null)
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
        
        Debug.DrawLine( mainTransform.position,  mainTransform.position + rigidbody.velocity.normalized, Color.green);

        

    }

    private void FixedUpdate()
    {
        mainTransform.position = rigidbody.position;
    }

    //Others
    //====================================================================================================================//
    public void PlayBack(List<RB_Move.ButtonEvent> buttonEvents)
    {
        _buttonEvents = buttonEvents;
        currentIndex = 0;
        _t = 0;

        replaying = true;
    }

    private void NextFrame()
    {
        _t += Time.deltaTime;

        if (_t < _buttonEvents[currentIndex].time)
            return;


        switch (_buttonEvents[currentIndex].key)
        {
            case RB_Move.KEY.FORWARD:
                _forward = _buttonEvents[currentIndex].state;
                break;
            case RB_Move.KEY.LEFT:
                _left = _buttonEvents[currentIndex].state;
                break;
            case RB_Move.KEY.RIGHT:
                _right = _buttonEvents[currentIndex].state;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentIndex++;

        if (currentIndex >= _buttonEvents.Count)
            currentIndex = 0;

    }

    //====================================================================================================================//
    
}
