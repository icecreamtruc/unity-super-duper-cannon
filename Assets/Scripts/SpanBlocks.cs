using UnityEngine;

public class SpanBlocks : MonoBehaviour
{
    public int amount;
    public GameObject block;
    public Collider ground;
    public float minSize;
    public float maxSize;
  

    // Start is called before the first frame update
    private void Start()
    {
        Vector3 minPoint = ground.bounds.min;
        Vector3 maxPoint = ground.bounds.max;

        for (int i = 0; i < amount; i++)
        {
            float randomx = Random.Range(minPoint.x, maxPoint.x);
            float randomz = Random.Range(minPoint.z, maxPoint.z);
            float randomSize = Random.Range(minSize, maxSize);

            Vector3 position = new Vector3(randomx, maxPoint.y + randomSize / 2, randomz);
            GameObject spawnedBlock = Instantiate(block, position, Quaternion.identity, transform);
            spawnedBlock.transform.localScale *= randomSize;
            spawnedBlock.GetComponent<Health>().maxHealth = randomSize;
        }
    }

    
}
