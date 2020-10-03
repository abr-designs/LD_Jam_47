using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : RacerBase, IInput
{
    public List<RecordEvent> RecordEvents => _recordEvents;

    
    //ICanCrash Properties
    //====================================================================================================================//

    public override float impactForce => 120f;

    //Properties
    //====================================================================================================================//

    [SerializeField, Header("Player")]
    private Transform cameraTransform;

    [SerializeField]
    private float forwardForce = 0.5f;
    [SerializeField]
    private float turnSpeed = 50;

    //====================================================================================================================//
    
    private List<RecordEvent> _recordEvents = new List<RecordEvent>();
    
    private bool _recording;
    private const float RecordTime = 0.3f;
    private float _t;

    
    private Vector3 _cameraForward;

    
    private bool _movingForward, _turning;
    private int _turnDirection;
    
    //Unity Functions
    //====================================================================================================================//

    protected override void Start()
    {
        base.Start();
        
        InitInput();
    }

    protected override void Update()
    {
        followTransform.position = rigidbody.position;

        if (isDead)
            return;
        
        TryRecordData();
        
        if (rigidbody is null)
            return;
        
        _cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward.normalized, Vector3.up);

        if(_turning)
            followTransform.rotation *= Quaternion.Euler(Vector3.up * (turnSpeed * Time.deltaTime * _turnDirection));
        
    }

    private void LateUpdate()
    {
        SetSprite(CurrentSprite);
    }

    private void FixedUpdate()
    {
        if (_movingForward)
        {
            var forcePosition = followTransform.position + followTransform.up.normalized * 0.3f;
            rigidbody.AddForceAtPosition(_cameraForward * forwardForce, forcePosition, ForceMode.Impulse);
        }

        if (_turning)
        {
            rigidbody.velocity = followTransform.forward.normalized * rigidbody.velocity.magnitude;
        }
    }

    private void OnDestroy()
    {
        DeInitInput();
    }


    //ICanCrash Functions
    //====================================================================================================================//

    public override void Crashed(Vector3 point)
    {
        isDead = true;
        spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
        rigidbody.AddExplosionForce(20, point, 5);
        spriteRenderer.transform.SetParent(rigidbody.transform);
        rigidbody.angularDrag = 1;
        rigidbody.drag = 1f;
    }

    //IInput Functions
    //====================================================================================================================//

    #region Inputs

    public void InitInput()
    {
        LInput.Input.Default.Movement.Enable();
        LInput.Input.Default.Movement.performed += ProcessMovement;
        
        LInput.Input.Default.Turning.Enable();
        LInput.Input.Default.Turning.performed += ProcessTurning;
    }



    public void DeInitInput()
    {
        LInput.Input.Default.Movement.Disable();
        LInput.Input.Default.Movement.performed -= ProcessMovement;
        
        LInput.Input.Default.Turning.Disable();
        LInput.Input.Default.Turning.performed -= ProcessTurning;
    }
    
    //Inputs
    //====================================================================================================================//
    private void ProcessMovement(InputAction.CallbackContext ctx)
    {
        var inputValue = ctx.ReadValue<float>();
        
        _movingForward = Mathf.RoundToInt(inputValue) > 0f;
    }

    private void ProcessTurning(InputAction.CallbackContext ctx)
    {
        var inputValue = ctx.ReadValue<float>();

        _turnDirection = Mathf.RoundToInt(inputValue);
        
        _turning = _turnDirection != 0;

        
        
        

        if (_turnDirection > 0)
        {
            CurrentSprite = SPRITE.RIGHT;
        }
        else if (_turnDirection < 0)
        {
            CurrentSprite = SPRITE.LEFT;
        }
        else
        {
            CurrentSprite = SPRITE.FORWARD;
        } 
    }

    #endregion //Inputs

    //Player Functions
    //====================================================================================================================//

    #region Recording

    public void TriggerNewLap()
    {
        _recordEvents = new List<RecordEvent>();
        _recording = true;
    }
    
    private void TryRecordData()
    {
        if (!_recording)
            return;
        
        if (_t < RecordTime)
        {
            _t += Time.deltaTime;
            return;
        }

        _t = 0f;
        
        _recordEvents.Add(new RecordEvent
        {
            Position = followTransform.position,
            Direction = followTransform.forward.normalized,
            Sprite = CurrentSprite,
            Time = Time.time
        });
    }

    private void RecordUsePickup(ABILITY ability)
    {
        if (!_recording)
            return;
        
        _recordEvents.Add(new RecordEvent
        {
            Position = followTransform.position,
            Direction = followTransform.forward.normalized,
            Sprite = CurrentSprite,
            Item = ability,
            Time = Time.time
        });
    }

    #endregion //Recording



}
