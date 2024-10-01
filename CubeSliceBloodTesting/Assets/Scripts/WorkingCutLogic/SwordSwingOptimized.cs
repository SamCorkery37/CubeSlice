using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwordSwingOptimized : MonoBehaviour
{
    public LayerMask layerMask;
    public Collider hitbox;

    [SerializeField] private Animator swordAnimator;

    [Header("Sword Blood Settings")]
    [SerializeField] private Material swordMaterial;

    private Material originalSwordMaterial;

    [SerializeField] private Material[] bloodMaterials;
    private Material currentBloodMaterial;

    private ObjectPooler objectPooler;

    [Header("Blood Splatter Settings")]
    [SerializeField] private Image[] bloodSplatterImages;
    public float bloodDisplayDuration = 6f;
    public float bloodFadeSpeed = 1f;
    public float initialAlpha = 0.5f;
    private Image currentBloodSplatter;

    private Coroutine currentFadeCoroutine;

    private bool isHolding = false;

    public Light DirLight;

    [SerializeField] private GameObject BloodAttach;  // The object to attach blood effects to surfaces
    [SerializeField] private GameObject[] BloodFX;  // Array of blood effects
    private int effectIdx = 0;  // Tracks the current blood effect to use
    [SerializeField] private Vector3 bloodScale = Vector3.one;
    public Vector3 bloodOffset = Vector3.zero;

    private float cooldownTimer = 0f;

    private int cooldownFrames = 15;
    private int currentFrame = 0;

    private Renderer swordRenderer;
    private Material cachedSwordMaterial;

    private bool isFading = false;

    private bool shouldShowBloodEffects = false;
    private Collider cachedCollider;

    public WeaponSwitcher weaponSwitcher;
    void Start()
    {

        swordRenderer = GetComponent<Renderer>();
        cachedSwordMaterial = GetComponent<Renderer>().material;
        objectPooler = ObjectPooler.Instance;

        if (swordAnimator == null)
        {
            Debug.LogError("Sword Animator not assigned.");
        }

        if (swordMaterial != null)
        {
            originalSwordMaterial = swordMaterial;
        }

        foreach (var image in bloodSplatterImages)
        {
            image.gameObject.SetActive(false);
        }
    }




    void Update()
    {
        // Increment the cooldown timer by the time that has passed since the last frame




        // Only process inputs if the cooldown has passed
        if (currentFrame >= cooldownFrames)
        {
            // Handling Fire1 (Sword Raise and Thrust)
            if (Input.GetButtonDown("Fire1"))
            {
                isHolding = true;

                // Reset SwordThrust and SwordRaise2 to prevent conflicts
                swordAnimator.ResetTrigger("SwordThrust");
                swordAnimator.ResetTrigger("SwordRaise2");

                swordAnimator.SetTrigger("SwordRaise");
            }

            if (Input.GetButtonUp("Fire1") && isHolding)
            {
                isHolding = false;

                // Reset SwordRaise to prevent conflicts
                swordAnimator.ResetTrigger("SwordRaise");
                swordAnimator.SetTrigger("SwordThrust");
                cooldownTimer = 0f;  // Reset cooldown timer
            }

            // Handling Fire2 (Sword Raise2 and Swipe)
            if (Input.GetButtonDown("Fire2"))
            {
                isHolding = true;

                // Reset SwordSwipe and SwordRaise to prevent conflicts
                swordAnimator.ResetTrigger("SwordSwipe");
                swordAnimator.ResetTrigger("SwordRaise");

                swordAnimator.SetTrigger("SwordRaise2");
            }

            if (Input.GetButtonUp("Fire2") && isHolding)
            {
                isHolding = false;

                // Reset SwordRaise2 to prevent conflicts
                swordAnimator.ResetTrigger("SwordRaise2");


                swordAnimator.SetTrigger("SwordSwipe");
                cooldownTimer = 0f;  // Reset cooldown timer
            }

            // Handling Sword Inspection (key T)
            if (Input.GetKeyDown(KeyCode.T))
            {
                // Reset other sword action triggers to prevent conflicts
                swordAnimator.ResetTrigger("SwordRaise");
                swordAnimator.ResetTrigger("SwordThrust");
                swordAnimator.ResetTrigger("SwordRaise2");
                swordAnimator.ResetTrigger("SwordSwipe");


                swordAnimator.SetTrigger("SwordInspect");
            }
        }

        if (!isHolding && currentFrame >= cooldownFrames)
        {
            swordAnimator.SetTrigger("Idle");  // Return to Idle state
        }

        // Cooldown management
        if (currentFrame < cooldownFrames)
        {
            currentFrame++;
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slicable") && currentFrame >= cooldownFrames)
        {
            Debug.Log("Sword collided with: " + other.name);

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = other.transform.position - transform.position;
            float angle = Mathf.Atan2(hitNormal.x, hitNormal.z) * Mathf.Rad2Deg + 180;

            if (effectIdx == BloodFX.Length) effectIdx = 0;

            // Instantiate blood effect at the hit point
            var instance = Instantiate(BloodFX[effectIdx], hitPoint, Quaternion.Euler(0, angle + 90, 0));
            effectIdx++;

            var settings = instance.GetComponent<BFX_BloodSettings>();
            settings.LightIntensityMultiplier = DirLight.intensity;

            // Randomize blood decal scaling and positioning
            instance.transform.localScale = Vector3.one * Random.Range(4f, 8f);

            StartCoroutine(ReturnBloodPrefabToPool(instance, 5f));  // Adjust delay as needed

            Debug.Log("Blood prefab spawned from sword for: " + other.name);

            // Reset cooldown frame counter
            currentFrame = 0;
        }
    }



    void HandleBloodPrefab(Collider other)
    {
        if (BloodFX != null)
        {
            // Spawn from pool using the single blood prefab tag
            GameObject spawnedBlood = objectPooler.SpawnFromPool("Blood", other.ClosestPoint(transform.position), Quaternion.identity);

            if (spawnedBlood != null)
            {
                // Set the position and rotation of the pooled blood prefab
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = other.transform.position - transform.position;

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

                Debug.Log("Blood prefab spawned from sword for: " + other.name);
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


    IEnumerator ReturnBloodPrefabToPool(GameObject bloodPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(objectPooler.ReturnToPool(bloodPrefab, 15f));
    }

    void ApplyBloodToSword()
    {
        if (bloodMaterials.Length > 0 && currentBloodMaterial != originalSwordMaterial)
        {
            currentBloodMaterial = bloodMaterials[Random.Range(0, bloodMaterials.Length)];
            cachedSwordMaterial = currentBloodMaterial;
            cachedSwordMaterial.SetColor("_BloodColor", new Color(1f, 0f, 0f, 1f));


            Debug.Log("Blood material applied: " + cachedSwordMaterial.name);
            Debug.Log("Blood color applied: " + cachedSwordMaterial.GetColor("_BloodColor"));

            // Log material after application
            Debug.Log("Sword material after: " + swordRenderer.material.name);

            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = StartCoroutine(FadeBloodFromSword());
        }
    }

    IEnumerator FadeBloodFromSword()
    {
        if (isFading) yield break;  // Avoid overlapping coroutines

        isFading = true;
        float elapsedTime = 0f;
        float fadeDuration = 7f;

        Color bloodColor = currentBloodMaterial.GetColor("_BloodColor");

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            currentBloodMaterial.SetColor("_BloodColor", new Color(bloodColor.r, bloodColor.g, bloodColor.b, alpha));
            yield return null;
        }

        GetComponent<Renderer>().material = originalSwordMaterial;
        isFading = false;  // Allow another fade after this one is complete
    }

    void ShowBloodSplatter()
    {
        if (bloodSplatterImages != null && bloodSplatterImages.Length > 0)
        {

            // Deactivate all blood splatter images within the ScreenBlood object first
            foreach (var splatterImage in bloodSplatterImages)
            {
                splatterImage.gameObject.SetActive(false);
            }

            // Randomly select a blood splatter image from the array
            currentBloodSplatter = bloodSplatterImages[Random.Range(0, bloodSplatterImages.Length)];

            if (currentBloodSplatter != null)
            {

                currentBloodSplatter.gameObject.SetActive(true);

                // Set initial alpha to make the blood less opaque at the start
                currentBloodSplatter.color = new Color(
                    currentBloodSplatter.color.r,
                    currentBloodSplatter.color.g,
                    currentBloodSplatter.color.b,
                    initialAlpha);

                // Hide the blood splatter after a delay
                Invoke(nameof(HideBloodSplatter), bloodDisplayDuration);
            }
        }
    }



    // Method to hide blood splatter with a fade effect
    void HideBloodSplatter()
    {
        StartCoroutine(FadeOutBloodSplatter());
    }

    // Coroutine to fade out the blood splatter image
    IEnumerator FadeOutBloodSplatter()
    {
        if (currentBloodSplatter != null)
        {
            Color imageColor = currentBloodSplatter.color;
            while (imageColor.a > 0f)
            {
                imageColor.a -= Time.deltaTime * bloodFadeSpeed;
                currentBloodSplatter.color = imageColor;

                // Reduce the frequency of updates to lower the overhead
                yield return new WaitForSeconds(0.1f);  // Update every 0.1 seconds
            }
            currentBloodSplatter.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeAndDestroyLight(Light lightComponent, float duration)
    {
        float startIntensity = lightComponent.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Gradually reduce light intensity
            lightComponent.intensity = Mathf.Lerp(startIntensity, 0, elapsed / duration);
            yield return null;  // Wait for the next frame
        }

        // Destroy the light after it has faded
        Destroy(lightComponent.gameObject);
    }

    IEnumerator ShowBloodEffectsWithDelay(Collider other, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowAllBloodEffects(other);
    }

    void ShowAllBloodEffects(Collider other)
    {
        HandleBloodPrefab(other);
        ApplyBloodToSword();
        Debug.Log("Attempting to call ShowBloodSplatter()");
        ShowBloodSplatter();
    }
};