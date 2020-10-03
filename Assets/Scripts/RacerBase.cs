using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class RacerBase : MonoBehaviour, ICanCrash
{
    protected static Manager _manager;
    //ICanCrash Properties
    //====================================================================================================================//

    public bool invulnerable { get; set; }
    
    public bool isDead { get; protected set; }
    public abstract float impactForce { get; }


    //====================================================================================================================//

    [SerializeField, Header("Sprites")]
    protected Sprite[] sprites;

    [SerializeField] 
    protected SpriteRenderer spriteRenderer;
    protected SPRITE CurrentSprite;

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

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!_manager)
            _manager = FindObjectOfType<Manager>();

        collider = GetComponent<Collider>();
        rigidbody = GetComponent <Rigidbody>();
    }

    protected abstract void Update();


    public void OnCollisionEnter(Collision other)
    {
        if (invulnerable)
            return;
        
        if (other.impulse.magnitude < impactForce)
            return;
        
        Crashed(other.contacts[0].point);
        CreateCrashEffects(other);
    }

    //ICanCrash Functions
    //====================================================================================================================//

    public abstract void Crashed(Vector3 point);

    //RacerBase Functions
    //====================================================================================================================//

    protected void SetSprite(SPRITE sprite)
    {
        int index;
        spriteRenderer.flipX = false;
        switch (sprite)
        {
            case SPRITE.FORWARD:
                index = 0;
                
                break;
            case SPRITE.LEFT:
                index = 1;
                spriteRenderer.flipX = true;
                break;
            case SPRITE.RIGHT:
                index = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sprite), sprite, null);
        }

        spriteRenderer.sprite = sprites[index];
    }


    //====================================================================================================================//

    private void CreateCrashEffects(Collision collision)
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
