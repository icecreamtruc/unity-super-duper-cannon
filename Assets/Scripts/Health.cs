using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 3f;
    public float damageFlashTime = 1f;
    public Color damageColor = Color.red;
    public GameObject explosion;
    
    private float currentHealth;
    private Color originalColor;
    private Color originalEmissionColor;
    private float t;

    private MeshRenderer[] _meshRenderers;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalColor = _meshRenderers[0].material.color;
        originalEmissionColor = _meshRenderers[0].material.GetColor("_emissionColor");

    }

    public void ReduceHealth(float damage)
    {
        StartCoroutine(DamageFlash());
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Instantiate(explosion, transform.position, new Quaternion());
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlash()
    {
        t = damageFlashTime;
        while (t > 0)
        {
            t -= Time.deltaTime;
            
            Color newColor = Color.Lerp(originalColor, damageColor, t / damageFlashTime);
            Color newEmissionColor = Color.Lerp(originalEmissionColor, damageColor, t / damageFlashTime);
            foreach (MeshRenderer r in _meshRenderers)
            {
                r.material.color = damageColor;
                r.material.SetColor("_EmissionColor", newEmissionColor);
            }
            yield return null;
        }
        foreach (MeshRenderer r in _meshRenderers)
        {
            r.material.color = originalColor;    
            r.material.SetColor("_EmissionColor", originalEmissionColor);
        }
    }   
}
