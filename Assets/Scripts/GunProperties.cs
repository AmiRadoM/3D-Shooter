using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GunProperties : MonoBehaviour
{

    public PlayerHold playerHoldParent;

    public float aimZoom;

    public int damage;
    public int numAmmo;
    public int maxAmmo;

    public bool holdFire;

    public void Start()
	{
        
    }
    
    public void Update()
	{
        if (transform.parent != null)
        {
            if (transform.parent.name.Contains("Hand"))
            {
                playerHoldParent = transform.parent.GetComponent<PlayerHold>();
            }

        }
    }

    public void ChangeIsShooting(int change)
	{
        if (change == 0)
            playerHoldParent.ChangeIsShooting(false);
        else
            playerHoldParent.ChangeIsShooting(true);

    }

    public void Suicide()
	{
        Destroy(gameObject);
	}
}
