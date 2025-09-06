using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
   public Weapon Weapon;

    [SerializeField] private bool IsCard;

   [SerializeField] private GameObject UI;

   private GameObject UIInstance;
   
   
   private TextMeshProUGUI WeaponName;
   private TextMeshProUGUI Damage;
   private TextMeshProUGUI Weight;
   private TextMeshProUGUI SwingSpeed;
   private TextMeshProUGUI WeaponType;
   private TextMeshProUGUI Knockback;


    protected Transform UIChild;

    [HideInInspector] public string Name;

   private void Awake()
   {
        if(GetComponent<Weapon>() != null) { 
            Weapon = GetComponent<Weapon>();
        }
    }


   public void Setup()
   {
      
      if(UIInstance){
         Destroy(UIInstance);
      }


        if (Weapon) {

            if (!IsCard) { 
            UIInstance = Instantiate(UI , transform.position+ new Vector3(0,5,0) , quaternion.identity);
            }
            else
            {
                UIInstance = Instantiate(UI, transform);
                UIInstance.transform.SetSiblingIndex(1);
            }


            UIChild  = UIInstance.transform.GetChild(0);

          WeaponName = UIChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
          WeaponType = UIChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
          Damage = UIChild.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
          Weight = UIChild.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
          SwingSpeed = UIChild.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
          Knockback = UIChild.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

            if (!IsCard) { 
                WeaponName.text = transform.name;
            }
            else
            {
                WeaponName.text = Name;
            }

          WeaponName.text.Replace("(Clone)", "");
          WeaponType.text = Weapon.Type.ToString();
          Damage.text = Weapon.BaseWeaponDamage.ToString();
          Weight.text = Weapon.gameObject.GetComponent<Rigidbody2D>().mass.ToString();
          SwingSpeed.text = Weapon.MovementSpeed.ToString();
          Knockback.text = Weapon.KnockbackForce.ToString();

        }
        else
        {
            print("NO WEAPON YOU STUPID");
        }
    }


   public void CloseUI()
   {
      print("LEFT");
      Destroy(UIInstance);
   }

   private void OnDisable()
   {
      CloseUI();
   }


}
