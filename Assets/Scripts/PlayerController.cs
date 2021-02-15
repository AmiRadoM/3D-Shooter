using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public LayerMask groundMask;

    public CharacterController controller;

    public Rigidbody rb;

    public GameObject face;
    public GameObject rightHand;
    public GameObject pauseMenu;

    public TMP_Text nameText;
    public TMP_Text healthText;

    public Image HealthBG;

    public Transform groundCheck;

    public float walkSpeed = 15f;
    public float runSpeed = 30f;
    public float jumpForce = 1f;
    public float jumpheight = 3f;

    public int playerHealth = 100;

    public string playerNameString;

    [HideInInspector]
    public bool isPaused = false;


    StaticData staticData;
    GameObject gameSetup;

    Color statsBGColor;

    float groundDistance = 0.4f;
    float speed;

    bool isDead = false;
    bool isGrounded;


    void Start()
    {

        staticData = GameObject.Find("Static Data").GetComponent<StaticData>();
        gameSetup = GameObject.Find("GameSetup");

        if (!photonView.IsMine)
        {

            return;
        }

        rb = GetComponent<Rigidbody>();

        playerNameString = staticData.getPlayerName();
        if (playerNameString == null)
        {
            playerNameString = "Player" + PhotonNetwork.CurrentRoom.PlayerCount;
        }

        statsBGColor = HealthBG.GetComponent<Image>().color;
        tag = "LocalPlayer";
        ChangeLayerRecursivly(gameObject, 12);
        face.layer = 13;

        
    }

    void Update()
    {

        if (!photonView.IsMine)
        {


            return;
        }

        //Show Name
        photonView.RPC("NameSelf", RpcTarget.All, playerNameString);

        //Show Face
        photonView.RPC("ShowFace", RpcTarget.All, staticData.getFaceName());

		//ShowHealth
		if (!isDead)
		{
            healthText.text = playerHealth.ToString();
            HealthBG.color = new Color(-0.004f * playerHealth + 1f, statsBGColor.g, statsBGColor.b, statsBGColor.a);
        }


        





        //PauseMenu
        if (Input.GetKeyDown(KeyCode.Escape) && !isDead)
        {
            if (!pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isPaused = true;
            }
            else
            {
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isPaused = false;
            }

        }


    }

    void FixedUpdate()
    {


        if (!photonView.IsMine)
        {


            return;
        }





        if (!isDead)
		{




            //Ground Check
            //isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance, groundMask);
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance,groundMask);

			if (isGrounded && rb.velocity.y < 0)
			{
                rb.velocity = new Vector3(rb.velocity.x,-2f, rb.velocity.z);
			}
            

            //Movement
            float x = 0;
            float z = 0;

            if (!isPaused)
            {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");
            }

            Vector3 move = (x * transform.right + z * transform.forward) * speed;

            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

            //StepSound
            if ((x != 0 || z != 0) && isGrounded && speed == walkSpeed && GetComponent<AudioSource>().isPlaying == false)
            {
                GetComponent<AudioSource>().pitch = Random.Range(0.75f, 0.9f);
                GetComponent<AudioManager>().Play("FootStep");
            }
            else if ((x != 0 || z != 0) && isGrounded && speed == runSpeed && GetComponent<AudioSource>().isPlaying == false)
            {
                GetComponent<AudioSource>().pitch = Random.Range(1f, 1.2f);
                GetComponent<AudioManager>().Play("FootStep");
            }


            //Jumping
            if (isGrounded && Input.GetButton("Jump") && !isPaused)
            {
                //rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
                //rb.AddForce(0, jumpForce*Time.fixedDeltaTime, 0, ForceMode.Impulse);
                //rb.velocity += new Vector3(0, jumpForce, 0);
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpheight*-2f*Physics.gravity.y), rb.velocity.z);
            }

            //Run
            if (Input.GetKey(KeyCode.LeftShift) && !isPaused)
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }

            //Die Y
            if (transform.position.y <= FindObjectOfType<GameSetupController>().DeathY)
            {
                Die("You fell from the edge");

            }

            //0 Health Death
            if (playerHealth <= 0)
            {
                Die("You Died");

            }
        }



    }

    [PunRPC]
    public void Damage(int dmg, string name)
    {
        if (photonView.IsMine)
        {
            playerHealth -= dmg;
            if (playerHealth < 0)
            {
                playerHealth = 0;
                if(!isDead)
                    Die("You were killed by " + name);
            }


        }


    }


    public void Die(string msg)
    {
        gameSetup.GetComponent<GameSetupController>().Respawn(msg);
        //PhotonNetwork.Destroy(gameObject);
        isDead = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(transform.forward*-15f,ForceMode.Impulse);
        Destroy( transform.Find("Head").transform.Find("Cameras").gameObject);
        Destroy(rightHand.GetComponent("PlayerHold"));
        Destroy(transform.Find("UI").gameObject);
        Invoke("Suicide", 10f);

    }

    public void Suicide()
	{
        PhotonNetwork.Destroy(rightHand.transform.GetChild(0).gameObject);
        PhotonNetwork.Destroy(gameObject);
	}

    [PunRPC]
    public void showHealth()
    {
        healthText.text = playerHealth.ToString();
    }

    [PunRPC]
    public void NameSelf(string name)
    {

        nameText.text = name;
    }

    [PunRPC]
    public void ShowFace(string name)
    {
        face.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Path.Combine("Faces", name));
    }


    public void Continue()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ChangeLayerRecursivly(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursivly(child.gameObject, newLayer);
        }
    }
}
