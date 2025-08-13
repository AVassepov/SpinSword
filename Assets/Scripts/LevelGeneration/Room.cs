using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public List<GameObject> Doors;

    [SerializeField] private GameObject[] Anchors;
    public List<GameObject>Encounters;


   public Room[] neighbors = new Room[4]; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

        if(Encounters.Count > 0 && transform.name != "BaseRoom") { 
            Encounter encounter = Instantiate(Encounters[Random.Range(0, Encounters.Count)], transform).GetComponent<Encounter>();
            encounter.Doors = Doors;
        }
    }


    public Vector3 GetAnchor(int index)
    {

        Vector3 anchor = Vector3.zero;
        
        if (!Doors[index].activeSelf &&  Anchors[index].activeSelf)
        {
            anchor  = Anchors[index].transform.position;
            
            Anchors[index].gameObject.SetActive(false);
            return anchor;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void DisableAnchor(int index)
    {
        Anchors[index].gameObject.SetActive(false);
    }
    
    public bool CheckAnchor(int index)
    {
        if (Anchors[index].activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    
}
