using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Room StarterRoom;
    [SerializeField] private GameObject BlockRoom; 
    [SerializeField] private GameObject Corridor;

    [SerializeField] private int DesiredRoomCount = 7;
    
    
    [SerializeField] private List<Room> LeftRooms;
    [SerializeField] private List<Room> RightRooms;
    [SerializeField] private List<Room> TopRooms;
    [SerializeField] private List<Room> BottomRooms;


    [SerializeField] private GameObject BossEncounter;

    [SerializeField] private List<Room> rooms = new List<Room>();
   private List<GameObject> blockRooms = new List<GameObject>();

    private int roomCount = 1;
    
    
    [SerializeField]
    private NavMeshSurface navMesh;


    private void Start()
    {
        navMesh.BuildNavMeshAsync();
    }


    private void Awake()
    {
        rooms.Add(StarterRoom);
        
        
        for (int i = 0; i < StarterRoom.Doors.Count; i++)
        {
            StarterRoom.Doors[i].SetActive( Random.Range(0, 10) <= 1);   
        }

        for (int i = 0; i < StarterRoom.Doors.Count; i++)
        {

          Vector3 anchor = StarterRoom.GetAnchor(i);


          if (anchor != Vector3.zero)
          {
                StartCoroutine(CreateRoom(anchor, i, StarterRoom));
          }

        }
        
        StartCoroutine(BakeNavMesh());
        
        
    }


    private IEnumerator BakeNavMesh()
    {
        
        yield return new WaitForSeconds(DesiredRoomCount * 0.1f +3f);
        navMesh.BuildNavMeshAsync();
        yield return new WaitForSeconds(2f);
        SetBossRoom();
    }


    private void SetBossRoom()
    {
        Room room = null;
        float distance = 0;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (Vector3.Distance(transform.position, rooms[i].transform.position) > distance)
            {
                distance = Vector3.Distance(transform.position, rooms[i].transform.position);
                room = rooms[i];
            }
        }
        print(room.name + " " + room.transform.position);
        room.Encounters.Clear();
        room.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;

        CreateBossRoom(room);
       // FillInHoles();
    }

    private void SetTreasureRoom()
    {

    }
    

    private IEnumerator CreateRoom(Vector3 anchor, int directionIndex , Room previousRoom)
    {
        yield return new WaitForSeconds(roomCount * 0.1f);
        
        bool roomExists = false;
        

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].transform.position == anchor)
            {
                roomExists = true;
            }
        }

        int roomDirectionIndex = 0;
        Room spawningRoom = null;
        // up
        if (directionIndex == 0)
        {
            spawningRoom = BottomRooms[Random.Range(0, BottomRooms.Count)];
            roomDirectionIndex = 3;

            //left    
        }
        else if (directionIndex == 1)
        {
            spawningRoom = RightRooms[Random.Range(0, RightRooms.Count)];
            roomDirectionIndex = 2;

            //right    
        }
        else if (directionIndex == 2)
        {
            spawningRoom = LeftRooms[Random.Range(0, LeftRooms.Count)];
            roomDirectionIndex = 1;
            //down    
        }
        else
        {
            spawningRoom = TopRooms[Random.Range(0, TopRooms.Count)];
        }
        Room roomInstance = null;

        if (roomCount < DesiredRoomCount && !roomExists)
        {
            roomCount++;
            roomInstance = Instantiate(spawningRoom, anchor, Quaternion.identity);
            roomInstance.transform.parent = transform.GetChild(0);
            roomInstance.DisableAnchor(roomDirectionIndex);
            rooms.Add(roomInstance);

            previousRoom.neighbors[roomDirectionIndex] = roomInstance;
            roomInstance.neighbors[3-roomDirectionIndex] = previousRoom;

            for (int i = 0; i < roomInstance.Doors.Count; i++)
            {
                Vector3 newAnchor =  roomInstance.GetAnchor(i);
                if (newAnchor != Vector3.zero)
                {
                    StartCoroutine(CreateRoom(newAnchor , i, roomInstance));
                }
            }
        }else if (roomCount >= DesiredRoomCount && !roomExists)
        {
            roomInstance =  Instantiate(BlockRoom, anchor, Quaternion.identity).GetComponent<Room>();
            blockRooms.Add(roomInstance.gameObject);
            previousRoom.neighbors[roomDirectionIndex] = roomInstance;
            roomInstance.neighbors[3 - roomDirectionIndex] = previousRoom;
        }
    }
    private void FillInHoles()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (rooms[i].CheckAnchor(j)){
                    Instantiate(BlockRoom, rooms[i].GetAnchor(j), Quaternion.identity);
                   // rooms[i].DisableAnchor(j);
                }
            }
        }
    }

    private void CreateBossRoom(Room entrance)
    {
        //get opposite room of the entrance to corridor room
        for (int i = 0; i < entrance.neighbors.Length; i++)
        {
            if (entrance.neighbors[i] != null && !entrance.neighbors[i].name.Contains("BlockRoom"))
            {
                if (entrance.neighbors[3 - i] != null)
                {
                    print("Destroyed opposite room");
                    Destroy(entrance.neighbors[3 - i].gameObject);
                }
                print("Destroyed opposite door");
                Destroy(entrance.Doors[3 - i].gameObject);
                Destroy(entrance.Doors[ i].gameObject);


                Room corridor = Instantiate(Corridor , entrance.GetAnchor(3-i)+ entrance.transform.position, Quaternion.identity).GetComponent<Room>();
                if (i == 1)
                {
                    print("Get rotated idiot");
                    corridor.transform.Rotate(new Vector3(0, 0, 90));
                }
                else if (i == 2)
                {
                    print("Get rotated idiot");
                    corridor.transform.Rotate(new Vector3(0, 0, -90));
                }
                else if (i == 3)
                {
                    print("Get rotated idiot");
                    corridor.transform.Rotate(new Vector3(0, 0, -180));
                }
            }
        }

        
    }

    private void UnlockDoor(Room otherRoom , int directionIndex)
    {
        // up
        if (directionIndex == 0)
        {
            otherRoom.Doors[3].SetActive(false);
                
            //left    
        }else if (directionIndex == 1)
        {
          
            otherRoom.Doors[2].SetActive(false);
            //right    
        }else if (directionIndex == 2)
        { 
            
            otherRoom.Doors[1].SetActive(false);
            //down    
        }else
        {
            otherRoom.Doors[0].SetActive(false);
        }
    }
    
    
}
