//This Class Is setUp for being Parent Class For later creation diffrent Kinds of Wepons BUT so far no class inherits from it.
//INHERITANCE
//ABSTRACTION
//ENCAPSULATION

using UnityEngine;
using System.Collections;

public class ShootWithRaycast : MonoBehaviour
{
    
    public int gunDamage = 1, damageExtra=0;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;
    [SerializeField]private float weaponRange = 50f;// Number in seconds which controls how often the player can fire
                                   // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun
   
    [SerializeField]
    private Camera fpsCam;                                              // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private AudioSource gunAudio;                                       // Reference to the audio source which will play our shooting sound effect
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                             // Float to store the time the player will be allowed to fire again, after firing

    bool isItPlayer , isShootingAtObstacle=false;

    //ENCAPSULATION
    public float WeaponRange
    {
        get
        {
            return weaponRange;
        }
        set
        {
            if (value < 0.0f)
            {
                Debug.LogError("WeponRange Cannot be set to negative number!");
            }
            else { weaponRange = value; }

        }
    }


  //  string nameOfLastHeadShot = "a";

    private void Awake()
    {
        if (this.tag == "Player")
        {
            isItPlayer = true;

        }
        else isItPlayer = false;
    }

    void Start()
    {
        GetAllNeceserry();

    }


 


   public virtual void GetAllNeceserry()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();

        // Get and store a reference to our Camera by searching this GameObject and its parents
        // fpsCam = GetComponentInParent<Camera>();

        // fpsCam = GameObject.FindGameObjectWithTag("CameraPlayer").GetComponent<Camera>();//<----------------As becouse multiplayer
        //  fpsCam = this.transform.GetChild(3).gameObject.GetComponent<Camera>();
       // GameObject obj = this.transform.Find("Main Camera").gameObject;
       // fpsCam = obj.GetComponent<Camera>();//this.transform.Find("Main Camera").gameObject.GetComponent<Camera>();
    }

    void GetLaserLine()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();
    }

    void GetGunAudio()
    {
        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();
    }

    void FindCam()
    {
        // Get and store a reference to our Camera by searching this GameObject and its parents
        //->as converted to multiplayer I gonna use different fpsCam = GameObject.FindGameObjectWithTag("CameraPlayer").GetComponent<Camera>();
        fpsCam = GetComponentInChildren<Camera>();
    }


    //ABSTRACTION
  public virtual void Shoot()
    {
        Transform child = transform.GetChild(transform.childCount - 1);
      

        // Update the time when our player can fire next
        nextFire = Time.time + fireRate;

        // Start our ShotEffect coroutine to turn our laser line on and off
        StartCoroutine(ShotEffect());

        //Transform gOrgin;
        Vector3 rayOrigin , gOrgin;

       if(isItPlayer == true)
        {

            // Create a vector at the center of our camera's viewport
            rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
               gOrgin = fpsCam.transform.forward;
        }
        else 
        {
            //Create Vector form GunPoint
            rayOrigin = transform.Find("GunHolder").transform.Find("Hand").transform.Find("GunShootPosition").transform.position;
            gOrgin = transform.Find("GunHolder").transform.Find("Hand").transform.Find("GunShootPosition").transform.forward;
           
        }
      
        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        // Set the start position for our visual effect for our laser to the position of gunEnd
        laserLine.SetPosition(0, gunEnd.position);

        // Check if our raycast has hit anything
       if (Physics.Raycast(rayOrigin, gOrgin//gOrgin.transform.forward// gOrgin.transform.forward//fpsCam.transform.forward
          , out hit, weaponRange))

      
            {
            // Set the end position for our laser line 
            laserLine.SetPosition(1, hit.point);

            // Get a reference to a health script attached to the collider we hit
            Character health = hit.collider.GetComponent<Character>();

            //If script is Attachet to Player
            if(tag == "Player")
            {

                if (hit.collider.name == "Head" )
                {
                   
                    hit.collider.name = "HeadShooted";
                        health = hit.collider.GetComponentInParent<Enemy>();
                        damageExtra = 10;
                        //hit.collider.GetComponentInParent<Enemy>().enemyIsHitOnHead = true;
                        hit.collider.GetComponentInParent<Enemy>().EnemyIsHitOnHead(true);
                        if (health != null)
                        {
                            //  hit.collider.GetComponentInChildren<DestroyAfterTime>().StartCounting();
                            /////  StartCoroutine(DestroyAfterTime(hit.transform.gameObject, 3));
                            health.Damage(gunDamage + damageExtra);
                            GetComponent<PlayerController>().AddScore(gunDamage + damageExtra);

                        }
                        else if (health == null) { Debug.LogError("health: When Shoot With head Probably canot find character script in Parent!----ShootWithRayCast c#"); }               

                }

                else if (hit.transform.name != "Head" && health !=null && hit.transform.tag !="Player")
                {
                    damageExtra = 0;                  

                        // Call the damage function of that script, passing in our gunDamage variable
                        health.Damage(gunDamage + damageExtra);
                            GetComponent<PlayerController>().AddScore(gunDamage);
                    
                }
            }

            //If Script is attachet to Enemy
            if (tag=="Enemy"&& health != null)
            {
                // Call the damage function of that script, passing in our gunDamage variable
                health.Damage(gunDamage);
               
            }


            // Check if the object we hit has a rigidbody attached
            if (hit.rigidbody != null)
            {
                // Add force to the rigidbody we hit, in the direction from which it was hit
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }

            if(hit.transform.tag == "Wall")
            {
                isShootingAtObstacle = true;
            }
            if (hit.transform.tag != "Wall")
            {
                isShootingAtObstacle = false;
            }
        }
        else
        {
           //If we did not hit anything, set the end of the line to a position directly in fornt of the weapon at the distance weponRange
            laserLine.SetPosition(1, rayOrigin + (gOrgin * weaponRange));
            isShootingAtObstacle = false;
        }
    }


    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }

    public virtual bool IsItShootingAtObstacle()
    {
        return isShootingAtObstacle;
    }


    IEnumerator DestroyAfterTime(GameObject g , float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        Destroy(g);
        g.SetActive(false);
    }


}
