using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public List<GameObject> enemyList;
    float timer;

    //THE NUMBER OF ENEMIES DECIDED TO GO CHASE MODE
    int enemiesChosen;


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            //if (enemyList[i].GetComponent<Enemy>() != null)
            //{
            //    Debug.Log("FIND ENEMY SCRIPT");
            //}

            if (enemyList[i].GetComponent<Enemy>().currentState == Enemy.EnemyState.CHASE)
            {

            }
        }
    }
}
