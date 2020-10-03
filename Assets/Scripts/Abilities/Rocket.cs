using UnityEngine;

public class Rocket : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private bool ready;

    [SerializeField]
    private float force;

    private Vector3 _direction;

    //Unity Functions
    //====================================================================================================================//
    
    /*private void FixedUpdate()
    {
        if(!ready)
            return;

        rigidbody.AddForce(_direction * force);
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if(!ready)
            return;

        if (other.gameObject.GetComponent<ICanCrash>() is ICanCrash iCanCrash)
        {
            iCanCrash.Crashed(other.contacts[0].point);
        }
        else if (other.gameObject.GetComponentInParent<ICanCrash>() is ICanCrash iCanCrash2)
        {
            iCanCrash2.Crashed(other.contacts[0].point);
        }

        Destroy(gameObject);
    }

    //====================================================================================================================//
    
    public void Init(Vector3 direction, Collider ignoreCollider)
    {
        var collider = GetComponent<Collider>();
        Physics.IgnoreCollision(ignoreCollider, collider);
        collider.enabled = true;

        rigidbody = gameObject.GetComponent<Rigidbody>();
        _direction = direction;
        ready = true;
        
        rigidbody.AddForce(_direction * force, ForceMode.VelocityChange);
    }
}
