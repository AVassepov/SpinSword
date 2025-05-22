using UnityEngine;

public class Player : Character
{
     private Rigidbody2D rb2d;

    public float SprintSpeed = 6f;
    public float Stamina = 0; 

    private float maxStamina = 100;
    
    
    private bool sprinting = false;


    private float defaultSpeed;


    private Weapon weaponOnGround;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Stamina = maxStamina;
        defaultSpeed = MovementSpeed;
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
        
        
        healthbarInstance.transform.position = gameObject.transform.position;
        
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
            rb2d.AddForce(moveDirection * MovementSpeed , ForceMode2D.Force);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out WeaponUI UI))
        {
            if(other.GetComponent<Weapon>() != CurrentWeapon){
                UI.Setup();
                weaponOnGround = UI.GetComponent<Weapon>();
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out WeaponUI UI))
        {
            if (other.GetComponent<Weapon>() != CurrentWeapon)
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
        if (CurrentWeapon)
        {
            Vector2 weaponPosition = Vector2.Lerp( WeaponTargetTransform.position , WeaponAnchor.position , CurrentWeapon.MovementSpeed  );
            Quaternion weaponRotation = Quaternion.Lerp(WeaponTargetTransform.rotation, WeaponAnchor.rotation, CurrentWeapon.RotationSpeed );
            
            WeaponTargetTransform.transform.position = weaponPosition;
            WeaponTargetTransform.transform.rotation = weaponRotation;
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
            print("Tried to pickup");
            CurrentWeapon.Drop();
            CurrentWeapon = weaponOnGround;
            weaponOnGround.PickUp(WeaponAnchor);
            weaponOnGround = null;
        }
        
    }
    
    
}
