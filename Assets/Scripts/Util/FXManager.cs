using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{


    //References to objects
    [SerializeField] private GameObject SparkVFX;



    //Instances of Objects
    private List<GameObject> SparkVFXInstances = new List<GameObject>();

    public static FXManager Instance { get; private set; }



    private void Awake()
    {
        if(Instance == null)
        {

            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CreateInstances();
    }





    private void CreateInstances()
    {
        for (int i = 0; i < 10; i++)
        {
            //Creating spark VFX instances
            GameObject instance = Instantiate(SparkVFX, transform.GetChild(0));
            SparkVFXInstances.Add(instance);
            instance.SetActive(false);
        }
    }


    private IEnumerator DisableInstance(GameObject instance , float time)
    {
        yield return new WaitForSeconds(time);
        instance.SetActive(false);
    }

    public GameObject GetPooledObject(EffectType effect)
    {

        if(effect == EffectType.Spark) { 
            for (int i = 0; i < SparkVFXInstances.Count; i++)
            {
                if (!SparkVFXInstances[i].activeSelf)
                {
                    return SparkVFXInstances[i];
                }
            }

        }


        return null;
    }

    public void PoolObject(GameObject pooledObject , float time , Vector3 position, Quaternion rotation)
    {
        pooledObject.SetActive(true);
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        StartCoroutine(DisableInstance(pooledObject, time));
    }


    public enum EffectType{
        None,
        Spark,
        Blood
    }


}
