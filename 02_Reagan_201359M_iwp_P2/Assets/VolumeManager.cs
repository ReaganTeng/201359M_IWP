using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{

    [HideInInspector]
    public float mastervolume;

    // Start is called before the first frame update
    void Start()
    {
        mastervolume = PlayerPrefs.GetFloat("MasterVolume"); // Adjust this value as needed

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustAllVolumes()

    {
        //AdjustMasterVolume();
        AdjustSoundVolume();
    }

    //void AdjustMasterVolume()
    //{
    //    // Find all AudioSource components in the scene
    //    AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

    //    // Adjust the volume of each AudioSource
    //    foreach (AudioSource audioSource in audioSources)
    //    {
    //        audioSource.volume = mastervolume;
    //    }
    //}

    void AdjustSoundVolume()
    {
        float volume = PlayerPrefs.GetFloat("SoundVolume"); // Adjust this value as needed

        // Find all GameObjects with AudioSource components in the scene
        GameObject[] playersAndEnemies = GameObject.FindGameObjectsWithTag("Player")
            .Concat(GameObject.FindGameObjectsWithTag("Enemy"))
            .ToArray();

        foreach (GameObject playerOrEnemy in playersAndEnemies)
        {
            AdjustVolumeRecursive(playerOrEnemy.transform, volume);
        }
    }

    void AdjustVolumeRecursive(Transform parent, float volume)
    {
        // Adjust the volume of each AudioSource with "Player" or "Enemy" tag
        foreach (AudioSource audioSource in parent.GetComponentsInChildren<AudioSource>())
        {
            //Debug.Log($"AUDIOSOURCE VOLUME {audioSource.volume}");

            //soundvolume + percentage of sound volume
            audioSource.volume = volume * 
                ((mastervolume* 100) /100);
            float v = volume *
                ((mastervolume * 100) / 100);
            Debug.Log($"AUDIOSOURCE VOLUME {v}");
        }
    }

    

}
