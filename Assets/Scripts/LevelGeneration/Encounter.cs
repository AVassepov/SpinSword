using System;
using System.Collections.Generic;
using NUnit.Framework;


using UnityEngine;
using Random = UnityEngine.Random;

public class Encounter : MonoBehaviour
{

    public bool Started;
    
    
     
    public List<Enemy> Enemies;
    public List<Transform> EnemySpawners;


    


    public void SpawnEnemies()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            int enemyIndex =  Random.Range(0, Enemies.Count);
            
            
            int SpawnerIndex =  Random.Range(0, EnemySpawners.Count);



            Enemy enemyInstance = Instantiate(Enemies[enemyIndex] , EnemySpawners[SpawnerIndex]);

            Enemies.RemoveAt(enemyIndex);
            EnemySpawners.RemoveAt(SpawnerIndex);


        }
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!Started && TryGetComponent<Player>(out Player player))
        {
            Started = true;
            SpawnEnemies();
        }
        
        
        
    }
}
