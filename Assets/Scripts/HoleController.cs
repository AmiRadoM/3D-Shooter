using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HoleController : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        


    }

    public void InvokeSuicide()
	{
        Invoke("HoleSuicide", 8f);
	}

    public void HoleSuicide()
	{
        PhotonNetwork.Destroy(this.gameObject);
	}
}
