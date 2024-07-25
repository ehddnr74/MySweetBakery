using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sfxClips;
    public void PlaySound(int clipIndex, float volume = 1.0f)
    {
        if (clipIndex < 0 || clipIndex >= sfxClips.Length) 
        {
            return;
        }

        AudioSource audioSource = SoundPool.instance.GetAudioSource();
        audioSource.clip = sfxClips[clipIndex];
        audioSource.volume = volume;
        audioSource.Play();
        StartCoroutine(ReturnToPoolAfterPlaying(audioSource, audioSource.clip.length));
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(duration);
        SoundPool.instance.ReturnAudioSource(audioSource);
    }
}
