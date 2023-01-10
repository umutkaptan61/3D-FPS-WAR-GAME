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
        Cursor.lockState = CursorLockMode.Locked;    //Bu hazýr bir fonksiyon Unityde playe bastýðýmýzda mouseu yok ederek daha kolay döndürmeyi saðlar. Mouseun ekranýn kenarýna gelip hareketimizi kýsýtlamasýný engellemiþ oluruz.
    }

    // Update is called once per frame
    void Update()
    {
        float mouseXPos = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseYPos = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseYPos;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);        //Clamp fonksiyonu sýnýrlamak için kullanýlýr.Birinci parametresi neyi sýnýrlayacaðýmýz, diðer paramatreleri sýrayla min ve max deðerleridir.Burada yukarý aþaðý çok dönmesin diye sýnýrladýk.

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);     //Euler döndürme iþlemleri için kullandýðýmýz metodlardan. Lerp,Slerp,Rotate gibi fonksiyonlarda var.

        player.Rotate(Vector3.up * mouseXPos);
    }
}
