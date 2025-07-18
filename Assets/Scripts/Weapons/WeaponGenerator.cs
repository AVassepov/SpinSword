using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{

    public static WeaponGenerator Instance { get; private set; }
    public GameParameters Parameters;


    public List<WeaponStats> CommonWeapons;
    public List<WeaponStats> RareWeapons;
    public List<WeaponStats> EpicWeapons;
    public List<WeaponStats> LegendaryWeapons;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void SetWeapon(Weapon Target)
    {
        int rarityRoll = Random.Range(0,100);




        if (rarityRoll <= Parameters.ItemDiscovery)
        {
            Target.WeaponRarity = Weapon.Rarity.Legendary;
            Target.BaseWeaponDamage += Random.Range(7,18);
            Transform child = Instantiate(LegendaryWeapons[Random.RandomRange(0, LegendaryWeapons.Count)].WeaponPrefab, Target.transform).transform;
        }
        else if (rarityRoll <= Parameters.ItemDiscovery * 2) {
            Target.WeaponRarity = Weapon.Rarity.Epic;
            Target.BaseWeaponDamage += Random.Range(4, 13);
        }
        else if (rarityRoll <= Parameters.ItemDiscovery * 3)
        {
            Target.WeaponRarity = Weapon.Rarity.Rare;
            Target.BaseWeaponDamage += Random.Range(2, 8);
        }
        else
        {
            Target.WeaponRarity = Weapon.Rarity.Common;
            Target.BaseWeaponDamage += Random.Range(0,5);
        }
    }


    public void SetWeaponStats(Weapon target, WeaponStats data)
    {
        target.MovementSpeed = data.MovementSpeed;
        target.BaseWeaponDamage = data.BaseWeaponDamage;
        target.SpeedModifier = data.SpeedModifier;
        target.RotationSpeed = data.RotationSpeed;
        target.KnockbackForce = data.KnockbackForce;
    }

    public void SetPassives(Weapon Target, float bonusExtraPassive)
    {
        bool passivesGranted = false;

        while (!passivesGranted) { 
           PassiveEffect effect = new PassiveEffect();


            Target.PassiveEffects.Add(effect);

           effect.Percentage = Parameters.PlayerPower + Random.Range(-10 , 10);

            if (effect.Percentage < 0)
            {
                effect.Percentage += 12;
            }

            
            if (Random.Range(0,100) < bonusExtraPassive+ Parameters.ExtraPassiveChance)
            {
                bonusExtraPassive -= 100;
            }
            else
            {
                passivesGranted = true;
            }

        }
    }
}
[System.Serializable]
public class WeaponStats
{
    public GameObject WeaponPrefab;
    public float MovementSpeed;
    public float BaseWeaponDamage = 1;
    public float SpeedModifier = 0.1f;
    public float RotationSpeed;
    public float KnockbackForce = 1f;
}


[System.Serializable]
public class GameParameters
{
    public float PlayerPower;
    public float ItemDiscovery = 1;
    public float ExtraPassiveChance;
}