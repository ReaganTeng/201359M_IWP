using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip HubWorldMusic;
    public AudioClip MainMenuMusic;
    public AudioClip GameSceneMusic;
    public AudioClip gameOverMusic;
    public AudioClip gameWinMusic;

    [HideInInspector] public AudioSource AS;

    Scene currentScene;
    const string GameScene = "GameScene";
    const string HubWorldScene = "HubWorld";
    const string MainMenuScene = "MainMenu";


    PlayerManager PM;
    GameObject GameCompletePanel;

    // Start is called before the first frame update
    void Awake()
    {
        PM = FindObjectOfType<PlayerManager>();

        AS = GetComponent<AudioSource>();
        AS.loop = true;

        
        currentScene = SceneManager.GetActiveScene();
        if (FindObjectOfType<MenuManager>() != null)
        {
            GameCompletePanel = FindObjectOfType<MenuManager>().GameCompletePanel;
        }
        PlayStartingMusic();
    }


    void PlayStartingMusic()
    {
        switch(currentScene.name)
        {
            case GameScene:
                AS.clip = GameSceneMusic;
                break;
            case HubWorldScene:
                AS.clip = HubWorldMusic;
                break;
            case MainMenuScene:
                AS.clip = MainMenuMusic;
                break;
            default:
                break;
        }

        AS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(PM == null
           || PM.players == null)
        {
            return;
        }

        bool allDead = PM.players.All(p => p.GetComponent<Player>().health <= 0);
        if (allDead)
        {
            PlayMusicWithoutLoop(gameOverMusic);
        }

        if(GameCompletePanel.GetComponent<CanvasGroup>().interactable)
        {
            PlayMusicWithoutLoop(gameWinMusic);

        }
    }

    public void PlayMusicWithoutLoop(AudioClip cliptoplay)
    {
        if (AS.clip == cliptoplay)
        {
            return;
        }
        AS.loop  = false;
        AS.clip = cliptoplay;
        // Set the time to 0 (start from the beginning)
        AS.time = 0f;
        AS.Play();
    }
}
