//This script manages to set and sort data and ui for highscore list 
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    List<PlayerAchivments> scoreList = new List<PlayerAchivments>();
    List<GameObject> uiElements = new List<GameObject>();//<- uiElements is uiScore text(name : Score) in mainMenuList

    [SerializeField] int maxCount = 3;//<- MaxCount is used for maximum people in high score list 
    [SerializeField] string filename;

    [SerializeField] GameObject uiTextPrefab;
    [SerializeField] Transform wrapperElement;

    bool itExists = false;
    
    private void Awake()
    {
       wrapperElement= GameObject.FindGameObjectWithTag("ScoreList").transform;
    }
    private void Start()
    {
     
  
        if (File.Exists(JsonHelper.GetPath(filename)))
        {
           
            LoadHighScores();

            for(int i=0; i< scoreList.Count; i++)//<- load saved users to a uiList(UI display) 
            {
                if (scoreList[i].Name == SaveSystem.getUserName())
                {
                    itExists = true;
                }
            }

            if (!itExists)//<- if its new player add him to the list
            {
                PlayerAchivments thisPlayer = new PlayerAchivments(SaveSystem.getUserName(), 0, 0, 0);

                scoreList.Add(thisPlayer);

                SaveHighScores();
                LoadHighScores();
            }
           
        }
        if (!File.Exists(JsonHelper.GetPath(filename)))
        {
            Debug.LogError("File path does not exists: " + JsonHelper.GetPath(filename));
           

        }

        ContinueButtonHandlerIfPlayerIsNewOrETC();//<- if its new user that does not have a save yeat disable continue button in MainMenu and viceVersa




        LoadHighScores();

        updateUI(scoreList);

    }

    public void ContinueButtonHandlerIfPlayerIsNewOrETC()//<- if its new user that does not have a save yeat disable continue button in MainMenu and viceVersa

    {

        if (SaveSystem.justCreatedNewAccount)//<- if its new user that does not have a save yeat disable continue button in MainMenu
        {
            SaveSystem.buttonHolder = GameObject.FindGameObjectWithTag("ContinueButton");
            Button continueButton = SaveSystem.buttonHolder.GetComponent<Button>();
            continueButton.GetComponent<Image>().color = Color.clear;
            continueButton.interactable = false;
            //  SaveSystem.buttonHolder.SetActive(false);
        }
        if (!SaveSystem.justCreatedNewAccount)
        {
            SaveSystem.buttonHolder = GameObject.FindGameObjectWithTag("ContinueButton");
            Button continueButton = SaveSystem.buttonHolder.GetComponent<Button>();
            continueButton.GetComponent<Image>().color = Color.white;
            continueButton.interactable = true;
        }

    }

    private void LoadHighScores()
    {
        scoreList = JsonHelper.ReadListFromJSON<PlayerAchivments>(filename);
        scoreList = scoreList.OrderByDescending(o => o.Score).ToList();//<- filter list to show the highest scores

        while (scoreList.Count > maxCount)
        {
            scoreList.RemoveAt(maxCount);
        }
    }

    private void updateUI(List<PlayerAchivments> list)
    {
        for (int i = 0; i<list.Count; i++)
        {
            PlayerAchivments pA = list[i];

            if (pA.Score > -1)
            {
                if (i >= uiElements.Count)
                {
                    //Instaniate new entry UI 
                    var inst = Instantiate(uiTextPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(wrapperElement,false);

                    uiElements.Add(inst);
                }
                var scoreTexts = uiElements[i].GetComponentsInChildren<TMP_Text>();
              //  Debug.Log("uiELEMENTS@@@@@@@ : " + uiElements[i].name);
               
                           
                scoreTexts[0].text = scoreList[i].Name;


                scoreTexts[1].text = "Score:" + scoreList[i].Score;



            }
        }
    }

    private void SaveHighScores()
    {
        JsonHelper.SaveToJSON<PlayerAchivments>(scoreList, filename);
    }

    public void AddHighScoreIfPossible(PlayerAchivments playerAchivments)
    {
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

                SaveHighScores();

                break; // Break as no point to go further as the scores will be lower
            }
        }
    }
}
