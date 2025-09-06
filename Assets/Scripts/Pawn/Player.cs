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


    [SerializeField] private PauseMenu Pause;
    
    private bool sprinting = false;


    private float defaultSpeed;




    public List<Weapon> EquippedWeapons = new List<Weapon>();

    private int MaximumWeaponSlots = 3;

    private Vector2 moveDir;

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


    public override void UpdateHealth(float value, Vector3 direction)
    {
        Health += value;

        if (Health > MaxHealth + BonusEffect.MaxHP)
        {
            Health = MaxHealth + BonusEffect.MaxHP;
        }
        else if (Health <= 0)
        {
            Die();
        }


        TakeHit(direction);



    }
    public void Update()
    {
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));



       //Weapon movement and states 
        MouseUpdate();
        
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
            Interact(false);
        }

        int number = GetPressedNumber();

        if (number > 0 && number<= EquippedWeapons.Count && EquippedWeapons[number] !=null) {
            SwapWeapon(number-1);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            print("SCROLLING");
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

    private void FixedUpdate()
    {

        // Movement
        Move(moveDir);
    }

    public int GetPressedNumber()
    {
        for (int number = 0; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString())) { 
            return number;
            }
        }

        return -1;
    }

    private void Move(Vector2 moveDirection)
    {

        if (sprinting && Stamina > 0   && (moveDirection.x != 0 || moveDirection.y != 0 ))
        {
            Stamina--;
            MovementSpeed = SprintSpeed + BonusEffect.MovementSpeed;
        }
        else if(!sprinting)
        {
            MovementSpeed = defaultSpeed + BonusEffect.MovementSpeed;
            Stamina++;

            if (Stamina > maxStamina + BonusEffect.Stamina)
            {
                Stamina = maxStamina + BonusEffect.Stamina;
            }
        }else if (sprinting && Stamina == 0)
        {
            MovementSpeed = defaultSpeed + BonusEffect.MovementSpeed;
        }



          // rb2d.linearVelocity = moveDirection * MovementSpeed;
            rb2d.AddForce(moveDirection * (MovementSpeed + BonusEffect.MovementSpeed) , ForceMode2D.Force);
    }

    public void SwapWeapon(int weaponIndex)
    {

      // if (EquippedWeapons.Count>= weaponIndex && !EquippedWeapons[weaponIndex])
        //{
            WeaponElements.CurrentWeapon.ClearAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
            WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileUnequiped);
            WeaponElements.CurrentWeapon.gameObject.SetActive(false);
            WeaponElements.CurrentWeapon = EquippedWeapons[weaponIndex];
            WeaponElements.CurrentWeapon.gameObject.SetActive(true);
            WeaponElements.CurrentWeapon.transform.position = WeaponElements.WeaponAnchor.transform.position;
            WeaponElements.CurrentWeapon.Target = WeaponElements.WeaponTargetTransform;
            WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
            WeaponElements.CurrentWeapon.ClearAllEffects(PassiveEffect.ActivationCondition.WhileUnequiped);

            WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
        //}
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

    public void Interact(bool swap)
    {

        if (weaponOnGround)
        {

            if (MaximumWeaponSlots >= EquippedWeapons.Count)
            {
                weaponOnGround.PickUp(WeaponElements.WeaponAnchor);
                EquippedWeapons.Add(weaponOnGround);
                SwapWeapon(EquippedWeapons.IndexOf(weaponOnGround));
                weaponOnGround = null;
            }
            else if(swap)
            {
                EquippedWeapons[Pause.CurrentCardIndex] = weaponOnGround;
                print("Tried to pickup");
                WeaponElements.CurrentWeapon.ClearAllEffects( PassiveEffect.ActivationCondition.WhileEquiped);
                WeaponElements.CurrentWeapon.ClearAllEffects( PassiveEffect.ActivationCondition.Constant);
                WeaponElements.CurrentWeapon.Drop();
                WeaponElements.CurrentWeapon = weaponOnGround;
                weaponOnGround.PickUp(WeaponElements.WeaponAnchor);
                weaponOnGround = null;
                WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
                WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
                Pause.ToggleUI(false);

            }
            else
            {
                Pause.ToggleUI(true);
            }
        }
        
    }
    
    
}
