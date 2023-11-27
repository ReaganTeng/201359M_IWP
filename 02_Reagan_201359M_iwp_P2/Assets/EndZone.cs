using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndZone : MonoBehaviour
{

    GameObject player;
    public GameObject Panel;

    float countdown;
    bool countdown_started;
    public int money;
    private void Awake()
    {
        money = 0;
        countdown = 0;
        countdown_started = false;
        player = GameObject.FindWithTag("Player");
    }


    public void Update()
    {
        if (countdown_started)
        {
            countdown += 1 * Time.deltaTime;

            if(countdown >= 2.0f)
            {
                Debug.Log("QUIT");
                Application.Quit();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        //&& collision.gameObject.GetComponent<Player>().immunity_timer <= 0.0f)
        {
           Panel.SetActive(true);
            countdown_started = true;

            //COUNT DOWN THE MONEY
            foreach(var slot in collision.GetComponent<Inventory>().slots)
            {
                for(int i = 0; i < slot.Quantity; i++)
                {
                    money += (int)slot.CurrentItem.money;
                    PlayerPrefs.SetInt("GrossMoney", PlayerPrefs.GetInt("GrossMoney") + money);
                }
            }

            
        }
    }
}
