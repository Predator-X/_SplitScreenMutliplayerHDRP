//this script is used to disactivede gameobjects 
//manly heads bodys and guns from enemys that have been detached on head shot effect
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisActivateAfter : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(DeadDellay(this.gameObject));
    }
 

    public IEnumerator DeadDellay(GameObject g)
    {
        

        yield return new WaitForSeconds(4);
        Destroy(g);
        Destroy(gameObject);
    }
}
