using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int minDamage, maxDamage;
    public Camera playerCamera;
    public float range = 300f;
    public ParticleSystem muzzleFlash;     //Ate� etme efekti,ate� ��kart�yor
    public GameObject impactEffect;    //Mermi izi ve ate� edilen yerde k�v�lc�m ��karmak i�in

    private EnemyManager enemyManager;
    public AudioSource gunSound;

 
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.timeScale > 0)
        {
            Fire();
            muzzleFlash.Play();
            gunSound.Play();
        }
    }

    void Fire()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {        
            enemyManager = hit.transform.GetComponent<EnemyManager>();
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));       //Vurdu�umuz yerde k�v�lc�m ve mermi izi olu�turma kodu

            if ( enemyManager != null)
            {
                enemyManager.EnemyTakeDamage(Random.Range(minDamage, maxDamage));
            }
           
        }              
    }
}
