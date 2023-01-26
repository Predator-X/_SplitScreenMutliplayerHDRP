//INHERITANCE - Enemy Inherits From character class 
//Manages Enemy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{   //For Targeting Player
    public NavMeshAgent agent;
    public Transform player;
    public GameObject[] players; 
    public LayerMask WhatIsGround, WhatIsPlayer,WhatToAfoid;

    //Patrolling
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    //Attacking
    public float timeBetweenAttacks;
    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange,alreadyAttacked , obstacleInWay=false;

    //Shooting
    ShootWithRaycast attack;
    private float nextFire;

    //Dead effect
  public bool  enemyIsHitOnHead=false;

    //Check distance between two objects
    float distanceBetweenObjects, distanceBetweenObjects2,holder;
    List<Transform> p = new List<Transform>();

    private void Awake()
    {//Find Player
       // player = GameObject.FindGameObjectWithTag("Player").transform;
     //   players = GameObject.FindGameObjectsWithTag("Player");//<---For Multiplayer
        agent.GetComponent<NavMeshAgent>();

        attack = this.GetComponent<ShootWithRaycast>();

        SaveSystem.getEnemysOnStart = GameObject.FindGameObjectsWithTag("Enemy");

        

    }

    private void Start()
    {
      //  player = GameObject.FindGameObjectWithTag("Player").transform;
    }

     Transform GetClosestPlayer(Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
        }

        return bestTarget;
    }

   public void CheckForSightAndAttackRange()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        //Checks Which Players is the Closest And Sets it As Target
        if (Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer))
        {

            players = GameObject.FindGameObjectsWithTag("Player");
            //  Debug.Log(" number of players: " + players.Length);
            for (int i = 0; i <= players.Length - 1; i++)
            {
                p.Add(players[i].gameObject.transform);

            }
            player = GetClosestPlayer(p.ToArray());
        }
    }

    void Update()
    {//Check for Sight and Attack Range

        CheckForSightAndAttackRange();

        //Check distance between two objects
        /*
         *   RaycastHit rayCast;
                if (Physics.SphereCast(transform.position, attackRange,transform.forward,out rayCast, 10,WhatToAfoid))
                {
                    player = rayCast.collider.gameObject.transform;
                }


        ---------------
                for (int i=0; i <= players.Length; i++)
                {
                    distanceBetweenObjects = Vector3.Distance(this.transform.position, players[i].transform.position);
                    holder = distanceBetweenObjects;
                    if(holder )
                }
               */

        if (!playerInSightRange & !playerInAttackRange ) Patrolling();
        if (playerInSightRange || obstacleInWay & !playerInAttackRange  ) ChasePlayer();
        if (playerInSightRange && playerInAttackRange & !obstacleInWay) AttackPlayer();


        //Behafiour  when enemy standing in fornt of the wall do this ....
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, attackRange ))
        {

            if (hit.transform.tag == "Wall")
            {
                obstacleInWay = true;  //<-- this will make go back in update to chase the player
            }
            else if (hit.transform.tag != "Wall") { obstacleInWay = false; }
        }

    }


    public override void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
     

        if (currentHealth <= 0 && enemyIsHitOnHead)
        {
            GameObject body, gun, head;
            body = this.transform.Find("Body").gameObject;
            body.GetComponent<DisActivateAfter>().enabled = true;
            body.AddComponent<Rigidbody>();
            body.transform.parent = null;
         

            gun = this.transform.Find("GunHolder").gameObject;
            gun.GetComponent<DisActivateAfter>().enabled = true;
            gun.AddComponent<Rigidbody>();
            gun.transform.parent = null;

           
            head = this.transform.Find("HeadShooted").gameObject;
            head.GetComponent<DisActivateAfter>().enabled = true;
            head.AddComponent<Rigidbody>();
            head.transform.parent = null;

            isDead = true;
            gameObject.SetActive(false);


        }
        else if(currentHealth <=0 && enemyIsHitOnHead == false)
        {
            isDead = true;
            gameObject.SetActive(false);
        }

    }

    public virtual void EnemyIsHitOnHead(bool gotHit)
    {
        enemyIsHitOnHead = gotHit;
    }


   public void Patrolling()
    {
        if (!walkPointSet) SearchWalkingPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
          
            //Calculate Distance to walkpoint
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false; obstacleInWay = false;
        }
    }

    void SearchWalkingPoint()
    {//Calculate random point Range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Check if Walking Point is not outside of the map
        if(Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsGround))  walkPointSet = true; 


    }

    void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
  
    }

  public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void AttackPlayer()
    {
        //Make sure enemy Does not move
        agent.SetDestination(transform.position);
        transform.LookAt(player.transform);

        if (!alreadyAttacked)
        {//Attack Code Here
            Transform pointGun = transform.Find("GunHolder");
            pointGun.transform.LookAt(player.transform); 

            attack.Shoot();

            //
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }
}