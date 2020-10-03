using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class RacerBase : MonoBehaviour, ICanCrash
{
    protected static Manager _manager;
    //ICanCrash Properties
    //====================================================================================================================//
    
    public bool isDead { get; protected set; }
    public abstract float impactForce { get; }


    //====================================================================================================================//

    [SerializeField, Header("Sprites")]
    protected Sprite[] sprites;

    [SerializeField] 
    protected SpriteRenderer spriteRenderer;
    protected SPRITE CurrentSprite;

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
        if (other.impulse.magnitude < impactForce)
            return;
        
        Crashed(other.contacts[0].point);
    }

    //ICanCrash Functions
    //====================================================================================================================//

    public abstract void Crashed(Vector3 point);

    //RacerBase Functions
    //====================================================================================================================//

    protected void SetSprite(SPRITE sprite)
    {
        int index;
        switch (sprite)
        {
            case SPRITE.FORWARD:
                index = 0;
                break;
            case SPRITE.LEFT:
                index = 1;
                break;
            case SPRITE.RIGHT:
                index = 2;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sprite), sprite, null);
        }

        spriteRenderer.sprite = sprites[index];
    }
}
