using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerSpawnPointManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;
    private GameObject playerPrefabForLobby;


    //Multiplayer
    private int getNumberOfPlayersToSpawn;
    int NumberOfPlayers;

    private void Awake()
    {
        getNumberOfPlayersToSpawn = PauseMenu.numberOfPlayers;
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }
    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)//<--------If its In MainMenu 
        {
            playerPrefabForLobby = Resources.Load<GameObject>("MainPlayerVae1 Variant 1 ForPlayer2 Variant");
           // Debug.Log(playerPrefabForLobby.name + "AAAAAAAAAAAAAAA");

            playerInputManager.playerPrefab= playerPrefabForLobby;
            
        }

        if(SceneManager.GetActiveScene().buildIndex >1)
        {
           // GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            for(int i=1; i <= getNumberOfPlayersToSpawn; i++)
            {
                Debug.Log(i + " ___________________________");
                if (i == 1)
                {
                    
                    playerPrefabForLobby = Resources.Load<GameObject>("MainPlayerVae1 Variant 1");
                  GameObject holder=  Instantiate(playerPrefabForLobby);
                    holder.transform.rotation = startingPoints[0].rotation;
                    holder.transform.position = this.gameObject.transform.position;
                    
                   // Debug.Log("0!!!!!!!!!!!!!!!!!!!");
                    //Turn Of Scene Camera When Player Is Spawned
                    GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
                  //  playerInputManager.playerPrefab = playerPrefabForLobby;
                 //   playerInputManager.onPlayerJoined += AddPlayer;
                }
                else if (i == 2)
                {
                    playerPrefabForLobby = Resources.Load<GameObject>("MainPlayerVae1 Variant 1 ForPlayer2");
                    GameObject holder = Instantiate(playerPrefabForLobby);
                    holder.transform.rotation = startingPoints[0].rotation;
                    holder.transform.position = this.gameObject.transform.position;
                    // Instantiate(playerPrefabForLobby);
                    // playerInputManager.onPlayerJoined += AddPlayer;
                    Debug.Log("1!!!!!!!!!!!!!!!!!!!");
                }
                else if (i == 3)
                {
                   // playerPrefabForLobby = Resources.Load<GameObject>("MainPlayerVae1 Variant 1 ForPlayer2");
                   // Instantiate(playerPrefabForLobby);
                    //  playerInputManager.playerPrefab = playerPrefabForLobby;
                  //  Debug.Log("2!!!!!!!!!!!!!!!!!!!");
                  //  playerInputManager.onPlayerJoined += AddPlayer;
                }
                // AddPlayer(players[i]);
            }
        }
    }

    void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }


    public void AddPlayer(PlayerInput player)
    {
        PauseMenu.numberOfPlayers = playerInputManager.playerCount;
        if (SceneManager.GetActiveScene().buildIndex == 1)//<--------If its In MainMenu 
        {
            // playerInputManager.playerPrefab = playerPrefabForLobby;
            NumberOfPlayers = playerInputManager.playerCount;
            int i = playerInputManager.playerCount;//<---- As playerCount+1  will print 11
            i++;
            GameObject nOPtext;
            nOPtext = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SavingAndLoading>().numberOfPlayersText.gameObject;
            nOPtext.SetActive(true);
            nOPtext.GetComponent<TextMeshProUGUI>().
                SetText("Number Of Local Players: " + playerInputManager.playerCount);//i);
            Debug.Log(playerInputManager.playerCount + " s");

        }

        players.Add(player);

        //need to use the parent due to the structure of the prefab
        //Transform playerParent = player.transform.parent;
        //  playerParent.position = startingPoints[players.Count - 1].position;
        player.transform.rotation = startingPoints[0].rotation;
        player.transform.position = startingPoints[0].position;

        if(SceneManager.GetActiveScene().buildIndex != 1)
        {
            //convert layer mask (bit) to an intiger
            int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);
            //set the layer
            // playerParent.GetComponentInChildren<Camera>().gameObject.layer = layerToAdd; 
            player.GetComponentInChildren<Camera>().gameObject.layer = layerToAdd + 1;
            //add the layer
            //playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
            player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
            //set the action in the custom cinemachine Input Handler
            // playerParent.GetComponentInChildren<InputHandlerForCinemachine>().horizontal = player.actions.FindAction("Look");
            player.GetComponentInChildren<InputHandlerForCinemachine>().horizontal = player.actions.FindAction("Look");


           
        }

        Debug.Log(PauseMenu.numberOfPlayers + " ssssssssssssss");

    }
}
