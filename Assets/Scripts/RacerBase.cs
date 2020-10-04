using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(SimpleAnimator))]
public abstract class RacerBase : MonoBehaviour, ICanCrash
{
    protected static Manager _manager;
    //ICanCrash Properties
    //====================================================================================================================//

    public bool invulnerable
    {
        get => _invulnerable;
        set => _invulnerable = value;
    }
    [SerializeField]
    private bool _invulnerable;
    
    public bool isDead { get; protected set; }
    public abstract float impactForce { get; }

    protected float currentSpeed;


    //====================================================================================================================//

    public bool useAudio
    {
        get => _useAudio;
        set
        {
            if (!value && engineAudioSource)
                engineAudioSource.Stop();

            _useAudio = value;

        }
    }

    private bool _useAudio = true;

    [SerializeField]
    private AudioSource engineAudioSource;
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
        
        var magnitude = other.impulse.magnitude;
        if (magnitude < impactForce)
        {
            if(magnitude > 10)
                CreateCollisionAudioEffect(magnitude / impactForce);
            return;
        }
        
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

        currentSpeed = rigidbody.velocity.magnitude / 25f;
        _animator.SetState(state);
        _animator.SetSpeed(currentSpeed);

        if(!isDead)
            SetEngineSound(currentSpeed);
    }

    //Audio
    //====================================================================================================================//
    
    private void SetEngineSound(float value, bool disable = false)
    {
        if (!engineAudioSource || !useAudio)
            return;

        if (disable)
        {
            engineAudioSource.enabled = false;
            return;
        }
        
        engineAudioSource.pitch = Mathf.Lerp(0.5f, 3, value);
    }
    
    protected void CreateCrashAudioEffect()
    {
        SetEngineSound(0f, true);
        
        var soundTransform = FactoryManager.Instance.CreateCarExplosionAudio().transform;
        soundTransform.position = followTransform.position;
        
        Destroy(soundTransform.gameObject, 2f);
    }
    
    protected void CreateCollisionAudioEffect(float volume)
    {
        var soundTransform = FactoryManager.Instance.CreateCarCollisionAudio().transform;
        soundTransform.GetComponent<AudioSource>().volume = Mathf.Lerp(0.05f, 0.2f, volume);
        soundTransform.position = followTransform.position;
        
        Destroy(soundTransform.gameObject, 2f);
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
                rocket.Init(followTransform.forward, collider, gameObject.tag);
                break;
            case ABILITY.MINE:
                var mine = FactoryManager.Instance.CreateMine();
                mine.transform.position = followTransform.position;
                mine.Init(collider, gameObject.tag);
                break;
            case ABILITY.SMOKE:
                var smoke = FactoryManager.Instance.CreateSmokeScreen().transform;
                var pos = followTransform.position;
                pos.y = 0f;
                smoke.transform.position = pos;
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
