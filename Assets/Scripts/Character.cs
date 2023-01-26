//INHERITANCE-Character is a Parent class PlayerrController and enemy cs Inherits from this class 
// ENCAPSULATION - Of players CurrentSpeed 
//contains health,take damage , heal, move and dead dellay 
// sets the basics for (inheritance) characters controller like playerController, enemy 
//>> Here you can set after what time dead dellay (game object gets destroyed from scene
// Move() method only using mouse to rotate as it might be inherited for other characters like tank 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class Character : MonoBehaviour
{
    // speed is a holder for standard players speed         
    //Current speed is total speed so if player holds...
    //..the run button the run speed adds to it 
    // Run speed is the holder for runing speed                // | Setting mouse sensityfity multiplication    |
    //to understand it better check it in PlayerController cs //  v                                             v
    public float _speed = 7f, runSpeed = 15f, mouseSensityvityX = 10f, mouseSensityvityY = 10f;
    public GameObject head, gun, body;

                   // ENCAPSULATION |
                   //               v
    [SerializeField] private float _currentSpeed { get; set; }

   public float CurrentSpeed
    {
        get { return _currentSpeed; }
        set
        {
            if (value < 0.0f)
            {
                Debug.LogError("You cannot set as standart speed of the player to negative! ");
            }
            else
            {
                _currentSpeed = value;
            }
        }
    }


    public bool isDead = false;

    //health
    public float currentHealth =10;

 
    public float jumpForce;


    private void Awake()
    {
        CurrentSpeed = _speed;

    }



    public virtual void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
       // Debug.Log("Name: " + gameObject.name + " HasLife: " + currentHealth);

        if (currentHealth <= 0 && !this.gameObject.CompareTag("Player"))
        {

            isDead = true;
            gameObject.SetActive(false);


        }
        else if (currentHealth <= 0 && this.gameObject.CompareTag("Player"))
        {
            isDead = true;


            //  GameObject.FindGameObjectWithTag("GameManager").gameObject.GetComponent<PauseMenu>().Pause();
            PauseMenu.pauseTheGamePressed = true;
            PauseMenu.GameIsPaused = true;
            PauseMenu.playerHasDied = true;
            gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
            gameObject.SetActive(false);

            List<GameObject> listEnemys = new List<GameObject>();
            GameObject[] EnemysArray = GameObject.FindGameObjectsWithTag("Enemy");

            for (int i = 0; i < EnemysArray.Length; i++)
            {
                EnemysArray[i].GetComponent<Enemy>().ResetAttack();
                EnemysArray[i].GetComponent<Enemy>().Patrolling();
                EnemysArray[i].GetComponent<Enemy>().CheckForSightAndAttackRange();
            }

            // gameObject.GetComponent<PlayerInput>().enabled = false;


        }
        else if (currentHealth > 0 && this.gameObject.CompareTag("Player"))
        {
            PauseMenu.playerHasDied = false;
        }


    }

   

  
// | ABSTRACTION
// V
  protected virtual void Move(GameObject head ,GameObject gun, GameObject body)             //<-- here only rotate as it might be used for tank or other character
    {
        //body.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensityvityX);   //etc..
        //head.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * mouseSensityvityY);
        //transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensityvityX);
        //gun.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * mouseSensityvityY);

        // | //For new Input system
        // V
       // Vector3 moveInput = move.ReadValue<Vector2>();
       /* 
        body.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensityvityX);                         
                                                                                    
        head.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * mouseSensityvityY);         
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensityvityX);
                                                                                    
        gun.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * mouseSensityvityY);
       */
    }

 

    public virtual void Heal(float healAmount)
    {
        currentHealth += healAmount; 
    }

   public  IEnumerator DeadDellay(GameObject g)
    {
        

        yield return new WaitForSeconds(4);
        Destroy(g);
        Destroy(gameObject);
    }


    public void SetHealth(float healthset)
    {
        currentHealth = healthset;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
}
