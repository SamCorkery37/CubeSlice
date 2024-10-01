using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SlicedObjectPooler : MonoBehaviour
{
    public static SlicedObjectPooler Instance; // Singleton instance

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple SlicedObjectPoolers in the scene!");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);  // Disable initially
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.Log("No objects left in pool, creating new one.");

            // Dynamically instantiate a new object if the pool is empty
            GameObject newObj = Instantiate(pools.Find(p => p.tag == tag).prefab);
            newObj.SetActive(false);  // Initially deactivate the new object
            poolDictionary[tag].Enqueue(newObj);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);  // Re-enqueue the object immediately for future reuse

        return objectToSpawn;
    }

    public IEnumerator ReturnToPool(GameObject slicedObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        slicedObject.SetActive(false);
    }
}

