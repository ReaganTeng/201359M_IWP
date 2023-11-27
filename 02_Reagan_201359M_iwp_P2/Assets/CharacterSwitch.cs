using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSwitch : MonoBehaviour
{

    public GameObject gamemanager;
    PlayerManager playermanager;
    int numberofplayers;
    //public List<GameObject> playerlist;

    public List<Button> buttonlist = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        playermanager = gamemanager.GetComponent<PlayerManager>();


        Button[] buttonsInScene = GameObject.FindObjectsOfType<Button>();
        // Add the found buttons to the list
        buttonlist.AddRange(buttonsInScene);

        //numberofplayers = playermanager.players.Count;
    }

    void AssignIcons()
    {
        //THE ICONS IN BUTTONLIST MUST BE THE SAME AS PLAYER ICON
        for(int i = 0; i < buttonlist.Count; i++)
        {
            buttonlist[i].image = playermanager.players[i].GetComponent<Player>().icon;
        }

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
