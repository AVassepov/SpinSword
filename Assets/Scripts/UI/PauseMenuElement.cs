using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuElement : WeaponUI
{
    public Sprite WeaponSprite;


    [SerializeField]private Image WeaponImage;

    private WeaponUI weaponUI;
    private TextMeshProUGUI bonusEffects;


    private void OnEnable()
    {
        Invoke("Setup", 0.1f);
        Invoke("DisplayBonusEffects", 0.1f);
    }


    private void DisplayBonusEffects()
    {
        WeaponImage.sprite = WeaponSprite;

        if (UIChild.transform.GetChild(6) && UIChild.transform.GetChild(6).GetComponent<TextMeshProUGUI>()) { 
        bonusEffects = UIChild.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        bonusEffects.text = "";
       string constantString ="", equipString = "", unequipString = "";

        for (int i = 0; i < Weapon.PassiveEffects.Count; i++)
        {

            if (Weapon.PassiveEffects[i].Condition == PassiveEffect.ActivationCondition.Constant)
            {
                if (constantString =="")
                {
                    constantString += "<b>Constant Effects</b> \n";
                }

                if(Weapon.PassiveEffects[i].Duration!= 0) { 
                constantString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage +" for " + Weapon.PassiveEffects[i].Duration + " seconds \n";
                }
                else
                {
                    constantString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage + "\n";
                }

                bonusEffects.text += constantString;
            }
            else if (Weapon.PassiveEffects[i].Condition == PassiveEffect.ActivationCondition.WhileEquiped)
            {
                if (equipString == "")
                {
                    equipString += "<b>While Equipped Effects</b> \n";
                }

                if (Weapon.PassiveEffects[i].Duration != 0)
                {
                    equipString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage + " for " + Weapon.PassiveEffects[i].Duration + " seconds \n";
                }
                else
                {
                    equipString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage + "\n";
                }

                bonusEffects.text += equipString;
            }
            else
            {
                if (unequipString == "")
                {
                    unequipString += "<b>While Unequipped Effects</b> \n";
                }

                if (Weapon.PassiveEffects[i].Duration != 0)
                {
                    unequipString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage + " for " + Weapon.PassiveEffects[i].Duration + " seconds \n";
                }
                else
                {
                    unequipString += Weapon.PassiveEffects[i].Type.ToString() + " at " + Weapon.PassiveEffects[i].Percentage + "\n";
                }

                bonusEffects.text += unequipString;
            }



        }
         
        if (Weapon.PassiveEffects.Count > 0) {

        }
        else
        {
            bonusEffects.text = "";
        }
        }
    }


    public IEnumerator MoveToLocation(Vector3 target, Vector3 targetSize)
    {
        
        float startTime = Time.time; // Record the start time

        // Loop as long as the elapsed time is less than the desired duration
        while (Time.time < startTime + 1f)
        {
            transform.position = Vector3.Slerp(transform.position, target, 0.02f);
            transform.localScale = Vector3.Slerp(transform.localScale, targetSize, 0.02f);
            yield return null;
        }


        yield return null;


    }

    public void SetSiblingOrder(int index)
    {
        transform.SetSiblingIndex(index);
    }

}

