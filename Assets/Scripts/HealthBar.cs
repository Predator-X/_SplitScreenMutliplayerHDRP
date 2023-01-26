//Sets Health bar for player GUI by geting playerController and geting health data form  there
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Scrollbar healthBar;
    private GameObject player;
   

    private void Awake()
    {
        healthBar = this.GetComponent<Scrollbar>();
       // player = GameObject.FindGameObjectWithTag("Player");
        player = gameObject.transform.parent.gameObject.transform.parent.gameObject;
    }


    void LateUpdate()
    {
        healthBar.size = player.GetComponent<PlayerController>().currentHealth / 20;
        if (player.activeInHierarchy == false)
        {
            healthBar.interactable = false;
           
            healthBar.size = 1;
        }

    }
}
