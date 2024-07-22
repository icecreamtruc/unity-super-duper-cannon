/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterParticles : MonoBehaviour
{
    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();    
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isPlaying)
        {
            Destroy(gameObject);
        
        }
    }
}


using System.Collections;
using System.Collections.Generic;*/

using UnityEngine;

public class DestroyAfterParticles : MonoBehaviour
{
    private ParticleSystem _ps;

    // Start is called before the first frame update
    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        _ps.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if (!_ps.isPlaying && _ps.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
