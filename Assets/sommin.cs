using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Security;
using UnityEditor;

public class sommin : MonoBehaviourPun
{
    // Start is called before the first frame update
    [SerializeField]
    public ParticleSystem ps;
    [SerializeField]
    public Animator animator;
    [SerializeField]
    public string Floor ;
    [SerializeField]
    public Rigidbody rb;
    [SerializeField]
    public float rotationSpeed = 500.0f;
    [SerializeField]
    public float speed = 10.0f;
    [SerializeField]
    public int MouseSensitivity = 10; 
    [SerializeField]
    public bool IsOnGround = false;
    [SerializeField]
    public bool IsJumping = false;
    [SerializeField]
    public bool MenuOpen = false;
    [SerializeField]
    public float JumpForce ;
    [SerializeField]
    public Rigidbody bullet;
    [SerializeField]
    public GameObject barrel;
    [SerializeField]
    public int health = 100;
    [SerializeField]
    public float timeInterval = 0.5f;
    [SerializeField]
    public float nexttime = 0;
    [SerializeField]
    public int ammo = 80;
    [SerializeField]
    public int currentammo = 80;
    [SerializeField]
    public Text text;
    [SerializeField]
    public bool IsReloading = false;
    [SerializeField] public Transform playerCamera;
    [SerializeField]
    private float xRotation = 0f;
    [SerializeField]
    public GameObject escscreen;
    void Start()
    {
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
            
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        if (!photonView.IsMine)
        {
            // Try to find the camera. ONLY turn it off if it actually exists!
            Camera playerCam = GetComponentInChildren<Camera>();
            if (playerCam != null) 
            {
                playerCam.gameObject.SetActive(false);
            }

            // Do the same safety check for the AudioListener
            AudioListener playerAudio = GetComponentInChildren<AudioListener>();
            if (playerAudio != null) 
            {
                playerAudio.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!photonView.IsMine)
        {
            return; 
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuShow();
        }
        if(MenuOpen) return;

        text.text = "AMMO:"+currentammo+"HEALTH:"+health;
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * speed;
        animator.SetFloat("Speed", Mathf.Abs(translation));
        if (Input.GetAxis("Horizontal") != 0)
        {
            animator.SetBool("sidewalk", true);
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            animator.SetBool("sidewalk", false);
        }
        if (Input.GetButtonDown("Jump") && IsOnGround)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            IsOnGround = false;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButton("Fire1") && Time.time >= nexttime)
        {
            if (currentammo > 0)
            {
                var emission = ps.emission;
                emission.enabled = true;
                Shoot();
                nexttime = Time.time + timeInterval;
            }
            else
            {
                var emission = ps.emission;
                emission.enabled = false;
            }
            if (currentammo <= 0 && !IsReloading)
            {
                animator.SetBool("IsReloading", true);
                IsReloading = true;
                Invoke("Reload",3.2f);
                
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && !IsReloading)
        {
            animator.SetBool("IsReloading", true);
                Invoke("Reload",3.2f);
        }
        if(Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("IsShootingGun", false);
        }
        // Get the raw mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Vector3 moveVelocity = (transform.forward * translation * speed) + (transform.right * rotation * speed);
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        if (Input.GetKey(KeyCode.LeftShift) && IsOnGround && Input.GetAxis("Vertical") != 0)
        {
            speed = 15f;
            animator.SetBool("IsRunning", true); 
        }
        else
        {
            speed = 10f; 
            animator.SetBool("IsRunning", false);
        }
        
        
    }
    public void takedmg(int damageamount)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SyncDamage",RpcTarget.All,damageamount);
        }
    }
    
    [PunRPC]
    void SyncDamage(int damageamount)
    {
        health -= damageamount;
        if (health <= 0)
        {
            Die();
        }
        

    }
    void Die()
    {
        if (photonView.IsMine)
        {
            Camera myCam = GetComponentInChildren<Camera>();
            
            // 2. If the camera exists, detach it from the monkey!
            if (myCam != null)
            {
                myCam.transform.SetParent(null); 
            }

            // 3. (Optional) Do the same for the AudioListener so you can still hear the game
            AudioListener myAudio = GetComponentInChildren<AudioListener>();
            if (myAudio != null)
            {
                myAudio.transform.SetParent(null);
            }
            PhotonNetwork.Instantiate("ragdoll", transform.position, transform.rotation);
            // 4. NOW it is safe to destroy the monkey. The camera will stay behind!
            PhotonNetwork.Destroy(gameObject);
        }
            
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("ground"))
        {
            IsOnGround = true;
            animator.SetBool("IsJumping", false);
            IsJumping = false;
        }
        if(collision.collider.CompareTag("death"))
        {
            Die();
        }
    }
    void Shoot()
    {
        GameObject bulletObj = PhotonNetwork.Instantiate("bullet", barrel.transform.position, barrel.transform.rotation);
        Rigidbody clone = bulletObj.GetComponent<Rigidbody>();  
        animator.SetBool("IsShootingGun", true);
        currentammo -= 1;
    }
    void Reload()
    {
        currentammo= 60;
        animator.SetBool("IsReloading",false);
        IsReloading = false;
    }
    public void MenuShow()
    {
        MenuOpen = !MenuOpen; // Flips between true and false
        escscreen.SetActive(MenuOpen); // Turns the canvas on and off

        if (MenuOpen)
        {
            // Unlock the mouse so you can click buttons
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
        }
        else
        {
            // Lock the mouse back to the game
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void LeaveMatch()
    {
        PhotonNetwork.LeaveRoom(); 
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); 
    }
}