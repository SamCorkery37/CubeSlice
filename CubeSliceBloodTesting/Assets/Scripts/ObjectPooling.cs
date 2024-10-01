using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;              // For example, "Blood"
        public GameObject prefab;       // Single blood prefab to pool
        public int size;                // Pool size
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Initialize each pool
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);  // Disable it initially
                objectPool.Enqueue(obj);  // Add to the pool
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent multiple instances
        }
    }

    // Function to retrieve an object from the pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();  // Get an object from the pool

        if (objectToSpawn == null)
        {
            Debug.LogError("Object pool is empty or has been mismanaged.");
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Start coroutine to return the object to the pool after a delay
        StartCoroutine(ReturnToPool(objectToSpawn, 2f));  // Adjust the delay as needed

        poolDictionary[tag].Enqueue(objectToSpawn);  // Re-enqueue the object for reuse

        return objectToSpawn;
    }

    // Coroutine to return the object to the pool
    public IEnumerator ReturnToPool(GameObject bloodPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        bloodPrefab.SetActive(false);  // Deactivate the blood prefab
    }
}
