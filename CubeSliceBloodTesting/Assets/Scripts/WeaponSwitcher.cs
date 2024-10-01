using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons;  // Array to store your weapons
    public Animator[] animators;  // Array to store animators for each weapon
    private int currentWeaponIndex = 0;

    void Start()
    {
        // Make sure only the current weapon is active at the start
        ActivateWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // Switch weapons when pressing the key (can be changed to any other input)
        if (Input.GetKeyDown(KeyCode.Q) && !IsAnimationPlaying())  // Prevent switching if animation is playing
        {
            SwitchWeapon();
        }
    }

    void SwitchWeapon()
    {
        // Deactivate current weapon
        weapons[currentWeaponIndex].SetActive(false);

        // Increment weapon index
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;

        // Activate the next weapon and play the pull-out animation
        ActivateWeapon(currentWeaponIndex);
    }

    void ActivateWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].activeSelf && i != index)
            {
                // Reset animation triggers for the weapon being deactivated
                animators[i].ResetTrigger("PullOut");
                animators[i].ResetTrigger("Fire1");
                animators[i].ResetTrigger("Fire2");
                animators[i].ResetTrigger("SwordThrust");
                animators[i].ResetTrigger("SwordSwipe");
                animators[i].ResetTrigger("Idle");  // Reset idle trigger if needed
            }

            // Activate the selected weapon
            weapons[i].SetActive(i == index);
        }

        // Set PullOut trigger to play the pull-out animation
        animators[index].SetTrigger("PullOut");
    }

    bool IsAnimationPlaying()
    {
        Animator currentAnimator = animators[currentWeaponIndex];

        // Check if the animator is currently transitioning between states
        if (currentAnimator.IsInTransition(0))
        {
            return true; // Animation is in transition
        }

        // Get the current animation state info
        AnimatorStateInfo stateInfo = currentAnimator.GetCurrentAnimatorStateInfo(0);

        // Check if the current animation is still playing and if it's not the "Idle" animation
        if (stateInfo.normalizedTime < 1.0f && !stateInfo.IsName("Idle"))
        {
            return true; // Animation other than idle is still playing
        }

        return false; // Either idle animation is playing or no animation is playing
    }
}