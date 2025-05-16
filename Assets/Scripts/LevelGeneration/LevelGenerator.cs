using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Room starterRoom;
    [SerializeField] private GameObject BlockRoom;

    [SerializeField] private int DesiredRoomCount = 7;
    
    
    [SerializeField] private List<Room> LeftRooms;
    [SerializeField] private List<Room> RightRooms;
    [SerializeField] private List<Room> TopRooms;
    [SerializeField] private List<Room> BottomRooms;


    

    private List<Room> Rooms = new List<Room>();
    
    private int roomCount = 1;
    
    
    [SerializeField]
    private NavMeshSurface navMesh;


    private void Start()
    {
        navMesh.BuildNavMeshAsync();
    }


    private void Awake()
    {
        Rooms.Add(starterRoom);
        
        
        for (int i = 0; i <    starterRoom.Doors.Length; i++)
        {
            starterRoom.Doors[i].SetActive( Random.Range(0, 10) <= 1);   
        }

        for (int i = 0; i < starterRoom.Doors.Length; i++)
        {

          Vector3 anchor =  starterRoom.GetAnchor(i);


          if (anchor != Vector3.zero)
          {
              StartCoroutine(CreateRoom(anchor , i));

          }

        }
        
        StartCoroutine(BakeNavMesh());
        
        
    }


    private IEnumerator BakeNavMesh()
    {
        
        yield return new WaitForSeconds(DesiredRoomCount * 0.1f +1f);
        navMesh.BuildNavMeshAsync();
    }
    

    private IEnumerator CreateRoom(Vector3 anchor, int directionIndex)
    {
        yield return new WaitForSeconds(roomCount * 0.1f);
        
        bool roomExists = false;
        

        for (int i = 0; i < Rooms.Count; i++)
        {
            if (Rooms[i].transform.position == anchor)
            {
                roomExists = true;
            }
        }
        
        

        if (roomCount < DesiredRoomCount && !roomExists)
        {
            roomCount++;

            int roomDirectionIndex = 0;
            Room spawningRoom = null;
            // up
            if (directionIndex == 0)
            {
                spawningRoom = TopRooms[Random.Range(0, TopRooms.Count)];
                roomDirectionIndex = 3;
                
             //left    
            }else if (directionIndex == 1)
            {
                spawningRoom = LeftRooms[Random.Range(0, LeftRooms.Count)];
                roomDirectionIndex = 2;

                //right    
            }else if (directionIndex == 2)
            { 
                spawningRoom = RightRooms[Random.Range(0, RightRooms.Count)];
                roomDirectionIndex = 1;
                //down    
            }else
            {
                spawningRoom = BottomRooms[Random.Range(0, BottomRooms.Count)];
            }
            Room roomInstance = Instantiate(spawningRoom, anchor, Quaternion.identity);
            roomInstance.transform.parent = transform.GetChild(0);
            roomInstance.DisableAnchor(roomDirectionIndex);
            Rooms.Add(roomInstance);
            
            
            for (int i = 0; i < roomInstance.Doors.Length; i++)
            {

                Vector3 newAnchor =  roomInstance.GetAnchor(i);


                if (newAnchor != Vector3.zero)
                {
                    StartCoroutine(CreateRoom(newAnchor , i));

                }

            }
        }else if (roomCount >= DesiredRoomCount && !roomExists)
        {
            Instantiate(BlockRoom, anchor, Quaternion.identity);
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
