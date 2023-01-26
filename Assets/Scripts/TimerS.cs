//This Class updates UI Timer by finding PlayerController and geting Time From there   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerS : MonoBehaviour
{
    public GameObject player;
    private void Awake()
    {

        // player = GameObject.FindGameObjectWithTag("Player");
        player = this.gameObject.transform.parent.gameObject.transform.parent.gameObject;
      
    }

    void LateUpdate()
    {
        if (player.GetComponent<PlayerController>().isDead == false && PauseMenu.GameIsPaused==false)
        {
            GetComponent<TextMeshProUGUI>().SetText(player.GetComponent<PlayerController>().updateTimer());
        }

      


    }
}
