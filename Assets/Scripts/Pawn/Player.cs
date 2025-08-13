using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Character
{
     private Rigidbody2D rb2d;

    public float SprintSpeed = 6f;
    public float Stamina = 0; 

    private float maxStamina = 100;
    
    
    private bool sprinting = false;


    private float defaultSpeed;


    public List<Weapon> EquippedWeapons = new List<Weapon>();

    private int MaximumWeaponSlots = 3;



    private Weapon weaponOnGround;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Stamina = maxStamina;
        defaultSpeed = MovementSpeed;


        for (int i = 0; i < EquippedWeapons.Count; i++)
        {
            EquippedWeapons[i].ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
        }
    }


    public void FixedUpdate()
    { 
       //Weapon movement and states 
        MouseUpdate();
      
        // Movement
        Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        
         // Sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprinting = true;
        }else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprinting = false;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Interact();
        }

        int number = GetPressedNumber();

        if (number > 0) {
            SwapWeapon(number-1);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
          int newWeapon = EquippedWeapons.IndexOf(WeaponElements.CurrentWeapon) + (int)Input.GetAxis("Mouse ScrollWheel");


            if (newWeapon < 0)
            {
                newWeapon = EquippedWeapons.Count;
            }
            else if (newWeapon > EquippedWeapons.Count) 
            {
                newWeapon = 0;    
            }

                SwapWeapon(newWeapon);
        }



        healthbarInstance.transform.position = gameObject.transform.position;
        
    }

    public int GetPressedNumber()
    {
        for (int number = 0; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }

        return -1;
    }

    private void Move(Vector2 moveDirection)
    {

        if (sprinting && Stamina > 0   && (moveDirection.x != 0 || moveDirection.y != 0 ))
        {
            Stamina--;
            MovementSpeed = SprintSpeed;
        }
        else if(!sprinting)
        {
            MovementSpeed = defaultSpeed;
            Stamina++;

            if (Stamina > maxStamina)
            {
                Stamina = maxStamina;
            }
        }else if (sprinting && Stamina == 0)
        {
            MovementSpeed = defaultSpeed;
        }



          // rb2d.linearVelocity = moveDirection * MovementSpeed;
            rb2d.AddForce(moveDirection * (MovementSpeed + BonusEffect.MovementSpeed) , ForceMode2D.Force);
    }

    private void SwapWeapon(int weaponIndex)
    {
        WeaponElements.CurrentWeapon.ClearAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
        WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileUnequiped);
        WeaponElements.CurrentWeapon.gameObject.SetActive(false);
        WeaponElements.CurrentWeapon = EquippedWeapons[weaponIndex];
        WeaponElements.CurrentWeapon.gameObject.SetActive(true);
        WeaponElements.CurrentWeapon.transform.position = WeaponElements.WeaponAnchor.transform.position;
        WeaponElements.CurrentWeapon.Target = WeaponElements.WeaponTargetTransform;
        WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
        WeaponElements.CurrentWeapon.ClearAllEffects(PassiveEffect.ActivationCondition.WhileUnequiped);

        WeaponElements.CurrentWeapon.ActivateAllEffects( PassiveEffect.ActivationCondition.Constant);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out WeaponUI UI))
        {
            if(other.GetComponent<Weapon>() != WeaponElements.CurrentWeapon){
                UI.Setup();
                weaponOnGround = UI.GetComponent<Weapon>();
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out WeaponUI UI))
        {
            if (other.GetComponent<Weapon>() != WeaponElements.CurrentWeapon)
            {
                UI.CloseUI();
                weaponOnGround = null;
            }
        }
    }
    
    
    private void MouseUpdate()
    {
          
        // Make the weapon follow the anchor
        /*CurrentWeapon.GetComponent<Rigidbody2D>().MovePosition(WeaponAnchor.position);
        CurrentWeapon.GetComponent<Rigidbody2D>().MoveRotation(WeaponAnchor.rotation);*/
        if (WeaponElements.CurrentWeapon)
        {
            Vector2 weaponPosition = Vector2.Lerp(WeaponElements.WeaponTargetTransform.position , WeaponElements.WeaponAnchor.position , WeaponElements.CurrentWeapon.MovementSpeed  );
            Quaternion weaponRotation = Quaternion.Lerp(WeaponElements.WeaponTargetTransform.rotation, WeaponElements.WeaponAnchor.rotation, WeaponElements.CurrentWeapon.RotationSpeed );

            WeaponElements.WeaponTargetTransform.transform.position = weaponPosition;
            WeaponElements.WeaponTargetTransform.transform.rotation = weaponRotation;
        }
        

        
        // make the anchor point at mouse
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2) transform.position).normalized;
        transform.up = direction;
    }

    void Interact()
    {

        if (weaponOnGround)
        {

            if (MaximumWeaponSlots > EquippedWeapons.Count)
            {
                weaponOnGround.PickUp(WeaponElements.WeaponAnchor);
                EquippedWeapons.Add(weaponOnGround);
                SwapWeapon(EquippedWeapons.IndexOf(weaponOnGround));
                weaponOnGround = null;
            }
            else
            {
                print("Tried to pickup");
                WeaponElements.CurrentWeapon.ClearAllEffects( PassiveEffect.ActivationCondition.WhileEquiped);
                WeaponElements.CurrentWeapon.ClearAllEffects( PassiveEffect.ActivationCondition.Constant);
                WeaponElements.CurrentWeapon.Drop();
                WeaponElements.CurrentWeapon = weaponOnGround;
                weaponOnGround.PickUp(WeaponElements.WeaponAnchor);
                weaponOnGround = null;
                WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
                WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);


            }
        }
        
    }
    
    
}
