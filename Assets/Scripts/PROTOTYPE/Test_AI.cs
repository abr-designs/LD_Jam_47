using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
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

    private Vector3 _targetPosition;
    
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
    
    [SerializeField]
    private GameObject rocketPrefab;


    //Unity Functions
    //====================================================================================================================//

    // Update is called once per frame
    private void Update()
    {
        if (isDead)
            return;
        
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
    [SerializeField]
    private bool isElastic = false;
    
    [SerializeField]
    private float elasticForce = 2f;
    [SerializeField]
    private float setForce = 115f;

    [SerializeField]
    private float idealDistanceFromTarget = 7f;

    [SerializeField]
    private float TEMP_currentDistance;
    [SerializeField]
    private float TEMP_Calc;
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
        
        mainTransform.position = rigidbody.position;
    }

    //Others
    //====================================================================================================================//
    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        rigidbody.position = position;
        rigidbody.rotation = rotation;
    }
    
    public void PlayBack(IReadOnlyList<RB_Move.InputEvent> inputEvents, bool isElastic)
    {
        this.isElastic = isElastic;
        //TODO Need to add a way of cleaning the last recorded position
        _inputEvents = inputEvents;
        targetIndex = 1;
        currentIndex = 0;
        _t = 0;

        replaying = true;
    }

    private void NextFrame()
    {
        var mult = 1f;
        TEMP_currentDistance = Vector3.Distance(_targetPosition, mainTransform.position);
        TEMP_Calc =TEMP_currentDistance / idealDistanceFromTarget;

        if (TEMP_Calc > 1f)
        {
            mult = Mathf.Clamp01(mult - (TEMP_Calc - 1f));
        }
        
        
        _t += Time.deltaTime * mult;
        
        
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

            switch (_inputEvents[currentIndex].State)
            {
                case STATE.FORWARD:
                    spriteRenderer.sprite = ForwardSprite;
                    break;
                case STATE.LEFT:
                    spriteRenderer.sprite = LeftSprite;
                    break;
                case STATE.RIGHT:
                    spriteRenderer.sprite = RightSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_inputEvents[currentIndex].item)
            {
                case ABILITY.NONE:
                    break;
                case ABILITY.ROCKET:
                    SpawnRocket();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _t = 0f;
            return;
        }

        _targetPosition = Vector3.Lerp(current.position, target.position, t);
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
    
    private void SpawnRocket()
    {
        var rocket = Instantiate(rocketPrefab, mainTransform.position, mainTransform.rotation);
        rocket.GetComponent<Rocket>().Init(mainTransform.forward.normalized, GetComponent<Collider>());
    }

    private void OnDrawGizmosSelected()
    {
        if (!replaying)
            return;
        
        
        
        for (int i = 1; i < _inputEvents.Count; i++)
        {
            var temp = _inputEvents[i - 1];
            
            Gizmos.color = temp.item == ABILITY.NONE ? Color.white : Color.blue;

            
            Gizmos.DrawWireSphere(temp.position, 0.3f);
            Gizmos.DrawLine(_inputEvents[i].position, temp.position);
        }
        Gizmos.DrawLine(_inputEvents[_inputEvents.Count - 1].position, _inputEvents[0].position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_targetPosition, 0.4f);
    }

    //====================================================================================================================//

    public bool invulnerable { get; set; }
    public bool isDead { get; private set; }
    public float impactForce => 150f;
    public void OnCollisionEnter(Collision other)
    {
        if (!replaying)
            return;
        var mag = other.impulse.magnitude;
        
        Debug.Log($"Impact Force: {mag}");
        if (other.impulse.magnitude >= impactForce)
            Crashed(other.contacts[0].point);
    }

    public void Crashed(Vector3 point)
    {
        isDead = true;
        spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
        rigidbody.AddExplosionForce(20, point, 5);
        rigidbody.angularDrag = 1;
        rigidbody.drag = 1f;
    }
}
