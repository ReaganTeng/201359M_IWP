using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{

    [HideInInspector]
    public float mastervolume;

    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SoundVolumeSlider;


    // Start is called before the first frame update
    void Awake()
    {
        mastervolume = PlayerPrefs.GetFloat("MasterVolume"); // Adjust this value as needed

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            SoundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        OnValueChangedSliders();
        //AdjustAllVolumes();


        //AdjustSoundVolume();
        //AdjustMusicVolume();
        //AdjustVolumeSlider("MasterVolume", MasterVolumeSlider);
        //AdjustVolumeSlider("SoundVolume", SoundVolumeSlider);
        //AdjustVolumeSlider("MusicVolume", MusicVolumeSlider);
    }



    public void OnValueChangedSliders()
    {
        MasterVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolumeSlider("MasterVolume", MasterVolumeSlider));
        SoundVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolumeSlider("SoundVolume", SoundVolumeSlider));
        MusicVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolumeSlider("MusicVolume", MusicVolumeSlider));

    }
    // Update is called once per frame
    void Update()
    {
        AdjustAllVolumes();

        //AdjustAllVolumes();
    }


    public void AdjustVolumeSlider(string dataname, Slider slider)
    {
        mastervolume = PlayerPrefs.GetFloat("MasterVolume"); // Adjust this value as needed

        //string dataname = "MasterVolume";
        float val = slider.value;
        PlayerPrefs.SetFloat(dataname, val);
        //Debug.Log($"VALUE IS {val}");

        //AdjustAllVolumes();
        //SendJSON(dataname, val);
    }
    public void AdjustAllVolumes()
    {
        //AdjustMasterVolume();
        AdjustSoundVolume();
        AdjustMusicVolume();
    }

   

    void AdjustSoundVolume()
    {
        float volume = PlayerPrefs.GetFloat("SoundVolume"); // Adjust this value as needed

        // Find all GameObjects with AudioSource components in the scene
        GameObject[] playersAndEnemies = GameObject.FindGameObjectsWithTag("Player")
            .Concat(GameObject.FindGameObjectsWithTag("Enemy"))
             .Concat(GameObject.FindGameObjectsWithTag("GameMGT"))
            .ToArray();

        foreach (GameObject playerOrEnemy in playersAndEnemies)
        {
            AdjustVolumeRecursive(playerOrEnemy.transform, volume);
        }
    }

    void AdjustMusicVolume()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume"); // Adjust this value as needed

        // Find all GameObjects with AudioSource components in the scene
        GameObject[] musicPlayer = GameObject.FindGameObjectsWithTag("MusicPlayer")
            .ToArray();
        foreach (GameObject mp in musicPlayer)
        {
            AdjustVolumeRecursive(mp.transform, volume);
        }
    }

    void AdjustVolumeRecursive(Transform parent, float volume)
    {
       
        foreach (AudioSource audioSource in parent.GetComponentsInChildren<AudioSource>())
        {
            //soundvolume + percentage of sound volume
            audioSource.volume = volume * 
                ((mastervolume* 100) /100);
            
        }
    }

    

}
