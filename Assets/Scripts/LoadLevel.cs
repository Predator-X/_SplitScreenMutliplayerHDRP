//This Script is Used for LoadingScreen UI when sceene is being loaded 

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public GameObject loadingSceen;
    public Slider slider;
    public Text progressText;
    PauseMenu pauseMenuScript;

    bool loadDone = false;

    Scene scene;

    public static LoadLevel Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>().SetLoadLevel();
            Instance = this;
            DontDestroyOnLoad(gameObject);  
    }
    private void Start()
    {
        loadingSceen = GameObject.FindGameObjectWithTag("LoadScreen");
        slider = GameObject.FindGameObjectWithTag("LoadSlider").GetComponent<Slider>();
        progressText = GameObject.FindGameObjectWithTag("LoadText").GetComponent<Text>();
        loadingSceen.SetActive(false);
    }

    public void Load()//(int sceneIndex)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>().SetLoadLevel();
        loadDone = false;
 
        StartCoroutine(LoadAsynchronously(SaveSystem.LoadPlayer().sceneIndexx));
    }




    public void LoadMenu()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>().SetLoadLevel();
        loadingSceen.active = true;
        loadDone = false;
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
             GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>().SetLoadLevel();
            
                loadingSceen = GameObject.FindGameObjectWithTag("LoadScreen");
                slider = GameObject.FindGameObjectWithTag("LoadSlider").GetComponent<Slider>();
                progressText = GameObject.FindGameObjectWithTag("LoadText").GetComponent<Text>();
                loadingSceen.SetActive(false);
                         
            this.GetComponent<SavingAndLoading>().FindEnemys();
            this.GetComponent<SavingAndLoading>().LoadPlayer();
            Time.timeScale = 1.0f;

        }
      
    }
}