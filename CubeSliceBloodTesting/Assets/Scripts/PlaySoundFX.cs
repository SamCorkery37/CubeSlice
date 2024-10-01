using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;

    public void PlaySound(int clipIndex)
    {
        switch (clipIndex)
        {
            case 1:
                audioSource.PlayOneShot(clip1);
                break;
            case 2:
                audioSource.PlayOneShot(clip2);
                break;
            case 3:
                audioSource.PlayOneShot(clip3);
                break;
            default:
                Debug.Log("Invalid clip index.");
                break;
        }
    }

}
