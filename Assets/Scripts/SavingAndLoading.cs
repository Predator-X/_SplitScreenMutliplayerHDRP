//This Script is manly used for Loading next Scenes and seting up whats necessary for certain scene like Main Menu Scene
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem;
using TMPro;
//using TMPro;
public class SavingAndLoading : MonoBehaviour
{
     GameObject[] onStartGameObjectsInScene;
    GameObject playerHolder;

    Scene scene;
    bool sceneIsLoaded = false;

    public static SavingAndLoading Instance;

   
    public GameObject loadingSceen;
    public Slider slider;
    public Text progressText;
    bool loadDone = false , isLoadingMenu=false , isLoading =false,isSceneFromSaveOrAreadyPlayed=false, isLoadingNextLevel=false;

  

    //If this scene is main menu and player has no save/ checkpoint, has not played the game, disactivate counitue button
    GameObject mainMenuContinueButton;


    //Converting to multiplayer------------------------
    GameObject mainPlayerManekin, mainPlayerVariant, playerInputManager;
   
    [SerializeField]
   public GameObject numberOfPlayersText;


    //----------------------------------------------------

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
          
            return;
        }
       
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        onStartGameObjectsInScene = GameObject.FindGameObjectsWithTag("Enemy");

        //If this scene is main menu and player has no save/ checkpoint, has not played the game, disactivate counitue button
        CheckIfIsMainManuAndSetUI();


        //Converting to multiplayer------------------------
        numberOfPlayersText.SetActive(false);
     
        //----------------------------------------------------
    }


    //If this scene is main menu and player has no save/ checkpoint, has not played the game, disactivate counitue button
    public void CheckIfIsMainManuAndSetUI()
    {

        Scene thisScene = SceneManager.GetActiveScene();
        if (thisScene.buildIndex == 1)
        {
            string path = Application.persistentDataPath + "/" + SaveSystem.getUserName() + "player.save";
          

            if (!File.Exists(path))
            {
                SaveSystem.buttonHolder = GameObject.FindGameObjectWithTag("ContinueButton");
                Button continueButton = SaveSystem.buttonHolder.GetComponent<Button>();
                continueButton.GetComponent<Image>().color = Color.clear;
                continueButton.interactable = false;
            }
            else if (File.Exists(path))
            {

                ContinueMainMenuUISetUp();
            }


            /*
            //Converting to multiplayer------------------------
            mainPlayerManekin = GameObject.Find("MainPlayer");
            mainPlayerVariant = GameObject.FindGameObjectWithTag("Player");
            playerInputManager = GameObject.FindGameObjectWithTag("PlayerInputManager");
            mainPlayerManekin.gameObject.SetActive(true);
            mainPlayerVariant.gameObject.SetActive(false);
            playerInputManager.gameObject.SetActive(true);
            numberOfPlayersText = GameObject.FindGameObjectWithTag("NumberOfPlayersCounterText");
            numberOfPlayersText.SetActive(false);
            //----------------------------------------------------
            */
        }
    }

    public void ContinueMainMenuUISetUp()
    {
        string path = Application.persistentDataPath + "/" + SaveSystem.getUserName() + "player.save";
        if (File.Exists(path))
        {
            SaveSystem.buttonHolder = GameObject.FindGameObjectWithTag("ContinueButton");
            Button continueButton = SaveSystem.buttonHolder.GetComponent<Button>();
            continueButton.GetComponent<Image>().color = Color.white;
            continueButton.interactable = true;

            //Swaping buttons postions when player has played the game and has save so it will look more apropiate
            GameObject startButtonGameObject = GameObject.FindGameObjectWithTag("StartButton").gameObject;//this.gameObject.transform.Find("StartButton").gameObject;
            RectTransform startButton = startButtonGameObject.GetComponent<RectTransform>();
            startButtonGameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Start New Campain";


            Vector3 continueButtonPositionHolder;
            continueButtonPositionHolder = startButton.position;
            startButton.position = SaveSystem.buttonHolder.GetComponent<RectTransform>().position;
            SaveSystem.buttonHolder.GetComponent<RectTransform>().position = continueButtonPositionHolder;
            SaveSystem.buttonHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Continue Campain";
        }
        else if (!File.Exists(path)) {
            Debug.LogWarning("SavingAndLoadinScrpit SaveFilePathNot exists" +
            " possible miss use of ContinueMainMenuUISetUp method or something else");
        }
        
    }


    public void FindEnemys()
    {
            onStartGameObjectsInScene = GameObject.FindGameObjectsWithTag("Enemy");   
    }
    public virtual void SavePlayer()
    {
        scene = SceneManager.GetActiveScene();
        SaveSystem.SavePlayer(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(),  scene.buildIndex);
        SaveEnemys();
    }

  
  
    public virtual void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        
            playerHolder = GameObject.FindGameObjectWithTag("Player");
            playerHolder.GetComponent<PlayerController>().SetHealth(data.health);
            playerHolder.GetComponent<PlayerController>().SetScore( data.score);
            playerHolder.GetComponent<PlayerController>().SetTime(data.currntTime);

            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];
            playerHolder.transform.position = position;
     

        Debug.Log("Player Loaded Position: " + position);

            LoadEnemys();


        //This Is porbably not most efficient way of seting rotation but I do not have time as I wont to finish this license as quick as possible as I have to work in other job 
        Vector3 rotation;
        rotation.x = data.rotation[0];
        rotation.y = data.rotation[1];
        rotation.z = data.rotation[2];

        playerHolder.transform.rotation = Quaternion.Euler(rotation);
    }

    public virtual void SaveEnemys()
    {
        GameObject[] enemysInScene = onStartGameObjectsInScene; //GameObject.FindGameObjectsWithTag("Enemy");
        int enemysLeft = enemysInScene.Length;

       
        if (enemysInScene.Length == 0)
        {
            Debug.Log("There was no enemys to save ----- PauseMenu c# ");

        }
        else
        {
            for (int i = 0; i != enemysInScene.Length; i++)
            {
                
                SaveSystem.SaveEnemys(enemysInScene[i].GetComponent<Enemy>(), i);
            }
        }
    }

    public virtual void LoadEnemys()
    {
        GameObject[] enemysInScene = onStartGameObjectsInScene; //GameObject.FindGameObjectsWithTag("Enemy");

        int enemysLeft = enemysInScene.Length;

        Vector3 position;
       
        if (enemysInScene.Length <= 0)
        {
            Debug.Log("There was no enemys to save ----- PauseMenu c# ");

        }
        else
        {
            for (int i = 0; i != enemysInScene.Length; i++)
            {
                enemysInScene[i].GetComponent<Enemy>().currentHealth = SaveSystem.LoadEnemys(i).heath;
                enemysInScene[i].SetActive(SaveSystem.LoadEnemys(i).isItActive);
               
                position.x = SaveSystem.LoadEnemys(i).position[0];
                position.y = SaveSystem.LoadEnemys(i).position[1];
                position.z = SaveSystem.LoadEnemys(i).position[2];

                enemysInScene[i].transform.position = position;

            }
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 

    public void Load()
    {
       
     
        loadDone = false;
        loadingSceen.SetActive(true);
       
        isLoadingMenu = false;
        isLoadingNextLevel = false;
        isSceneFromSaveOrAreadyPlayed = true;

     

        StartCoroutine(LoadAsynchronously(SaveSystem.LoadPlayer().sceneIndexx));
        PauseMenu.pauseTheGamePressed = false;
        PauseMenu.GameIsPaused = false;
    }

    public void StartNewGame()
    {
        loadingSceen.SetActive(true);
        loadDone = false;
        isLoadingNextLevel = false;

        isLoadingMenu = false;
        isSceneFromSaveOrAreadyPlayed = false;

        Scene scene = SceneManager.GetActiveScene();             
        StartCoroutine(LoadAsynchronously(scene.buildIndex + 1));

    }


    //Converting to multiplayer------------------------
    public void NewMulitplayerCampain()
    {
        mainPlayerManekin = GameObject.Find("MainPlayer");
        mainPlayerVariant = GameObject.FindGameObjectWithTag("Player");
        playerInputManager = GameObject.FindGameObjectWithTag("PlayerInputManager");
       
       
       // mainPlayerVariant.gameObject.SetActive(true);
        playerInputManager.gameObject.SetActive(true);
        PlayerInputManager p = new PlayerInputManager();
        p= playerInputManager.gameObject.GetComponent<PlayerInputManager>();
        p.enabled = true;

        // numberOfPlayersText = GameObject.FindGameObjectWithTag("NumberOfPlayersCounterText");
  
            numberOfPlayersText.SetActive(true);

        if (mainPlayerManekin != null)
        {
            mainPlayerManekin.gameObject.SetActive(false);
        }
    }
    //-----------------------------------------------------


    public void LoadLastSave()
    {
        isLoadingMenu = false;
        isSceneFromSaveOrAreadyPlayed = true;
       
    }

    //LoadsNextSceneFromCurrentWone
    public void LoadNextScene()
    {
        loadingSceen.SetActive(true);
        loadDone = false;

        Scene scene = SceneManager.GetActiveScene();

        isLoadingNextLevel = true;

        isLoadingMenu = false;
        isSceneFromSaveOrAreadyPlayed = false;
        StartCoroutine(LoadAsynchronously(scene.buildIndex + 1));
    }

    public void LoadSpecificScene(int sceneIndex,bool isSceneFromSave)
    {
        loadingSceen.SetActive(true);
        loadDone = false;
        
        isSceneFromSaveOrAreadyPlayed = isSceneFromSave;
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

  
    public void LoadMenu()
    {
        
        loadingSceen.SetActive(true);
        loadDone = false;
        isLoadingMenu = true;

        isLoadingNextLevel = false;
        isSceneFromSaveOrAreadyPlayed = false;
        StartCoroutine(LoadAsynchronously(1));
    }

 


    IEnumerator LoadAsynchronously(int sceneIdnex)
    {
       
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIdnex);


       
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
        if (operation.isDone)
        {
          
            if (!isLoadingMenu && !isLoadingNextLevel && isSceneFromSaveOrAreadyPlayed)
            {
                yield return new WaitForSeconds(1f);
                this.GetComponent<SavingAndLoading>().FindEnemys();
                this.GetComponent<SavingAndLoading>().LoadPlayer();
                loadingSceen.SetActive(false);

            }
            if(!isSceneFromSaveOrAreadyPlayed && !isLoadingMenu && isLoadingNextLevel)
            {
                PlayerController playerC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                PlayerData playerdata = SaveSystem.LoadPlayer();
                playerC.SetHealth(playerdata.health); playerC.SetTime(playerdata.currntTime); playerC.SetScore(playerdata.score);
            }
            if (!isSceneFromSaveOrAreadyPlayed && !isLoadingMenu && !isLoadingNextLevel)
            {
               if(GameObject.FindGameObjectWithTag("Player").gameObject ==null || GameObject.FindGameObjectWithTag("PlayerSpawnPoint").gameObject == null)
                {
                    Debug.LogError("Cannot find Player or PlayersSpawnPointOnSceneLoad ----SaveAndLoading c#/// Player.GameObject=" + GameObject.FindGameObjectWithTag("Player").gameObject.name +
                        "   ---  PlayerSpawnPoint = " + GameObject.FindGameObjectWithTag("PlayerSpawnPoint").gameObject.name);
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform.position;
                }
               
            }
            if (isLoadingMenu)
            {

                CheckIfIsMainManuAndSetUI();
                numberOfPlayersText = GameObject.FindGameObjectWithTag("NumberOfPlayersCounterText");
                this.GetComponent<PauseMenu>().SetMainMenuON();

            }


            loadingSceen.SetActive(false);
    
            Time.timeScale = 1.0f;
            

        }

    }
}