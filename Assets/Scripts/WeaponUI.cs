using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
   private Weapon weapon;


   [SerializeField] private GameObject UI;

   private GameObject UIInstance;
   
   
   private TextMeshProUGUI WeaponName;
   private TextMeshProUGUI Damage;
   private TextMeshProUGUI Weight;
   private TextMeshProUGUI SwingSpeed;
   private TextMeshProUGUI WeaponType;
   private TextMeshProUGUI Knockback;


   private void Awake()
   {
      weapon = GetComponent<Weapon>();
   }


   public void Setup()
   {
      
      if(UIInstance){
         Destroy(UIInstance);
      }
      
      
      
      UIInstance = Instantiate(UI , transform.position+ new Vector3(0,5,0) , quaternion.identity);

      WeaponName = UIInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
      WeaponType = UIInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
      Damage = UIInstance.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
      Weight = UIInstance.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
      SwingSpeed = UIInstance.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
      Knockback = UIInstance.transform.GetChild(6).GetComponent<TextMeshProUGUI>();


      WeaponName.text = transform.name;
      WeaponName.text.Replace("(Clone)", "");
      WeaponType.text = weapon.Type.ToString();
      Damage.text = weapon.BaseWeaponDamage.ToString();
      Weight.text = GetComponentInChildren<Rigidbody2D>().mass.ToString();
      SwingSpeed.text = weapon.MovementSpeed.ToString();
      Knockback.text = weapon.KnockbackForce.ToString();
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
