using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundPool : MonoBehaviour
{
    public static SoundPool instance;

    public GameObject soundPrefab;
    public int poolSize = 10;

    public List<SoundClip> soundClips;
    private Queue<AudioSource> soundPool = new Queue<AudioSource>();
    private Dictionary<string, AudioClip> clipDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePool();
        InitializeClipDictionary();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject soundObj = Instantiate(soundPrefab);
            soundObj.transform.SetParent(transform); 
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.gameObject.SetActive(false); 
            soundPool.Enqueue(audioSource);
        }
    }

    private void InitializeClipDictionary()
    {
        clipDictionary.Clear();
        foreach (var soundClip in soundClips)
        {
            if (!clipDictionary.ContainsKey(soundClip.name))
            {
                clipDictionary.Add(soundClip.name, soundClip.clip);
            }
        }
    }
    public void PlaySound(string clipName)
    {
        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource audioSource = GetAudioSource();
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(ReturnToPoolAfterPlaying(audioSource, clip.length));
        }
    }
    private IEnumerator ReturnToPoolAfterPlaying(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        ReturnAudioSource(audioSource);
    }

    public AudioSource GetAudioSource()
    {
        if (soundPool.Count > 0)
        {
            AudioSource audioSource = soundPool.Dequeue();
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }
        else
        {
            GameObject soundObj = Instantiate(soundPrefab);
            soundObj.transform.SetParent(transform);
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            return audioSource;
        }
    }

    public void ReturnAudioSource(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(false);
        if (!soundPool.Contains(audioSource)) 
        {
            soundPool.Enqueue(audioSource);
        }
    }



    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
    }
}
