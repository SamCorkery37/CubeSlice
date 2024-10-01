using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwordSwingHoldRelease : MonoBehaviour
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
    public float bloodDisplayDuration = 2f;
    public float bloodFadeSpeed = 1f;
    public float initialAlpha = 0.5f;
    private Image currentBloodSplatter;

    private Coroutine currentFadeCoroutine;

    private bool isHolding = false;

    [SerializeField] private GameObject bloodPrefab;  // Now we only need one blood prefab
    [SerializeField] private Vector3 bloodScale = Vector3.one;
    public Vector3 bloodOffset = Vector3.zero;

    private float cooldownTime = 0.0f;
    private int cooldownFrames = 60;
    private int currentFrame = 0;

    void Start()
    {
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
        // Only process inputs if the cooldown has passed
        if (currentFrame >= cooldownFrames)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isHolding = true;
                swordAnimator.SetTrigger("SwordRaise");
            }

            if (Input.GetButtonUp("Fire1") && isHolding)
            {
                isHolding = false;
                swordAnimator.SetTrigger("SwordThrust");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                isHolding = true;
                swordAnimator.SetTrigger("SwordRaise2");
            }

            if (Input.GetButtonUp("Fire2") && isHolding)
            {
                isHolding = false;
                swordAnimator.SetTrigger("SwordSwipe");
            }
        }

        // Update cooldown frames
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

            // Perform the cut and handle blood
            Cutter.Cut(other.gameObject, other.ClosestPoint(transform.position), Vector3.up);
            HandleBloodPrefab(other);  // Updated blood prefab handling
            ApplyBloodToSword();
            ShowBloodSplatter();

            // Reset cooldown frame counter
            currentFrame = 0;
        }
    }

    void HandleBloodPrefab(Collider other)
    {
        if (bloodPrefab != null)
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

                // Transform decal = spawnedBlood.transform.Find("Decal");
                // if (decal != null)
                // {
                //     decal.parent = null;
                //     Destroy(decal.gameObject, 20f);
                // }

                GameObject redLight = new GameObject("RedLight");
                Light lightComponent = redLight.AddComponent<Light>();
                lightComponent.color = Color.red;
                lightComponent.intensity = 0.8f;
                lightComponent.range = 50f;
                redLight.transform.position = spawnedBlood.transform.position;

                StartCoroutine(FadeAndDestroyLight(lightComponent, 2f));

                Debug.Log("Blood prefab spawned from sword for: " + other.name);
            }
            else
            {
                Debug.LogError("Blood prefab is null.");
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
        StartCoroutine(objectPooler.ReturnToPool(bloodPrefab, 5f));
    }

    void ApplyBloodToSword()
    {
        if (bloodMaterials.Length > 0)
        {
            currentBloodMaterial = bloodMaterials[Random.Range(0, bloodMaterials.Length)];
            GetComponent<Renderer>().material = currentBloodMaterial;
            currentBloodMaterial.SetColor("_BloodColor", new Color(1f, 0f, 0f, 1f));

            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = StartCoroutine(FadeBloodFromSword());
        }
    }

    IEnumerator FadeBloodFromSword()
    {
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
                yield return null;
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
};

