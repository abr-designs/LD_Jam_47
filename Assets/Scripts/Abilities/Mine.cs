using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Mine : MonoBehaviour
{
    private static Manager _manager;
    
    private bool ready;

    private string _from;
    
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if(!ready)
            return;

        if (collision.gameObject.GetComponent<ICanCrash>() is ICanCrash iCanCrash && !iCanCrash.isDead)
        {
            iCanCrash.Crashed(collision);

            if (_from.Equals("Player")) _manager.AddKill();
        }
        else if (collision.gameObject.GetComponentInParent<ICanCrash>() is ICanCrash iCanCrash2 && !iCanCrash2.isDead)
        {
            iCanCrash2.Crashed(collision);
            if (_from.Equals("Player")) _manager.AddKill();
        }
        else
        {
            var temp = FactoryManager.Instance.CreateExplosionEffect().transform;
            temp.position = collision.contacts[0].point;
            temp.forward = collision.contacts[0].normal;
        }

        
        Destroy(gameObject);
    }

    //====================================================================================================================//
    
    public void Init(Collider ignoreCollider, string from)
    {
        if (!_manager)
            _manager = FindObjectOfType<Manager>();
        
        _from = from;
        
        
        
        var collider = GetComponent<Collider>();
        Physics.IgnoreCollision(ignoreCollider, collider);
        collider.enabled = true;

        ready = true;
    }
}
