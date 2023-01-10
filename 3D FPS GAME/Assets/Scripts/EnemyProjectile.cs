using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damageAmount = 25;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerController player = other.transform.GetComponent<PlayerController>();
            player.PlayerTakeDamage(damageAmount);    //PlayerController'daki playertakedamage'e eriþtik ve ordaki deðere üstte belirttiðimiz damage deðerini verdik.
            Destroy(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject, 1f);     //Eðer bize çarpmazsa bile 1 saniye sonra silinsinler.Etrafta çok top olmasýn.
        }
    }
}
