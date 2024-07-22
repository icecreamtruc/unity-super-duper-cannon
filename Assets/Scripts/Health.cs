using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 3f;
    public GameObject explosion;
    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ReduceHealth(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Instantiate(explosion, transform.position, new Quaternion());
            Destroy(gameObject);
        }
    }
}
