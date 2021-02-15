using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviourPunCallbacks
{
    

    public int mouseSensitivity = 400;

    public Transform head;
    public Transform player;
    public Transform rightHand;

    public GameObject Cameras;
    public GameObject UI;

    float xRotation = 0f;

    GameObject staticData;

    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(UI);
            DestroyImmediate(Cameras);
            return;
        }

        staticData = GameObject.Find("Static Data");
        mouseSensitivity = staticData.GetComponent<StaticData>().getSensitivity()*5;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!photonView.IsMine) return;


        if (!player.GetComponent<PlayerController>().isPaused)
        {
            float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= MouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            head.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.Rotate(Vector3.up * MouseX);
        }

    }

    
}
