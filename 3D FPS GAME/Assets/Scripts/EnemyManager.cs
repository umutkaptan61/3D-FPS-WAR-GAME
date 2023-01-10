using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;     //NavMesh kullandýðýmýz için ekledik.

public class EnemyManager : MonoBehaviour
{
    public int EnemyHealth = 200;
    
    //NavMesh
    public NavMeshAgent enemyAgent;
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    //Patrolling:Devriye gezmek, düþman bizi görmediði zaman çalýþacak.
    public Vector3 walkPoint;
    public float walkPointRange;
    public bool walkPointSet;

    //Detecting ve Attacking
    public float sightRange, attackRange;    //Bizi görme mesafesine girince düþman bize doðru gelecek ve atak mesafesine girince atak edecek.
    public bool EnemySightRange, EnemyAttackRange;

    //Attacking
    public float attackDelay;
    public bool isAttacking;
    public Transform attackPoint;
    public GameObject projectile;    //Mermi,kurþun gibi birþey
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
            //Patrolling yani devriye gezerken ki kodlarý yazýyoruz.Hem atak hem de görüþ mesafesinin dýþýndayken yani.
            Patrolling();
            enemyAnimator.SetBool("Patrolling", true);
            enemyAnimator.SetBool("PlayerAttacking", false);
            enemyAnimator.SetBool("PlayerDetecting", false);
        }

        else if (EnemySightRange && !EnemyAttackRange)
        {
            //Detecting durumunda yani bizi ilk gördüðü ve bize doðru hareketlenmeye baþlayacaðý an.
            DetectPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerAttacking", false);
            enemyAnimator.SetBool("PlayerDetecting", true);
        }

        else if (EnemySightRange && EnemyAttackRange)
        {
            //Bize saldýrmaya baþlayacaðý kodlar
            AttackPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerAttacking", true);
            enemyAnimator.SetBool("PlayerDetecting", false);
        }
    }


    void Patrolling()   //Devriye gezdirme kodlarý.
    {
        if (walkPointSet == false)
        {
            float randomZPos = Random.Range(-walkPointRange, walkPointRange);
            float randomXPos = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomXPos, transform.position.y, transform.position.z + randomZPos);   //Yürüme alanýna x ve z düzleminde rastgele deðerler verdik.

            if (Physics.Raycast(walkPoint, -transform.up ,2f, groundLayer))     //Zemin olan yerlerde yürümesi için çukura falan düþmemesi için zemin kontrolü yaptýk. 1.Raycastin oluþacaðý yer,2.Yönü yani aþaðý doðru,3.Iþýnýn uzaklýðý,4.Katman
            {
                walkPointSet = true;
            }          
        }

        if (walkPointSet == true)
        {
            enemyAgent.SetDestination(walkPoint);   //walkpointset true olduðunda yani gideceði bir nokta belli olduðu zaman navmeshden onu hesaplatýp oraya gitmesini saðladýk.
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;     //Rastgele bir nokta olunca düþman oraya gidip durur. Bunu döngüye sokmak için anlýk yürüdüðü yer ile ulaþmaya çalýþtýðý yer arasýndaki mesafe 1f'in altýna düþünce
                                                                          //deðerimiz tekrar false olacak ve döngü baþa dönecek. Böylelikle durmadan sürekli dolaþacak bir düþman elde edilecek.
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    
    void DetectPlayer()    //Düþmanýn görüþ menziline girdiðimiz kodlar.
    {
        enemyAgent.SetDestination(player.position);     //Bizi tespit ettiðinde bize doðru bir yol çizecek.
        //transform.LookAt(player);    //Bize doðru bakacak.
    }

    void AttackPlayer()    //Bize atak edeceði kodlar.
    {
        enemyAgent.SetDestination(transform.position);    //Bize doðru daha gelmesin uzaktan atýþ yapacak.
        transform.LookAt(player);      
         
        if (isAttacking == false)     //Eðer atak etmiyorsa önce bize atýþ yapacak deðiþkenimiz true olacak. Ondan sonra Invoke metoduyla bizim belirleyeceðimiz süre sonunda tekrar false olup yenide atak yapabilecek.
        {
            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);    //Forcemode impulse kütleyi kullanarak cisme anlýk bir kuvvet impulsu ekler.

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

    private void OnDrawGizmosSelected()    //Belirlediðimiz rangeleri scene ekranýnda görmemizi saðlar.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
