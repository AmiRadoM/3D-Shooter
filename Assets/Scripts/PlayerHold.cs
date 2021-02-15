using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SocialPlatforms;
using System;
using UnityEngine.UIElements;

public class PlayerHold : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject bulletHole;
    public GameObject bloodParticle;
    public GameObject damagePopUp;
    public GameObject zoomPosition;
    

    public Transform crossHair;

    public Camera mainCam;

    public int[] cantHoleLayersNums;


    GunProperties gunProp;

    Vector3 gunPosition;

    float camZoom;

    bool isShooting = false;

    //sway
    public float intensity = 2f;
    public float smooth = 8f;

    Quaternion originRotation;

    void Start()
    {
        if (!photonView.IsMine) return;
        
        camZoom = mainCam.fieldOfView;
        gunPosition = transform.localPosition;

        //sway
        originRotation = transform.localRotation;

    }


    void Update()
    {

        if (!photonView.IsMine) return;

        gunProp = transform.GetChild(0).GetComponent<GunProperties>();

        if (transform.childCount > 0)
        {
            ChangeLayerRecursivly(transform.GetChild(0).gameObject, 14);
            //transform.GetChild(0).gameObject.layer = 14;
        }

        if (!player.GetComponent<PlayerController>().isPaused)
        {
            //Shoot
            if (transform.childCount == 1)
            {

                if (Input.GetMouseButtonDown(0) && !gunProp.holdFire && !isShooting)
                {
                    Shoot();


                }
                else if (Input.GetMouseButton(0) && gunProp.holdFire && !isShooting)
                {

                    Shoot();

                }
            }

            //Aim
            if (Input.GetMouseButton(1))
            {
                mainCam.fieldOfView = transform.GetChild(0).GetComponent<GunProperties>().aimZoom;
                photonView.RPC("ZoomGunHold", RpcTarget.All, null);
                transform.GetChild(0).GetComponent<AudioManager>().Play("Zoom");

            }
            else
            {
                mainCam.fieldOfView = camZoom;
                photonView.RPC("NormalGunHold", RpcTarget.All, gunPosition);
                transform.GetChild(0).GetComponent<AudioManager>().Play("UnZoom");


            }


            //sway
            UpdateSway();

        }


        
    }


    //sway


    void UpdateSway()
    {
        //controls
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion adjX = Quaternion.AngleAxis(-intensity * MouseX, Vector3.up);
        Quaternion adjY = Quaternion.AngleAxis(intensity * MouseY, Vector3.right);
        Quaternion targetRotation = originRotation * adjX * adjY;

        //rotate to target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);

    }


    public void Shoot()
	{
        photonView.RPC("ShootAnimation", RpcTarget.All, null);


        Ray rayShoot = Camera.main.ScreenPointToRay(crossHair.position);

        RaycastHit hitObject;

        if (Physics.Raycast(rayShoot, out hitObject))
        {
            if (hitObject.transform.CompareTag("Player"))
            {
                isShooting = true;
                CreateBlood(hitObject.point, hitObject.normal);
                transform.root.GetComponent<AudioManager>().Play("Hit");
                CreateDamagePopup(hitObject.transform, gunProp.damage);
                hitObject.transform.GetComponent<PlayerController>().photonView.RPC("Damage", RpcTarget.Others, gunProp.damage,player.GetComponent<PlayerController>().playerNameString);
            }
            else if (hitObject.transform.gameObject.layer == 0)
            {

                isShooting = true;
                photonView.RPC("CreateHole", RpcTarget.All, hitObject.point, hitObject.normal);

            }
        }
    }

    public void ChangeIsShooting(bool change)
	{
        isShooting = change;
	}

    [PunRPC]
    public void ShootAnimation()
    {
        transform.GetComponentInChildren<GunProperties>().GetComponent<Animator>().SetTrigger("Shoot");
    }

    

    [PunRPC]
    public void ZoomGunHold()
    {

        transform.localPosition = zoomPosition.transform.localPosition;
        
    }

    [PunRPC]
    public void NormalGunHold(Vector3 position)
    {

        transform.localPosition = position;
        
    }

    [PunRPC]
    public void CreateHole(Vector3 hitObjectPoint,Vector3 hitObjectNormal)
    {
        GameObject newHole = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Bullet Hole"), hitObjectPoint + hitObjectNormal * 0.001f, Quaternion.identity) as GameObject;
        newHole.transform.LookAt(hitObjectPoint + hitObjectNormal);
        newHole.GetComponent<HoleController>().InvokeSuicide();
    }



    public void CreateBlood(Vector3 hitObjectPoint, Vector3 hitObjectNormal)
    {
        GameObject newBloodParticle = Instantiate(bloodParticle, hitObjectPoint + hitObjectNormal * 0.001f, Quaternion.identity) as GameObject;
        newBloodParticle.transform.LookAt(hitObjectPoint + hitObjectNormal);
    }

    public void CreateDamagePopup(Transform pos, int dmg)
	{
        GameObject d = Instantiate(damagePopUp,pos.position,Quaternion.Euler(0,90,0));
        d.transform.GetChild(0).GetComponent<DamagePopUp>().Init(dmg, player);
	}

    public void ChangeLayerRecursivly(GameObject obj, int newLayer)
	{
        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
		{
            ChangeLayerRecursivly(child.gameObject, newLayer);
		}
	}
}
