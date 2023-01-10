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

    //Ground Check : Zemin tespit deðiþkenleri

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.35f;     //Cismin etrafýnda bir daire oluþturup ona göre yere deðip deðmediðini kontrol edecez.
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

        //Hangi yöne bakýyorsak o yöne gitmek için bir kod yazdýk. Charactercontroller.move metodunu kullandýk. Araþtýrýlmalý. CharacterController gravityi desteklemiyor biz kendimiz oluþturacaðýz.
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        //CheckSphere ile bir küre oluþturup yere deðip deðmediðini kontrol ettik. 1. Cismin altýna yerleþtirdiðimiz noktanýn pozisyonu, 2. Açý deðeri , 3.Dikkate alacaðý katman
    }

    void JumpAndGravity()
    {
        gravityVector.y += gravity * Time.deltaTime;
        characterController.Move(gravityVector * Time.deltaTime);     //gravityi uygulamak için iki kez zamanla çarpmak gerekiyor.

        if (isGrounded && gravityVector.y < 0)     //Bu kod ile yere düþtüðümzdeki yani isGrounded true olduðu zamanki yerçekimini -3f yapýp dengeledik. Böylelikle jump yaparak bu kuvveti aþabileceðiz.
        {
            gravityVector.y = -3f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)     //Zýplama kodu.
        {
            gravityVector.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
    }

    public void PlayerTakeDamage(int DamageAmount)   //Parametre zarar miktarý.
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

    void HealthTextUpdate()    //Health barý düzenlemek için kodlar
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
