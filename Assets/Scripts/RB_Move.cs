using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Move : MonoBehaviour
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
    private Transform cameraTranform;

    [SerializeField]
    private Transform mainTransform;
    
    [SerializeField]
    private new Rigidbody rigidbody;
    

    [SerializeField]
    private float forwardForce;
    [SerializeField]
    private float turnForce;


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
        ReadInputs();
        
        if (rigidbody is null)
            return;

        var forward = Vector3.ProjectOnPlane(cameraTranform.forward.normalized, Vector3.up);
        
        if(Input.GetKey(KeyCode.W))
            rigidbody.AddForce(forward * forwardForce, ForceMode.Impulse);

        if (Input.GetKey(KeyCode.D))
        {
            mainTransform.rotation *= Quaternion.Euler(Vector3.up * (turnForce * Time.deltaTime));
            rigidbody.velocity = mainTransform.forward.normalized * rigidbody.velocity.magnitude;
            
            spriteRenderer.sprite = RightSprite;
        }
        else if(Input.GetKey(KeyCode.A))
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
    

    
    
    public enum KEY
    {
        FORWARD,
        LEFT,
        RIGHT,
    }

    public struct ButtonEvent
    {
        public KEY key;
        public bool state;
        public float time;
    }

    public List<ButtonEvent> ButtonEvents => _buttonEvents;

    private List<ButtonEvent> _buttonEvents = new List<ButtonEvent>();
    private void ReadInputs()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.FORWARD,
                state = true,
                time = Time.time
            });
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.FORWARD,
                state = false,
                time = Time.time
            });
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.RIGHT,
                state = true,
                time = Time.time
            });
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.RIGHT,
                state = false,
                time = Time.time
            });
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.LEFT,
                state = true,
                time = Time.time
            });
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _buttonEvents.Add(new ButtonEvent
            {
                key = KEY.LEFT,
                state = false,
                time = Time.time
            });
        }
    }
    
}
