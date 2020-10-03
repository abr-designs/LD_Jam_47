﻿using System;
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

    private new Collider collider;
    

    [SerializeField]
    private float forwardForce;
    [SerializeField]
    private float turnForce;

    private bool recording;
    private float recordTime = 0.3f;
    private float _t;

    [SerializeField]
    private GameObject rocketPrefab;

    private SPRITE _sprite;
    
    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        TryRecordData();
        
        if (rigidbody is null)
            return;
        
        if(Input.GetKeyDown(KeyCode.Space))
            SpawnRocket();

        var forward = Vector3.ProjectOnPlane(cameraTranform.forward.normalized, Vector3.up);
        
        if(Input.GetKey(KeyCode.W))
            rigidbody.AddForce(forward * forwardForce, ForceMode.Impulse);

        if (Input.GetKey(KeyCode.D))
        {
            mainTransform.rotation *= Quaternion.Euler(Vector3.up * (turnForce * Time.deltaTime));
            rigidbody.velocity = mainTransform.forward.normalized * rigidbody.velocity.magnitude;
            
            spriteRenderer.sprite = RightSprite;
            _sprite = SPRITE.RIGHT;
        }
        else if(Input.GetKey(KeyCode.A))
        {
            mainTransform.rotation *= Quaternion.Euler(Vector3.up * (-turnForce * Time.deltaTime));
            rigidbody.velocity = mainTransform.forward.normalized * rigidbody.velocity.magnitude;
            
            spriteRenderer.sprite = LeftSprite;
            _sprite = SPRITE.LEFT;
        }
        else
        {
            spriteRenderer.sprite = ForwardSprite;
            _sprite = SPRITE.FORWARD;
        }
        
        Debug.DrawLine( mainTransform.position,  mainTransform.position + rigidbody.velocity.normalized, Color.green);

        

    }

    private void FixedUpdate()
    {
        mainTransform.position = rigidbody.position;
    }

    //Others
    //====================================================================================================================//

    private void SpawnRocket()
    {
        var rocket = Instantiate(rocketPrefab, mainTransform.position, mainTransform.rotation);
        rocket.GetComponent<Rocket>().Init(mainTransform.forward.normalized, collider);
    }

    public void TriggerNewLap()
    {
        _inputEvents = new List<InputEvent>();
        recording = true;
    }
    
    
    public enum SPRITE
    {
        FORWARD,
        LEFT,
        RIGHT,
    }
    /*public enum KEY
    {
        FORWARD,
        LEFT,
        RIGHT,
    }*/

    /*public struct ButtonEvent
    {
        public KEY key;
        public bool state;
        public float time;
    }*/

    public struct InputEvent
    {
        public Vector3 position;
        public Vector3 direction;
        public SPRITE sprite;
        public float time;
    }

    public List<InputEvent> InputEvents => _inputEvents;

    private List<InputEvent> _inputEvents = new List<InputEvent>();
    private void TryRecordData()
    {
        if (!recording)
            return;
        
        if (_t < recordTime)
        {
            _t += Time.deltaTime;
            return;
        }

        _t = 0f;
        
        _inputEvents.Add(new InputEvent
        {
            position = mainTransform.position,
            direction = mainTransform.forward.normalized,
            sprite = _sprite,
            time = Time.time
        });
    }
    
}
