using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(SimpleAnimator))]
public abstract class RacerBase : MonoBehaviour, ICanCrash
{
    protected static Manager _manager;
    //ICanCrash Properties
    //====================================================================================================================//

    public bool invulnerable { get; set; }
    
    public bool isDead { get; protected set; }
    public abstract float impactForce { get; }


    //====================================================================================================================//

    //[SerializeField, Header("Sprites")]
    //protected Sprite[] sprites;

    [SerializeField, Header("Sprites")] 
    protected SpriteRenderer spriteRenderer;
    protected STATE CurrentState;

    //====================================================================================================================//
    
    [SerializeField, Header("Ground Check")]
    private LayerMask groundCheckMask;
    protected bool Grounded;

    //====================================================================================================================//
    

    [SerializeField, Header("Transforms")]
    protected Transform followTransform;
    [SerializeField]
    protected Transform parentTransform;
    
    protected new Rigidbody rigidbody;
    protected new Collider collider;
    private SimpleAnimator _animator;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!_manager)
            _manager = FindObjectOfType<Manager>();

        collider = GetComponent<Collider>();
        rigidbody = GetComponent <Rigidbody>();
        _animator = GetComponent<SimpleAnimator>();
        
        SetState(STATE.FORWARD);
    }

    protected abstract void Update();


    public void OnCollisionEnter(Collision other)
    {
        if (invulnerable)
            return;
        
        if (other.impulse.magnitude < impactForce)
            return;
        
        Crashed(other);

    }

    //ICanCrash Functions
    //====================================================================================================================//

    public abstract void Crashed(Collision collision);

    //RacerBase Functions
    //====================================================================================================================//

    protected void SetState(STATE state)
    {
        if (!_animator)
            return;

        _animator.SetState(state);
        _animator.SetSpeed(rigidbody.velocity.magnitude / 25f);
    }


    //====================================================================================================================//

    protected void ActivateAbility(ABILITY ability)
    {
        if (ability == ABILITY.NONE)
            return;

        switch (ability)
        {
            case ABILITY.ROCKET:
                var rocket = FactoryManager.Instance.CreateRocket();
                rocket.transform.position = followTransform.position;
                rocket.Init(followTransform.forward, collider);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected void CreateCrashEffects(Collision collision)
    {
        var position = collision.contacts[0].point;
        var normal = collision.contacts[0].normal;

        var fire = FactoryManager.Instance.CreateFireEffect().transform;
        fire.SetParent(followTransform);
        fire.localPosition = Vector3.down;

        var explosion = FactoryManager.Instance.CreateExplosionEffect().transform;
        explosion.position = position;
        explosion.forward = normal;
    }
    
    protected void CheckForGround()
    {
        Grounded = Physics.Raycast(followTransform.position, Vector3.down, 0.6f, groundCheckMask.value);
    }
}
