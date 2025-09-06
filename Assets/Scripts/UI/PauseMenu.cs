using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject Menu;
     [SerializeField] private GameObject ChangeWeaponButton;
    public PauseMenuElement[] WeaponCards = new PauseMenuElement[4];

    [SerializeField] private Transform CardAnchor;


    public int CurrentCardIndex;

    [SerializeField] private Player player;


    private void Start()
    {
           ToggleUI(false);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUI(false);
        }

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && Menu.activeSelf)
        {
            ChangeCard(-1);
        }else if((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && Menu.activeSelf)
        {
            ChangeCard(1);
        }
    }



    public void ChangeCard(int direction)
    {

        if (direction == -1 && CurrentCardIndex != 0) 
        {
            CurrentCardIndex += direction;
            SetForwardCard(CurrentCardIndex);
        }
        else if(direction == -1 && CurrentCardIndex == 0) 
        {
            //exception

        }
        else if(direction == 1 && CurrentCardIndex == 3)
        {
            //exception
        
        }
        else if (direction == 1 && CurrentCardIndex != 3)
        {
            CurrentCardIndex += direction;
            SetForwardCard(CurrentCardIndex);
        }

    }


    public void ToggleUI(bool toggleButton)
    {
        if (Menu.activeSelf)
        {
            Menu.SetActive(false);
        }
        else
        {
            Menu.SetActive(true);
        }
        ChangeWeaponButton.SetActive(toggleButton);




        for (int i = 0; i < player.EquippedWeapons.Count; i++)
        {
            WeaponCards[i].Weapon = player.EquippedWeapons[i];
            WeaponCards[i].WeaponSprite = player.EquippedWeapons[i].GetComponentInChildren<SpriteRenderer>().sprite;
            WeaponCards[i].Name = player.EquippedWeapons[i].transform.name;
        }
    }

    private void SetForwardCard(int index)
    {
        for (int i = 0; i < WeaponCards.Length; i++)
        {

            if (i == index)
            {
                WeaponCards[i].StopAllCoroutines();
                WeaponCards[i].StartCoroutine(WeaponCards[i].MoveToLocation(CardAnchor.position , new Vector3(1f, 1f, 1f)));
                WeaponCards[i].SetSiblingOrder(3);
            }
            else if (i < index)
            {
                WeaponCards[i].StopAllCoroutines();
                WeaponCards[i].StartCoroutine(WeaponCards[i].MoveToLocation(CardAnchor.position - (new Vector3(-70 + (10* (index-i)), 0, 0) * (index - i)), new Vector3(1, 1, 1) * (1f- ((index-i)/10f))));
                WeaponCards[i].SetSiblingOrder(i);
            }
            else
            {
                WeaponCards[i].StopAllCoroutines();
                WeaponCards[i].StartCoroutine(WeaponCards[i].MoveToLocation(CardAnchor.position - (new Vector3(70 - (10 * (i -index)), 0, 0) * (i - index)), new Vector3(1f, 1f, 1f) * (1f- ((i-index) / 10f))));
                WeaponCards[i].SetSiblingOrder(WeaponCards.Length - i-1);
            }

        }
    }


    public void ReplaceWeapon()
    {
        player.SwapWeapon(CurrentCardIndex);
        player.Interact(true);
    }

}