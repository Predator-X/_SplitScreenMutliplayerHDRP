//INHERITANCE - PlayerController Inherits From character class and POLYMORPHISM the move method
//This Class is used for manaching MainPlayer Character
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//For new InputSystem
using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Utilities;

public class PlayerController : Character
{
    public CharacterController characterController;  //<-- characterController that is build in unity 

    //Shooting
    ShootWithRaycast attack;
    private float nextFire;
    [SerializeField]
    Camera cam;
    public GameObject aimCamera, fallowCamera;
    public int score;

    //Timer
   public float currentTime = 0;
    string text;

    //get all Enemys in scene for Saving
    GameObject[] onStartGameObjectsInScene;
  

    //Effect on camera
    Cinemachine.CinemachineImpulseSource impulseSource;


    //<<<>>> New Code for overriting move method 
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    //For Multiplayer
    private InputActionAsset inputActionAsset;
    private InputActionMap playerActionMap;

    //For new InputSystem
    public InputAction playerInput;
    private InputAction move, look,pause;

    [SerializeField]
    private InputAction fire;
    bool shootHasBeenPressed = false;

    
    MultiplayerInput multiplayerInput;

    Vector2 mInput;
    Rigidbody rb;

    void Awake()
    {
        inputActionAsset = this.GetComponent<PlayerInput>().actions;
        playerActionMap = inputActionAsset.FindActionMap("Player");
        multiplayerInput = new MultiplayerInput();

        PauseMenu.GameIsPaused = false;
       
    }

   
    // Start is called before the first frame update
    void Start()
    {
        CurrentSpeed = _speed;
       
        characterController.GetComponent<CharacterController>();
        attack = this.GetComponent<ShootWithRaycast>();
        
       // cam = GameObject.FindGameObjectWithTag("CameraPlayer").GetComponent<Camera>();//<-------------Becouse Of Multiplayer

        onStartGameObjectsInScene = GameObject.FindGameObjectsWithTag("Enemy");

        rb = GetComponent<Rigidbody>();
        
       

    }



    // | For new InpurtSystem 
    // V
    private void OnEnable()
    {
        /*
        playerInput.Enable();
        move = multiplayerInput.Player.Move;
        look = multiplayerInput.Player.Look;

        //  fire = multiplayerInput.Player.Fire;
        //  fire = multiplayerInput.Player.Fire.performed += Fire;
        fire.action.performed += Fire;

        move.Enable();
        look.Enable();
        */
        // fire.Enable();
        //----------------------New | ------------------------------------
        //                          V

        playerActionMap.Enable();
        move = playerActionMap.FindAction("Move");
        look = playerActionMap.FindAction("Look");

        playerActionMap.FindAction("Fire").started += Fire;
        playerActionMap.FindAction("Aim").started += Aim;
        playerActionMap.FindAction("Aim").canceled += AimCanceled;

        playerActionMap.FindAction("PauseMenu").started += PauseTheGame;
        playerActionMap.FindAction("PauseMenu1").canceled += PauseTheGame;

        // fire.action.performed += Fire;

        move.Enable();
        look.Enable();

    }
    private void OnDisable()
    {
        playerInput.Disable();
        move.Disable();
        look.Disable();
       
        //----------------------New | ------------------------------------
        //                          V
        playerActionMap.FindAction("Fire").started -= Fire;
        playerActionMap.FindAction("Aim").started -= Aim;
        playerActionMap.FindAction("Aim").canceled -= AimCanceled;

       playerActionMap.FindAction("PauseMenu").canceled -= PauseTheGame;
    }


    // POLYMORPHISM
    protected override void Move(GameObject head, GameObject gun, GameObject body)  //<<<|>>> New Code added_________________________________________________________ to Move() method in character class by base.Move();
    {

        // Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // A
        // |
        // V
        //for New InputSystem -------------------------------------------
          Vector3 moveInput = move.ReadValue<Vector2>();
          Vector3 playerVelocity = new Vector3(moveInput.x * CurrentSpeed, 0 , moveInput.y * CurrentSpeed);

        bool isSpaceKeyHeld = multiplayerInput.Player.Space.ReadValue<float>() > 0.1f;  //---------------------------------
       // Debug.Log(isSpaceKeyHeld+"_________________________");
        


            if (characterController.isGrounded)    
        {
            velocity.y = - 1f;
           // bool isSpaceKeyHeld = multiplayerInput.Player.Space.ReadValue<float>() >0.1f; // old input system ---->// if (Input.GetKeyDown(KeyCode.Space))
          if(isSpaceKeyHeld)
            {
                velocity.y = jumpForce;
            }
        }
        else if(!isSpaceKeyHeld && !characterController.isGrounded)
        {
            velocity.y -= gravity * -2f * Time.deltaTime;   // <--to calculate gravity : y -= gravity * -2f * Time.deltatime; but calculate only on ground 
           // rb.AddForce(transform.up * gravity * -2f);
        }
        
        Vector3 moveVector = transform.TransformDirection(moveInput);
        rb.velocity = transform.TransformDirection(playerVelocity);  //<---- Becose of new Input System
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
        base.Move(head, gun, body);                   //<-- POLYMORPHISM adding code to Move() method that inherits from character class 


        Vector3 lookInput = look.ReadValue<Vector2>();
        body.transform.Rotate(Vector3.up * lookInput.x  * mouseSensityvityX);

        head.transform.Rotate(Vector3.left * lookInput.y * mouseSensityvityY);
        transform.Rotate(Vector3.up * lookInput.x * mouseSensityvityX);

        gun.transform.Rotate(Vector3.left * lookInput.y * mouseSensityvityY);

    }


    void Update()
    {

        if (!PauseMenu.GameIsPaused)
        {
            attackAndCameraManagmentOnInput();

        }
   
    }

    void FixedUpdate()
    {
        if (!PauseMenu.GameIsPaused)
        {
            Move(head, gun, body);

        }
      
    }



    void Fire(InputAction.CallbackContext ctx)
    {
       // shootHasBeenPressed = true;
        Debug.Log(ctx.action.name+ "Fire");

        attack.Shoot();
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        impulseSource.GenerateImpulse(cam.transform.forward);

    }

    void Aim(InputAction.CallbackContext ctx)
    {
        Debug.Log("Aim");
        if (!ctx.canceled &&fallowCamera.activeInHierarchy)
        {
            fallowCamera.SetActive(false);
            aimCamera.SetActive(true);
        }
        if (ctx.canceled && aimCamera.activeInHierarchy)
        {
            aimCamera.SetActive(false);
            fallowCamera.SetActive(true);

        }

    }
    void AimCanceled (InputAction.CallbackContext ctx)
    {
        Debug.Log("AimCanceled");
        
        if ( aimCamera.activeInHierarchy)
        {
            aimCamera.SetActive(false);
            fallowCamera.SetActive(true);

        }

    }
    
    void PauseTheGame(InputAction.CallbackContext ctx)
    {
        Debug.Log("1GameIsPaused: " + PauseMenu.GameIsPaused + " pauseTheGamePressed: " + PauseMenu.pauseTheGamePressed);

        // Debug.Log(ctx.ReadValueAsButton()+ "Duration: "+ ctx.duration);
        if (PauseMenu.GameIsPaused && !ctx.ReadValueAsButton()) //&& !PauseMenu.pauseTheGamePressed
        {
            // StartCoroutine( SendMessageToUnPause());
            PauseMenu.pauseTheGamePressed = true ;
            PauseMenu.GameIsPaused = false;
            Debug.Log("check false");
        }

        else if (!PauseMenu.GameIsPaused  && !ctx.ReadValueAsButton())//&& !ctx.ReadValueAsButton())//if(ctx.phase == InputActionPhase.Performed && !PauseMenu.pauseTheGamePressed)/
        {
            // SendMessageToPause();
            //  StartCoroutine(SendMessageToPause());
            PauseMenu.pauseTheGamePressed = true;
            PauseMenu.GameIsPaused = true;
            Debug.Log("check ture");
        }
      
            Debug.Log("2GameIsPaused: " + PauseMenu.GameIsPaused + " pauseTheGamePressed: " + PauseMenu.pauseTheGamePressed);
    }


    /*

   // void SendMessageToPause()
   IEnumerator SendMessageToPause()
    {
       
       
        yield return new WaitForSeconds(0.5f);
        PauseMenu.pauseTheGamePressed = true;
        Debug.Log("check ture");
    }

    IEnumerator SendMessageToUnPause()
    {
        
        yield return new WaitForSeconds(0.5f);
        PauseMenu.pauseTheGamePressed = false;
        Debug.Log("check false");
    }
    */
    // | ABSTRACTION
    //  V
    private void attackAndCameraManagmentOnInput()
    {

        // if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        /*
         if(fire.action.OnStateEnter())
         {
             attack.Shoot();
             impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
             impulseSource.GenerateImpulse(cam.transform.forward);
         }

         if (Input.GetButtonDown("Fire2") && fallowCamera.activeInHierarchy)
         {
             fallowCamera.SetActive(false);
             aimCamera.SetActive(true);
         }
        
        if (Input.GetButtonUp("Fire2") && aimCamera.activeInHierarchy)
         {
             aimCamera.SetActive(false);
             fallowCamera.SetActive(true);

         }
       
        //shift to speed up(run)
        if (Input.GetButtonDown("SpeedUp"))
        {
            CurrentSpeed = runSpeed;
        }
        if (Input.GetButtonUp("SpeedUp"))
        {
            CurrentSpeed = _speed;
        }
         */
    }

    public string updateTimer()
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

       return string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void SetScore(int scoreset)
    {
        score = scoreset;
    }

    public void AddScore(int sc)
    {
        score += sc;
    }

 
    public int GetScore()
    {
        return score;
    }

    public void SetTime(float timeset)
    {
        currentTime = timeset;
    }
    public float GetTime()
    {
        return currentTime;
    }
}