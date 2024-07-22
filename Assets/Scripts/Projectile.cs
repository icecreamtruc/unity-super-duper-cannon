using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float time;
    public float radius;
    public float damage;

    public string shooterTag;
    public GameObject explosion;

    private Rigidbody _rb;
    private float _t;

    // Start is called before the first frame update
    void Start()
    {
        _t = time;
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = transform.forward * speed;        
    }

    // Update is called once per frame
    void Update()
    {
        _t -= Time.deltaTime;
        if (_t < 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            if (health != null)
            {
                health.ReduceHealth(damage);
            }
        }
        
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Muzzle") && !other.CompareTag(shooterTag))
        {
            Explode();
        }
    }
}
