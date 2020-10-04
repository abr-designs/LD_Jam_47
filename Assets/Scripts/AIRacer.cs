using System;
using System.Collections.Generic;
using UnityEngine;

public class AIRacer : RacerBase
{
    //ICanCrash Properties
    //====================================================================================================================//

    public override float impactForce => 150;

    //Properties
    //====================================================================================================================//
    private Vector3 _targetPosition;
    
    
    private IReadOnlyList<RecordEvent> _recordEvents;

    private bool _replaying;
    private int _targetIndex;
    private int _currentIndex;
    private float _t;
    private float _lastDeltaTime;
    
    [SerializeField, Header("AI")]
    private bool isElastic;
    
    [SerializeField]
    private float elasticForce = 2f;
    [SerializeField]
    private float setForce = 115f;

    [SerializeField]
    private float idealDistanceFromTarget = 7f;

    [SerializeField] 
    private float cleanupTimer = 30f;

    //Unity Functions
    //====================================================================================================================//

    protected override void Update()
    {
        followTransform.position = rigidbody.position;
        
        if (isDead)
            return;
        
        if (!_replaying)
            return;

        NextFrame();
    }

    private void LateUpdate()
    {
        var dir = Vector3.ProjectOnPlane(
            (spriteRenderer.transform.position - Manager.CameraTransform.position).normalized, Vector3.up);
        
        spriteRenderer.transform.forward = dir;

        if (!isDead)
            return;

        cleanupTimer -= Time.deltaTime;
        
        if(cleanupTimer <= 0f)
            Destroy(parentTransform.gameObject);
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;
        
        var vector = _targetPosition - rigidbody.position;
        var direction = vector.normalized;
        var magnitude = vector.magnitude;

        if(isElastic)
            rigidbody.velocity += direction * (magnitude * elasticForce * Time.fixedDeltaTime);
        else
            rigidbody.velocity = direction * (magnitude * setForce * Time.fixedDeltaTime);
        
        
    }

    //ICanCrash Functions
    //====================================================================================================================//
    
    public override void Crashed(Collision collision)
    {
        if (isDead)
            return;
        
        var point = collision.contacts[0].point;
        isDead = true;
        spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
        rigidbody.AddExplosionForce(20, point, 5);
        rigidbody.angularDrag = 1;
        rigidbody.drag = 1f;
        
        CreateCrashAudioEffect();
        CreateCrashEffects(collision);
        SetState(STATE.DEAD);
    }

    //AIRacer Functions
    //====================================================================================================================//

    #region Playback

    public void PlayBack(IReadOnlyList<RecordEvent> inputEvents, float elasticicty)
    {
        isElastic = elasticicty > 0f;
        elasticForce = elasticicty;
        
        //TODO Need to add a way of cleaning the last recorded position
        _recordEvents = inputEvents;
        _targetIndex = 1;
        _currentIndex = 0;
        _t = 0;

        _replaying = true;
    }

    private void NextFrame()
    {
        var mult = 1f;
        var distance =Vector3.Distance(_targetPosition, followTransform.position) / idealDistanceFromTarget;

        if (distance > 1f)
        {
            mult = Mathf.Clamp01(mult - (distance - 1f));
        }
        
        _t += Time.deltaTime * mult;
        
        var target = _recordEvents[_targetIndex];
        var current = _recordEvents[_currentIndex];

        var time = target.Time - current.Time;

        if (time < 0) 
            time = _lastDeltaTime;

        var t = _t / time;

        if (t >= 1f)
        {
            _lastDeltaTime = time;
            _currentIndex++;
            _targetIndex++;

            if (_targetIndex >= _recordEvents.Count)
            {
                _targetIndex = 0;
            }

            if (_currentIndex >= _recordEvents.Count)
            {
                _currentIndex = 0;
            }

            SetState(_recordEvents[_currentIndex].State);

            var ability = _recordEvents[_currentIndex].Ability;
            if (ability != ABILITY.NONE)
                ActivateAbility(ability);
            
            _t = 0f;
            return;
        }

        _targetPosition = Vector3.Lerp(current.Position, target.Position, t);
        followTransform.forward = Vector3.Lerp(current.Direction, target.Direction, t);
    }

    #endregion //Playback
    
    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();
        
        rigidbody.position = position;
        rigidbody.rotation = rotation;
    }
    

    //Unity Editor
    //====================================================================================================================//
    
    #region Unity Editor

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!_replaying)
            return;
        
        for (var i = 1; i < _recordEvents.Count; i++)
        {
            var temp = _recordEvents[i - 1];
            
            Gizmos.color = temp.Ability == ABILITY.NONE ? Color.white : Color.blue;

            
            Gizmos.DrawWireSphere(temp.Position, 0.3f);
            Gizmos.DrawLine(_recordEvents[i].Position, temp.Position);
        }
        Gizmos.DrawLine(_recordEvents[_recordEvents.Count - 1].Position, _recordEvents[0].Position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_targetPosition, 0.4f);
    }
#endif

    #endregion //Unity Editor
}
