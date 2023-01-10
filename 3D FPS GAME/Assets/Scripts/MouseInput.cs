using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Transform player;
    public float mouseSens = 200f;    //Mouse sensivity
    private float xRotation;
   

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;    //Bu haz�r bir fonksiyon Unityde playe bast���m�zda mouseu yok ederek daha kolay d�nd�rmeyi sa�lar. Mouseun ekran�n kenar�na gelip hareketimizi k�s�tlamas�n� engellemi� oluruz.
    }

    // Update is called once per frame
    void Update()
    {
        float mouseXPos = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseYPos = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseYPos;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);        //Clamp fonksiyonu s�n�rlamak i�in kullan�l�r.Birinci parametresi neyi s�n�rlayaca��m�z, di�er paramatreleri s�rayla min ve max de�erleridir.Burada yukar� a�a�� �ok d�nmesin diye s�n�rlad�k.

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);     //Euler d�nd�rme i�lemleri i�in kulland���m�z metodlardan. Lerp,Slerp,Rotate gibi fonksiyonlarda var.

        player.Rotate(Vector3.up * mouseXPos);
    }
}
