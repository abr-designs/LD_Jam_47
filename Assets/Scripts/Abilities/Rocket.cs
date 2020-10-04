using UnityEngine;

public class Rocket : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private bool ready;

    [SerializeField]
    private float force;

    private Vector3 _direction;

    [SerializeField]
    private Transform modelTransform;


    //Unity Functions
    //====================================================================================================================//
    
    /*private void FixedUpdate()
    {
        if(!ready)
            return;

        rigidbody.AddForce(_direction * force);
    }*/

    private void LateUpdate()
    {
        modelTransform.localRotation *= Quaternion.Euler(Vector3.up * (180f * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!ready)
            return;

        if (collision.gameObject.GetComponent<ICanCrash>() is ICanCrash iCanCrash && !iCanCrash.isDead)
        {
            iCanCrash.Crashed(collision);
        }
        else if (collision.gameObject.GetComponentInParent<ICanCrash>() is ICanCrash iCanCrash2 && !iCanCrash2.isDead)
        {
            iCanCrash2.Crashed(collision);
        }
        else
        {
            var temp = FactoryManager.Instance.CreateExplosionEffect().transform;
            temp.position = collision.contacts[0].point;
            temp.forward = collision.contacts[0].normal;
        }

        PlayCollisionSound(collision.contacts[0].point);
        
        Destroy(gameObject);
    }

    //====================================================================================================================//
    
    public void Init(Vector3 direction, Collider ignoreCollider)
    {
        modelTransform.up = direction;
        
        
        var collider = GetComponent<Collider>();
        Physics.IgnoreCollision(ignoreCollider, collider);
        collider.enabled = true;

        rigidbody = gameObject.GetComponent<Rigidbody>();
        _direction = direction;
        ready = true;
        
        rigidbody.AddForce(_direction * force, ForceMode.VelocityChange);
    }

    private void PlayCollisionSound(Vector3 worldPosition)
    {
        var sound = FactoryManager.Instance.CreateRocketExplosionAudio().transform;
        sound.position = worldPosition;
        Destroy(sound.gameObject, 2f);
    }
}
