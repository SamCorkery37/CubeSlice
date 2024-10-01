using UnityEngine;

public class SubdivideOnStart : MonoBehaviour
{
    public int subdivisionLevel = 2; // Controls how many times the cube will be subdivided (2 means 2x2x2 cubes)

    void Start()
    {
        Subdivide(); // Automatically subdivide when the game starts
    }

    void Subdivide()
    {
        Vector3 cubeSize = transform.localScale;
        float subCubeSize = cubeSize.x / subdivisionLevel;

        for (int x = 0; x < subdivisionLevel; x++)
        {
            for (int y = 0; y < subdivisionLevel; y++)
            {
                for (int z = 0; z < subdivisionLevel; z++)
                {
                    // Create new smaller cubes
                    GameObject subCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    subCube.transform.position = transform.position + new Vector3(x * subCubeSize, y * subCubeSize, z * subCubeSize) - cubeSize / 2f + new Vector3(subCubeSize / 2, subCubeSize / 2, subCubeSize / 2);
                    subCube.transform.localScale = new Vector3(subCubeSize, subCubeSize, subCubeSize);

                    // Optionally add a Rigidbody to each sub-cube for physics
                    // subCube.AddComponent<Rigidbody>();
                }
            }
        }

        // Destroy the original cube after subdivision
        // Destroy(gameObject);
    }
}
