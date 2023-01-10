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
            player.PlayerTakeDamage(damageAmount);    //PlayerController'daki playertakedamage'e eri�tik ve ordaki de�ere �stte belirtti�imiz damage de�erini verdik.
            Destroy(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject, 1f);     //E�er bize �arpmazsa bile 1 saniye sonra silinsinler.Etrafta �ok top olmas�n.
        }
    }
}
