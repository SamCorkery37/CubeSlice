using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Shotgun : MonoBehaviour

{
    [SerializeField] private Animator shotGunAnimator;
    public ParticleSystem shotGunBlast;
    public Transform firePoint;
    public Camera firstPersonCam;

    public float maxRange = 100f; // Max distance for the raycast
    public float blastRadius = 5f; // Adjust this to control the size of the blast
    public float displacementFactor = 2f; // Adjust this to control how much the blast deforms the mesh

    private bool isHolding = false;



    public LayerMask slicableLayer; // Assign the Slicable layer mask in the Inspector
    [SerializeField] private GameObject BloodAttach;  // The object to attach blood effects to surfaces
    [SerializeField] private GameObject[] BloodFX;  // Array of blood effects
    private int effectIdx = 0;  // Tracks the current blood effect to use
    public ObjectPooler objectPooler; // Reference to your object pooler
    public Vector3 bloodOffset; // Define any offset for the blood effect
    public Light DirLight;

    public WeaponSwitcher weaponSwitcher;

    public CinemachineImpulseSource impulseSource;





    // Start is called before the first frame update







    // Update is called once per frame
    void Update()
    {


        if (Input.GetButtonDown("Fire1"))
        {
            isHolding = true;
            shotGunAnimator.ResetTrigger("ShotGunShoot");
            shotGunAnimator.SetTrigger("ShotGunAim");

        }

        if (Input.GetButtonUp("Fire1") && isHolding)
        {
            isHolding = false;
            shotGunAnimator.ResetTrigger("ShotGunAim");
            shotGunAnimator.SetTrigger("ShotGunShoot");

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();  // Trigger the screen shake impulse
            }
            if (Physics.Raycast(firstPersonCam.transform.position, firstPersonCam.transform.forward, out RaycastHit hit, maxRange))
            {
                Debug.DrawRay(firstPersonCam.transform.position, firstPersonCam.transform.forward * maxRange, Color.red, 3f);
                Debug.Log("Raycast Shot");


                // Ensure the object hit is on the "Slicable" layer
                if (slicableLayer == (slicableLayer | (1 << hit.collider.gameObject.layer)))
                {
                    HandleBloodPrefab(hit);
                    Debug.Log("Raycast hit a Slicable object: " + hit.collider.name);

                    MeshFilter meshFilter = hit.transform.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        // Apply the BlastCutter logic to the hit object
                        BlastCutter.Blast(hit.transform.gameObject, hit.point, blastRadius, displacementFactor);
                    }
                }
                else
                {
                    Debug.Log("Raycast hit a non-Slicable object: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("Raycast missed everything.");
            }


            if (shotGunBlast != null && !shotGunBlast.isPlaying)
            {
                PlayParticleSystem(shotGunBlast);

            }



        }

        if (Input.GetButtonDown("Fire2"))
        {
            isHolding = true;
            shotGunAnimator.SetTrigger("ShotGunAim2");
        }

        if (Input.GetButtonUp("Fire2") && isHolding)
        {
            isHolding = false;
            shotGunAnimator.ResetTrigger("ShotGunAim2");
            shotGunAnimator.SetTrigger("ShotGunSwipe");

        }

        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     isHolding = false;
        //     swordAnimator.SetTrigger("SwordInspect");

        // }

    }

    // void ShootRaycast()
    // {
    //     Debug.Log("Attempting to shoot raycast...");
    //     if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, maxRange))
    //     {
    //         Debug.DrawRay(firePoint.position, firePoint.forward * maxRange, Color.red, 3f);
    //         Debug.Log("Raycast Shot");

    //         // Ensure the object hit is on the "Slicable" layer
    //         if (slicableLayer == (slicableLayer | (1 << hit.collider.gameObject.layer)))
    //         {
    //             HandleBloodPrefab(hit);
    //             Debug.Log("Raycast hit a Slicable object: " + hit.collider.name);
    //         }
    //         else
    //         {
    //             Debug.Log("Raycast hit a non-Slicable object: " + hit.collider.name);
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Raycast missed everything.");
    //     }
    // }

    void HandleBloodPrefab(RaycastHit hit)
    {
        if (BloodFX != null)
        {
            // Spawn from pool using the single blood prefab tag
            GameObject spawnedBlood = objectPooler.SpawnFromPool("Blood", hit.point, Quaternion.identity);

            if (spawnedBlood != null)
            {
                // Set the position and rotation of the pooled blood prefab
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;

                float angle = Mathf.Atan2(hitNormal.x, hitNormal.z) * Mathf.Rad2Deg + 180;
                Quaternion bloodRotation = Quaternion.Euler(0, angle + 90, 0);

                spawnedBlood.transform.position = hitPoint + bloodOffset;
                spawnedBlood.transform.rotation = bloodRotation;

                // Randomize blood prefab scale
                float randomScale = Random.Range(3.6f, 4.9f);
                spawnedBlood.transform.localScale = Vector3.one * randomScale;

                // Find and detach the decal from the blood prefab
                Transform decal = spawnedBlood.transform.Find("Decal");
                if (decal != null)
                {
                    // Detach the decal so it stays in the scene
                    decal.parent = null;

                    // Optionally destroy the decal after a certain period, or leave it as needed
                    // Destroy(decal.gameObject, 20f);  // Uncomment if you want the decal to disappear after a while
                }

                // Now you can safely return the blood prefab to the pool without affecting the decal
                StartCoroutine(ReturnBloodPrefabToPool(spawnedBlood, 5f));  // Adjust delay as needed

                Debug.Log("Blood prefab spawned for: " + hit.collider.name);
            }
            else
            {
                Debug.LogError("Spawned blood prefab is null.");
            }
        }
        else
        {
            Debug.LogError("No blood prefab assigned in the Inspector.");
        }
    }





    private void PlayParticleSystem(ParticleSystem particle)
    {
        if (particle != null && !particle.isPlaying)
        {
            particle.Play();
            StartCoroutine(StopParticleAfterDuration(particle, particle.main.duration));
        }
    }

    private IEnumerator StopParticleAfterDuration(ParticleSystem particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (particle.isPlaying)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    IEnumerator ReturnBloodPrefabToPool(GameObject bloodPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(objectPooler.ReturnToPool(bloodPrefab, 15f));
    }
}

