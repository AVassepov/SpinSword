using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Encounter : MonoBehaviour
{

    public bool Started;
    
    
     
    public List<Enemy> Enemies;
    public List<Transform> EnemySpawners;


    [SerializeField] private GameObject Exit;

    private Player playerInstance;

    public List<Enemy> EnemyInstances = new List<Enemy>();

    public List<GameObject> Doors;

    public bool IsBoss;

    private void Start()
    {

        if (Enemies[0] is Boss)
        {
            IsBoss = true;
        }



        List<GameObject> openDoors = new List<GameObject>(); 

        for (int i = 0; i < Doors.Count; i++)
        {
            if (!Doors[i].activeSelf)
            {
                openDoors.Add(Doors[i]);
            }
        }

        Doors = openDoors;

    }


    public void SpawnEnemies()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            int enemyIndex =  Random.Range(0, Enemies.Count);
            
            
            int SpawnerIndex =  Random.Range(0, EnemySpawners.Count);



            Enemy enemyInstance = Instantiate(Enemies[enemyIndex] , EnemySpawners[SpawnerIndex].position, Quaternion.identity);

            Enemies.RemoveAt(enemyIndex);
            EnemySpawners.RemoveAt(SpawnerIndex);


            enemyInstance.Encounter = this;
            enemyInstance.Player = playerInstance;
            EnemyInstances.Add(enemyInstance);

        }
    }



    public void RemoveEnemy(Enemy enemy)
    {

        EnemyInstances.Remove(enemy);

       
        if(EnemyInstances.Count == 0)
        {
            FinishEncounter();
        }
    }

    private void FinishEncounter()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].SetActive(false);
        }

        if (Exit!=null)
        {
            Instantiate(Exit, transform.position , Quaternion.identity);
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!Started && other.TryGetComponent<Player>(out Player player))
        {
            playerInstance = player;
            Started = true;
            StartEncounter();
         }  
    }
    private void StartEncounter()
    {
        SpawnEnemies();

        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].SetActive(true);
        }
    }
}
