using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{



    [SerializeField] private string SceneName;

    [SerializeField]private GameObject SpawnedObject;
    [SerializeField] private Vector3 Location;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && SceneName != "")
        {
            print("Player entered");
            SceneManager.LoadScene(SceneName);
        }else if (SpawnedObject)
        {
            Instantiate(SpawnedObject, Location , Quaternion.identity);
        }
    }



}
