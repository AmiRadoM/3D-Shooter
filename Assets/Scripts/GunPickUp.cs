using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{

    public GameObject gunPos;

    public bool isEmpty;

    public float respawnTime;

    public string gunName;

    // Start is called before the first frame update
    void Start()
    {
        respawnWeapon();
    }

    // Update is called once per frame
    void Update()
    {
		if (isEmpty)
		{
            Invoke("respawnWeapon", respawnTime);
            isEmpty = false;
		}
    }

    public void respawnWeapon()
	{

        GameObject gun = PhotonNetwork.Instantiate(Path.Combine("Prefabs", gunName), Vector3.zero, Quaternion.Euler(0, 0, 0));

        gun.transform.SetParent(gunPos.transform);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.AddComponent<PhotonTransformView>();

        isEmpty = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12 && other.gameObject.name != "GroundCheck" && gunPos.transform.childCount != 0)
        {
            GetComponent<PhotonView>().RPC("Equip", RpcTarget.All, other.gameObject.GetPhotonView().ViewID);
            


        }
    }

    [PunRPC]
    public void Equip(int otherView)
	{
        GameObject other = PhotonView.Find(otherView).gameObject;
        other.gameObject.GetComponent<PlayerController>().rightHand.transform.GetChild(0).GetComponent<GunProperties>().Suicide();
        GameObject playerGun = PhotonNetwork.Instantiate(Path.Combine("Prefabs", gunName), Vector3.zero, Quaternion.identity);
        playerGun.transform.SetParent(other.gameObject.GetComponent<PlayerController>().rightHand.transform);
        playerGun.transform.localPosition = Vector3.zero;
        playerGun.transform.localRotation = Quaternion.Euler(0, 0, 0);

        GetComponent<AudioManager>().Play("Equip");

        PhotonNetwork.Destroy(gunPos.transform.GetChild(0).gameObject);
        isEmpty = true;
    }


}
