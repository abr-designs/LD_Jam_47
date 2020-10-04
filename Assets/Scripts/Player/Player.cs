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

    [SerializeField] 
    private Transform trailParent;

    
    private ABILITY _activeAbility;

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
        Manager.CameraTransform = cameraTransform;
    }

    protected override void Update()
    {
        followTransform.position = rigidbody.position;

        if (Grounded)
        {
            trailParent.position = followTransform.position;
            trailParent.rotation = followTransform.rotation;
        }

        if (isDead)
            return;
        
        TryRecordData();
        
        if (rigidbody is null)
            return;
        
        _cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward.normalized, Vector3.up);

        if(_turning)
            followTransform.rotation *= Quaternion.Euler(Vector3.up * (turnSpeed * Time.deltaTime * _turnDirection));

        CheckForGround();
    }

    private void LateUpdate()
    {
        if (isDead)
            return;
        
        SetState(CurrentState);
    }

    private void FixedUpdate()
    {
        if (_movingForward)
        {
            var forcePosition = followTransform.position + followTransform.up.normalized * 0.3f;
            rigidbody.AddForceAtPosition(_cameraForward * forwardForce, forcePosition, ForceMode.Impulse);
        }

        if (_turning && Grounded)
        {
            var velocity = followTransform.forward.normalized * rigidbody.velocity.magnitude;
            rigidbody.velocity = velocity;
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
        
        rigidbody.AddExplosionForce(20, point, 5);
        rigidbody.angularDrag = 3;
        rigidbody.drag = 3;
        
        
        spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f);
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
        
        LInput.Input.Default.Use_Ability.Enable();
        LInput.Input.Default.Use_Ability.performed += ProccessAbility;
    }



    public void DeInitInput()
    {
        LInput.Input.Default.Movement.Disable();
        LInput.Input.Default.Movement.performed -= ProcessMovement;
        
        LInput.Input.Default.Turning.Disable();
        LInput.Input.Default.Turning.performed -= ProcessTurning;
        
        LInput.Input.Default.Use_Ability.Disable();
        LInput.Input.Default.Use_Ability.performed -= ProccessAbility;
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
            CurrentState = STATE.RIGHT;
        }
        else if (_turnDirection < 0)
        {
            CurrentState = STATE.LEFT;
        }
        else
        {
            CurrentState = STATE.FORWARD;
        } 
    }

    private void ProccessAbility(InputAction.CallbackContext ctx)
    {
        if (_activeAbility == ABILITY.NONE)
            return;

        switch (_activeAbility)
        {
            case ABILITY.ROCKET:
                var rocket = FactoryManager.Instance.CreateRocket();
                rocket.Init(followTransform.forward.normalized, collider);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        RecordUseAbility(_activeAbility);
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
            State = CurrentState,
            Time = Time.time
        });
    }

    private void RecordUseAbility(ABILITY ability)
    {
        if (!_recording)
            return;
        
        _recordEvents.Add(new RecordEvent
        {
            Position = followTransform.position,
            Direction = followTransform.forward.normalized,
            State = CurrentState,
            Ability = ability,
            Time = Time.time
        });
    }

    #endregion //Recording

    


}
