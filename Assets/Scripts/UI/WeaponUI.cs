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

        Transform UIChild = UIInstance.transform.GetChild(0);

      WeaponName = UIChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
      WeaponType = UIChild.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
      Damage = UIChild.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
      Weight = UIChild.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
      SwingSpeed = UIChild.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
      Knockback = UIChild.transform.GetChild(5).GetComponent<TextMeshProUGUI>();


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
