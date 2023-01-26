//This Class updates UI Score By finding PlayerController and geting score form there 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreDisplay : MonoBehaviour
{
   public TextMeshPro scoreText;
   public  GameObject player;


    private void Awake()
    {

       // player = GameObject.FindGameObjectWithTag("Player");
       player = this.gameObject.transform.parent.gameObject.transform.parent.gameObject;

    }

    void LateUpdate()
    {

        GetComponent<TextMeshProUGUI>().SetText("Score:" + player.GetComponent<PlayerController>().GetScore());


    }
}


