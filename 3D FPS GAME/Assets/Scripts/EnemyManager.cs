using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;     //NavMesh kulland���m�z i�in ekledik.

public class EnemyManager : MonoBehaviour
{
    public int EnemyHealth = 200;
    
    //NavMesh
    public NavMeshAgent enemyAgent;
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    //Patrolling:Devriye gezmek, d��man bizi g�rmedi�i zaman �al��acak.
    public Vector3 walkPoint;
    public float walkPointRange;
    public bool walkPointSet;

    //Detecting ve Attacking
    public float sightRange, attackRange;    //Bizi g�rme mesafesine girince d��man bize do�ru gelecek ve atak mesafesine girince atak edecek.
    public bool EnemySightRange, EnemyAttackRange;

    //Attacking
    public float attackDelay;
    public bool isAttacking;
    public Transform attackPoint;
    public GameObject projectile;    //Mermi,kur�un gibi bir�ey
    public float projectileForce = 18f;

    public Animator enemyAnimator;
    private GameManager gameManager;

    //Particle Effect
    public ParticleSystem deadEffect;
    public Transform deadEffectPoint;

    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemyAnimator = GetComponent<Animator>();
    }

    
    void Update()
    {
        EnemySightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        EnemyAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
      
        if (!EnemySightRange && !EnemyAttackRange)
        {
            //Patrolling yani devriye gezerken ki kodlar� yaz�yoruz.Hem atak hem de g�r�� mesafesinin d���ndayken yani.
            Patrolling();
            enemyAnimator.SetBool("Patrolling", true);
            enemyAnimator.SetBool("PlayerAttacking", false);
            enemyAnimator.SetBool("PlayerDetecting", false);
        }

        else if (EnemySightRange && !EnemyAttackRange)
        {
            //Detecting durumunda yani bizi ilk g�rd��� ve bize do�ru hareketlenmeye ba�layaca�� an.
            DetectPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerAttacking", false);
            enemyAnimator.SetBool("PlayerDetecting", true);
        }

        else if (EnemySightRange && EnemyAttackRange)
        {
            //Bize sald�rmaya ba�layaca�� kodlar
            AttackPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerAttacking", true);
            enemyAnimator.SetBool("PlayerDetecting", false);
        }
    }


    void Patrolling()   //Devriye gezdirme kodlar�.
    {
        if (walkPointSet == false)
        {
            float randomZPos = Random.Range(-walkPointRange, walkPointRange);
            float randomXPos = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomXPos, transform.position.y, transform.position.z + randomZPos);   //Y�r�me alan�na x ve z d�zleminde rastgele de�erler verdik.

            if (Physics.Raycast(walkPoint, -transform.up ,2f, groundLayer))     //Zemin olan yerlerde y�r�mesi i�in �ukura falan d��memesi i�in zemin kontrol� yapt�k. 1.Raycastin olu�aca�� yer,2.Y�n� yani a�a�� do�ru,3.I��n�n uzakl���,4.Katman
            {
                walkPointSet = true;
            }          
        }

        if (walkPointSet == true)
        {
            enemyAgent.SetDestination(walkPoint);   //walkpointset true oldu�unda yani gidece�i bir nokta belli oldu�u zaman navmeshden onu hesaplat�p oraya gitmesini sa�lad�k.
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;     //Rastgele bir nokta olunca d��man oraya gidip durur. Bunu d�ng�ye sokmak i�in anl�k y�r�d��� yer ile ula�maya �al��t��� yer aras�ndaki mesafe 1f'in alt�na d���nce
                                                                          //de�erimiz tekrar false olacak ve d�ng� ba�a d�necek. B�ylelikle durmadan s�rekli dola�acak bir d��man elde edilecek.
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    
    void DetectPlayer()    //D��man�n g�r�� menziline girdi�imiz kodlar.
    {
        enemyAgent.SetDestination(player.position);     //Bizi tespit etti�inde bize do�ru bir yol �izecek.
        //transform.LookAt(player);    //Bize do�ru bakacak.
    }

    void AttackPlayer()    //Bize atak edece�i kodlar.
    {
        enemyAgent.SetDestination(transform.position);    //Bize do�ru daha gelmesin uzaktan at�� yapacak.
        transform.LookAt(player);      
         
        if (isAttacking == false)     //E�er atak etmiyorsa �nce bize at�� yapacak de�i�kenimiz true olacak. Ondan sonra Invoke metoduyla bizim belirleyece�imiz s�re sonunda tekrar false olup yenide atak yapabilecek.
        {
            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);    //Forcemode impulse k�tleyi kullanarak cisme anl�k bir kuvvet impulsu ekler.

            isAttacking = true;
            Invoke("ResetAttack", attackDelay);
        }
    }


    void ResetAttack()
    {
        isAttacking = false;
    }


    public void EnemyTakeDamage(int DamageAmount)
    {
        EnemyHealth -= DamageAmount;

        if (EnemyHealth <=0)
        {
            EnemyDeath();
        }
    }

    void EnemyDeath()
    {
        Destroy(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        gameManager.AddKill();
        Instantiate(deadEffect, deadEffectPoint.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()    //Belirledi�imiz rangeleri scene ekran�nda g�rmemizi sa�lar.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
