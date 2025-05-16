using UnityEngine;

public class Room : MonoBehaviour
{
    
    public GameObject[] Doors;
    
    [SerializeField] private GameObject[] Anchors;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created


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
    
    
    
}
