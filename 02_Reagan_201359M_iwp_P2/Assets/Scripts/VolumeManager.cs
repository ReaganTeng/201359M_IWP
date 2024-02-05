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
    void Start()
    {

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            //Debug.Log("MASTER");
            PlayerPrefs.SetFloat("MasterVolume", 1.0f);
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            SoundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }
        else
        {

            PlayerPrefs.SetFloat("SoundVolume", 1.0f);
            SoundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");

        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        mastervolume = PlayerPrefs.GetFloat("MasterVolume"); // Adjust this value as needed


        OnValueChangedSliders();
        AdjustAllVolumes();

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

        //Debug.Log($"VOLUME IS {volume}");

        // Find all GameObjects with AudioSource components in the scene
        GameObject[] soundeffects = FindObjectsOfType<GameObject>()
      .Where(obj => obj.CompareTag("MusicPlayer") == false)
      .ToArray();

        foreach (GameObject obj in soundeffects)
        {
            AdjustVolumeRecursive(obj.transform, volume);
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
            //Debug.Log($"VOLUME IS {mp.GetComponent<AudioSource>().volume}");
        }
    }

    void AdjustVolumeRecursive(Transform parent, float volume)
    {
        foreach (AudioSource audioSource in parent.GetComponentsInChildren<AudioSource>())
        {
            if (audioSource.volume != volume)
            {
                //soundvolume + percentage of sound volume
                audioSource.volume = volume *
                    ((mastervolume * 100) / 100);
            }
        }
    }

    

}
