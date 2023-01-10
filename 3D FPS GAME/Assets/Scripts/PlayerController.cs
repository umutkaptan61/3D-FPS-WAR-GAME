using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    private float speed = 10f;
    public float gravity = -14f;
    public int PlayerHealth = 100;

    private Vector3 gravityVector;

    //Ground Check : Zemin tespit de�i�kenleri

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.35f;     //Cismin etraf�nda bir daire olu�turup ona g�re yere de�ip de�medi�ini kontrol edecez.
    public LayerMask groundLayer;

    public bool isGrounded = false;
    public float jumpSpeed = 5f;

    //UI
    public Slider healthSlider;
    public Text healthText;
    public CanvasGroup damageScreenUI;

    private GameManager gameManager;
    public AudioSource takeDamageSound;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();
        damageScreenUI.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        GroundCheck();
        JumpAndGravity();
        DamageScreenCleaner();
    }

    void MovePlayer()
    {
        Vector3 moveVector = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        characterController.Move(moveVector * speed * Time.deltaTime);

        //Hangi y�ne bak�yorsak o y�ne gitmek i�in bir kod yazd�k. Charactercontroller.move metodunu kulland�k. Ara�t�r�lmal�. CharacterController gravityi desteklemiyor biz kendimiz olu�turaca��z.
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        //CheckSphere ile bir k�re olu�turup yere de�ip de�medi�ini kontrol ettik. 1. Cismin alt�na yerle�tirdi�imiz noktan�n pozisyonu, 2. A�� de�eri , 3.Dikkate alaca�� katman
    }

    void JumpAndGravity()
    {
        gravityVector.y += gravity * Time.deltaTime;
        characterController.Move(gravityVector * Time.deltaTime);     //gravityi uygulamak i�in iki kez zamanla �arpmak gerekiyor.

        if (isGrounded && gravityVector.y < 0)     //Bu kod ile yere d��t���mzdeki yani isGrounded true oldu�u zamanki yer�ekimini -3f yap�p dengeledik. B�ylelikle jump yaparak bu kuvveti a�abilece�iz.
        {
            gravityVector.y = -3f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)     //Z�plama kodu.
        {
            gravityVector.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
    }

    public void PlayerTakeDamage(int DamageAmount)   //Parametre zarar miktar�.
    {
        PlayerHealth -= DamageAmount;
        healthSlider.value -= DamageAmount;
        HealthTextUpdate();
        damageScreenUI.alpha = 1f;
        takeDamageSound.Play();

        if (PlayerHealth <=0)
        {
            PlayerDeath();
            HealthTextUpdate();
            healthSlider.value = 0;
        }
    }

    void PlayerDeath()
    {
        gameManager.RestartGame();
    }

    void HealthTextUpdate()    //Health bar� d�zenlemek i�in kodlar
    {
        healthText.text = PlayerHealth.ToString();
    }
   
    void DamageScreenCleaner()
    {
        if (damageScreenUI.alpha > 0)
        {
            damageScreenUI.alpha -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndTrigger"))
        {
            gameManager.WinLevel();
        } 
    }
}
