//This Script Is more GameManager Script it manages PauseMenu UI  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor; // original code to quit Unity player in editor
#endif

public class PauseMenu : MonoBehaviour
{

    //New Input System
    private InputAction playerUI;
    MultiplayerInput multiplayerInput;
    //For Multiplayer and new input system to able to find player when added 
    private InputActionAsset inputActionAsset;
    private InputActionMap playerActionMap;
   

    public static bool GameIsPaused = false;
    public static bool pauseTheGamePressed = false;
    public static bool playerHasDied = false;
    public static int numberOfPlayers;

    public GameObject pauseMenuUI;
    public Button loadLastCheckpointButton;
    GameObject playerHolder;

    GameObject[] onStartGameObjectsInScene;
    GameObject levelLoader;

    Scene scene;

    //path to save JsonFile
    [SerializeField] string filename;
    List<PlayerAchivments> listTosave = new List<PlayerAchivments>();//used for checkList to return

    //MainMenu
    public GameObject mainMenu;

    //PauseMenu
    public GameObject loadCheckpointButton;

    //Credits Panel
    public GameObject creditsPanel;

    //ScoreList
    ArrayList scoreListArrayList;

    //to Active CheckpointUI
    [SerializeField] private GameObject checkPointUI;

    private void Awake()
    {
        //For Multiplayer and new input system. so it will not mix with other local players input 
        // uiModule = activeAndDisActiveForXTimeGameObject().
     

        SetMainMenuOf();
        creditsPanel.gameObject.SetActive(false);
        
    }
    private void Start()
    {     
        GetEnemysFromScene();   
    }

  


   


  

    public void TaskOnClick()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader");
       
        if (levelLoader != null)
        {
            levelLoader = GameObject.FindGameObjectWithTag("LevelLoader");
            levelLoader.GetComponent<LoadLevel>().Load();
        }
        else if (levelLoader == null)
        {
            Debug.LogError("LevelLoader is Empty !! in ----PauseMenu");
        }
               
    }

    public void SetLoadLevel()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader");

    }
 

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) <---- this before new Input System also work done in PlayerController  if coming back you can delete stuff there  as well
        
         if(pauseTheGamePressed)
         {

             if (!GameIsPaused)
             {
                 Resume();
                 pauseTheGamePressed = false;

             }
             else if(GameIsPaused)
             {
                 Pause();

                 ////Check if The player Has Checkpoint and set Load last check point Button to correct state
                 string path = Application.persistentDataPath + "/" + SaveSystem.getUserName() + "player.save";
                 if (File.Exists(path))
                 {
                     loadLastCheckpointButton.gameObject.SetActive(true);
                     pauseTheGamePressed = false;

                 }
                 else if (!File.Exists(path))
                 {
                    loadLastCheckpointButton.gameObject.SetActive(false);
                     pauseTheGamePressed = false;

                 }

             }
         }
       // PauseMenuHandler();
    }


   public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
       
    }

  public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
     //   GameIsPaused = true;
        if (playerHasDied) { PlayerHasDied(); }
        else if (!playerHasDied)
        {
            PlayerHasNotDied();
        }
           
    }

    public void PlayerHasDied()
    {
        pauseMenuUI.gameObject.transform.Find("ResumeButton").gameObject.SetActive(false);
        pauseMenuUI.gameObject.transform.Find("SaveButton").gameObject.SetActive(false);
        pauseMenuUI.gameObject.transform.Find("OnDied").gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    void PlayerHasNotDied()
    {
     
        pauseMenuUI.gameObject.transform.Find("ResumeButton").gameObject.SetActive(true);
        pauseMenuUI.gameObject.transform.Find("SaveButton").gameObject.SetActive(true);
        pauseMenuUI.gameObject.transform.Find("OnDied").gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void EndGamePause()
    {
        
        GameIsPaused = true;
        SetCreditsPanelOnForseconds();   
    }

    public void SetMainMenuON()
    {
        mainMenu.SetActive(true);
    }

    public void SetMainMenuOf()
    {
        mainMenu.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player in build version
#endif
    }

    public void SetCreditsPanelOnForseconds()
    {
        StartCoroutine(Credits());
    }
  
    IEnumerator Credits()
    {
        yield return new WaitForSeconds(1);
        creditsPanel.SetActive(true);

        yield return new WaitForSeconds(10f);
        creditsPanel.SetActive(false);
        
    }

    public virtual void SavePlayer()
    {
        

        scene = SceneManager.GetActiveScene();
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();     
        
        float sumTotalSocre = player.GetTime() / player.GetScore() * 100;
        PlayerAchivments thisPlayer = new PlayerAchivments(SaveSystem.getUserName(), player.GetScore(), player.GetTime(), sumTotalSocre);


        //if score does not exists or is new highest then save it to scoreList under the name withiut creating duplicates
        CheckScoresList(thisPlayer);
        SaveSystem.justCreatedNewAccount = false;
        SaveSystem.buttonHolder.SetActive(true);

        SaveSystem.SavePlayer(player,scene.buildIndex);
        SaveEnemys();
    }

    //Check for duplicates or HigherScore
    public void CheckScoresList(PlayerAchivments thisPlayer)
    {
        Debug.Log("------------CHECK SCORES LIST is active---------------NAME: "+thisPlayer.Name);
        List<PlayerAchivments> pA = new List<PlayerAchivments>();
        pA = JsonHelper.ReadListFromJSON<PlayerAchivments>(filename);
        string name = SaveSystem.getUserName();
        bool STOP = false;
        for (int i = 0; i < pA.Count; i++)
        {
            
            if (pA[i].Name.Equals(name))
            {
                if (( pA[i].Score < thisPlayer.Score))
                {
                    pA.RemoveAt(i);
                    pA.Insert(i, thisPlayer);
                    

                    listTosave = pA;
                    SaveHighScores(pA);
                    pA = JsonHelper.ReadListFromJSON<PlayerAchivments>(filename);
                 //   i = pA.Count;
                    Debug.Log("------------CHECK SCORES LIST is active---------------" + "Player Score Removed and added at List position :" + i);
                    
                }
                if ( pA[i].Score > thisPlayer.Score)
                {
                    pA.RemoveAt(i);
                    listTosave = pA;
                    pA = JsonHelper.ReadListFromJSON<PlayerAchivments>(filename);
                    SaveHighScores(pA);
                    Debug.Log("------------CHECK SCORES LIST is active---------------" + "Old Players Score Removed that was founded with lower score at position :" + i);

                    
                }

            }
          
        }
    }

    private void SaveHighScores(List<PlayerAchivments> scoreList)
    {
        JsonHelper.SaveToJSON<PlayerAchivments>(scoreList, filename);
    }

    public void AddHighScoreIfPossible(PlayerAchivments playerAchivments, List<PlayerAchivments> scoreList)
    {
        int maxCount = 7;
        for (int i = 0; i < maxCount; i++)
        {
            if (i >= scoreList.Count || playerAchivments.Score > scoreList[i].Score)
            {
                //add new high score
                scoreList.Insert(i, playerAchivments);

                while (scoreList.Count > maxCount)
                {
                    scoreList.RemoveAt(maxCount);
                }

                SaveHighScores(scoreList);

                break; // Break as no point to go further as the scores will be lower
            }
        }
    }

    public virtual void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        playerHolder = GameObject.FindGameObjectWithTag("Player");
        playerHolder.GetComponent<PlayerController>().currentHealth = data.health;
        playerHolder.GetComponent<PlayerController>().score = data.score;
        playerHolder.GetComponent<PlayerController>().currentTime = data.currntTime;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        playerHolder.transform.position = position;

        LoadEnemys();

       
        //This Is porbably not most efficient way of seting rotation but I do not have time as I wont to finish this license as quick as possible as I have to work in other job 
        Vector3 rotation;
        rotation.x = data.rotation[0];
        rotation.y = data.rotation[1];
        rotation.z = data.rotation[2];
        playerHolder.transform.rotation = Quaternion.Euler(rotation);
    }


    public virtual void GetEnemysFromScene()
    {
        int a = 0;
        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            a++;
            if (obj.tag == "Enemy")
            {
                onStartGameObjectsInScene[a] = obj;
            }


        }
    }


    public virtual void SaveEnemys()
    {
       

        GameObject[] enemysInScene = SaveSystem.getEnemysOnStart; 
     
        if (enemysInScene.Length == 0)
        {
            Debug.Log("There was no enemys to save ----- PauseMenu c# ");

        }
        else
        {
            for (int i = 0; i != enemysInScene.Length; i++)
            { 
                SaveSystem.SaveEnemys(enemysInScene[i].GetComponent<Enemy>(),i);
            }
        }
    }

    public virtual void LoadEnemys()
    {
        GameObject[] enemysInScene = onStartGameObjectsInScene; 

        int enemysLeft = enemysInScene.Length;

        Vector3 position;
        // Enemy[] enemysInSceneToSave;
        if (enemysInScene.Length == 0)
        {
            Debug.Log("There was no enemys to save ----- PauseMenu c# ");

        }
        else
        {
            for (int i = 0; i != enemysInScene.Length; i++)
            {
                 enemysInScene[i].GetComponent<Enemy>().currentHealth =  SaveSystem.LoadEnemys(i).heath;
                 enemysInScene[i].SetActive(SaveSystem.LoadEnemys(i).isItActive);
                
                position.x = SaveSystem.LoadEnemys(i).position[0];
                position.y = SaveSystem.LoadEnemys(i).position[1];
                position.z = SaveSystem.LoadEnemys(i).position[2];

                enemysInScene[i].transform.position = position;

            }
        }
    }



    public GameObject GetCheckPointUI()
    {
        return checkPointUI;
    }
    public void CheckPointReachedt()
    {
        StartCoroutine(activeAndDisActiveForXTimeGameObject(checkPointUI, 3f));
    }

    IEnumerator activeAndDisActiveForXTimeGameObject(GameObject obj, float forTime)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(forTime);
        obj.SetActive(false);

    }



}





/* 
    public void SavePlayersScoreListDataToJSON(PlayerAchivments playerAchivments)//string playerName, int score, float time, float totalScore)
{
        scoreListArrayList = new ArrayList();

        List<PlayerAchivments> pList = new List<PlayerAchivments>()
        {
            playerAchivments
    };
       

        scoreListArrayList.Add(pList);
        PlayersScoreListData data = new PlayersScoreListData(scoreListArrayList);
        string json = JsonUtility.ToJson(data.ToString());
       
       File.WriteAllText("G:/__JSONtest/PlayersScoreListData.json", json);// Application.persistentDataPath + "/PlayersScoreListData.json", json);
        /* PlayerAchivments playerAchivments = new PlayerAchivments();

         playerAchivments.Name = playerName;
         playerAchivments.Score = score;
         playerAchivments.Time = time;
         playerAchivments.TotalScore = totalScore;

         */
//   PlayersScoreListData playersScoreListData = new PlayersScoreListData(playerAchivments);
// playersScoreListData.playersScoreArrayList.Add(playerAchivments);
//   playersScoreListData(playerAchivments);
//PlayersScoreListData.playersScoreArraList.AddRange(playerAchivments.ToArray())

// playersScoreListData.playersScoreArrayList.Add(playerAchivments);
//  string json = JsonUtility.ToJson(playersScoreListData);
//   File.WriteAllText("G:/__JSONtest/PlayersScoreListData.json", json);// Application.persistentDataPath + "/PlayersScoreListData.json", json);
/*
Debug.Log("G:/__JSONtest/PlayersScoreListData.json");
    }

*/







/*
   void PauseMenuHandler()
    {
        if (GameIsPaused)
        {
            Pause();

            ////Check if The player Has Checkpoint and set Load last check point Button to correct state
            string path = Application.persistentDataPath + "/" + SaveSystem.getUserName() + "player.save";
            if (File.Exists(path))
            {
                loadLastCheckpointButton.gameObject.SetActive(true);
                

            }
            else if (!File.Exists(path))
            {
                loadLastCheckpointButton.gameObject.SetActive(false);
               

            }
        }
        else if (!GameIsPaused)
        {
            Resume();
        }
    }*/