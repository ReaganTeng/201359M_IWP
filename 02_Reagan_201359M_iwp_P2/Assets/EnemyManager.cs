using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{

    public List<GameObject> enemyList;
    float timer;

    //THE NUMBER OF ENEMIES DECIDED TO GO CHASE MODE
    int enemiesChosen;

    int numberofEnemiesChosen;

    public int maxnumberenemies;

    public List<GameObject> enemyPrefabs;

    public MapGenerator mapGenerator;


    List<GameObject> enemiesInChaseMode = new List<GameObject>();


    List<Vector2> takenPos = new List<Vector2>();

    //FOR ORGANIZATION PURPOSES IN INSPECTOR
    public GameObject enemyparent;


    //public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    Vector3 prevpos = Vector3.zero;

    bool generationover = false;

    public void StartItself()
    {
        if (!generationover)
        {
            timer = 0;
            maxnumberenemies = 100;
            SpawnEnemy();
        }
    }


    void SpawnEnemy()
    {
        Vector2 randomPosition;
        int enemytype = UnityEngine.Random.Range(0, 3);
        // Find the smallest and largest X and Y values
        float smallestX = mapGenerator.occupiedPositions.Min(pos => pos.x);
        float largestX = mapGenerator.occupiedPositions.Max(pos => pos.x);
        float smallestY = mapGenerator.occupiedPositions.Min(pos => pos.y);
        float largestY = mapGenerator.occupiedPositions.Max(pos => pos.y);

        //HARD CODE THE VALUES FIRST
        //float smallestX = -64;
        //float largestX =0;
        //float smallestY = 0;
        //float largestY = 32;


        for (int i = 0; i < maxnumberenemies; i++)
        {
            int randomX = Random.Range((int)smallestX, (int)largestX + 1);
            int randomY = Random.Range((int)smallestY, (int)largestY + 1);
            randomPosition = new Vector2(
                randomX,
               randomY
            );
            if (randomPosition != Vector2.zero
                &&
                !takenPos.Contains( randomPosition )
                &&
                IsPositionValid(randomPosition))
            {
                GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], randomPosition, Quaternion.identity);
                //GameObject enemy = Instantiate(enemyPrefabs[0], randomPosition, Quaternion.identity);

                enemyList.Add(enemy);
                enemy.transform.SetParent(enemyparent.transform);
                takenPos.Add(randomPosition);
                continue; // Successful spawn, move to the next loop iteration
            }
        }

        takenPos.Clear();
        generationover = true;
        //enemiesspawned = true;
    }

    bool IsPositionValid(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f, LayerMask.GetMask("WallTilemap"));

        if (colliders.Length > 0)
        {
            // There is a wall tile at the position
            return false;
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float distance = Vector2.Distance(position, prevpos);
            float distance2 = Vector2.Distance(position, mapGenerator.startingposition);

            if (distance <= mapGenerator.roomdimension
                && distance2 <= mapGenerator.roomdimension)
            {
                // Too close to another enemy
                return false;
            }
        }

        return true;
    }



    // Update is called once per frame
    void Update()
    {
        if (generationover)
        {
            QuantityManager();
            FSMManager();
        }
    }


     void QuantityManager()
    {
        Vector2 randomPosition;

        if (enemyList.Count < maxnumberenemies)
        {
            //HARD CODE THE VALUES FIRST
            float smallestX = -64;
            float largestX = 0;
            float smallestY = 0;
            float largestY = 32;



            for (int i = 0; i < maxnumberenemies - enemyList.Count; i++)
            {

                int randomX = Random.Range((int)smallestX, (int)largestX + 1);
                int randomY = Random.Range((int)smallestY, (int)largestY + 1);
                randomPosition = new Vector2(
                    randomX,
                   randomY
                );
                if (randomPosition != Vector2.zero
                    &&
                    !takenPos.Contains(randomPosition)
                    &&
                    IsPositionValid(randomPosition))
                {
                    GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], randomPosition, Quaternion.identity);
                    enemyList.Add(enemy);
                    continue;
                }
            }

            takenPos.Clear();
        }
    }

    void FSMManager()
    {
        bool allNotAboutToAttack
           = enemyList.All(enemy => enemy.GetComponent<Enemy>().currentState != Enemy.EnemyState.ABOUT_TO_ATTACK);
        bool allnotAttack
           = enemyList.All(enemy => enemy.GetComponent<Enemy>().currentState != Enemy.EnemyState.ATTACK);
        //bool anynotAttack
        //   = enemyList.All(enemy => enemy.GetComponent<Enemy>().currentState != Enemy.EnemyState.ATTACK)
        if (allNotAboutToAttack
            && allnotAttack)
        {
            //Debug.Log("PREPARING TO CHOOSE");
            timer += 1.0f * Time.deltaTime;
        }
        else
        {
            timer = 0;
        }

        if (timer >= 3.0f)
        {
            //see how many enemies are currently in chase mode
            foreach (var enemies in enemyList)
            {
                if (enemies.GetComponent<Enemy>().currentState == Enemy.EnemyState.CHASE)
                {
                    Debug.Log("ADDED");
                    enemiesInChaseMode.Add(enemies);
                }
            }
            if (enemiesInChaseMode.Count > 0)
            {
                //choose the number of enemies to go to about to attack mode
                numberofEnemiesChosen = Random.Range(1, enemiesInChaseMode.Count);
                for (int i = 0; i < numberofEnemiesChosen; i++)
                {
                    int enemyindex = Random.Range(0, enemiesInChaseMode.Count);

                    Enemy enemyScript = enemiesInChaseMode[enemyindex].GetComponent<Enemy>();

                    enemyScript.currentState = Enemy.EnemyState.ABOUT_TO_ATTACK;
                    enemyScript.spriteRenderer.color = Color.red;
                    Debug.Log("ABOUT TO ATTACK");

                }
            }
            enemiesInChaseMode.Clear();
            timer = 0;
        }
    }
}
