using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class Ladder : MonoBehaviour
{

    public GameObject step;
    public GameObject Pole1;
    public GameObject Pole2;
    public int numOfSteps;
    public float baseHeight;

    // Start is called before the first frame update
    void Start()
    {

        Pole1.transform.localPosition = new Vector3(Pole1.transform.localPosition.x, Pole1.transform.lossyScale.y - baseHeight, Pole1.transform.localPosition.z);
        Pole2.transform.localPosition = new Vector3(Pole2.transform.localPosition.x, Pole2.transform.lossyScale.y - baseHeight, Pole2.transform.localPosition.z);
        for (int i = 0; i < numOfSteps; i++)
        {
            GameObject newStep = Instantiate(step, new Vector3(step.transform.position.x, Pole1.transform.localScale.y * 2 * ((float)i / ((float)numOfSteps)), step.transform.position.z), Quaternion.Euler(step.transform.localRotation.x, step.transform.localRotation.y, 90), transform.Find("Steps"));
            newStep.SetActive(true);
            newStep.transform.localPosition = new Vector3(step.transform.localPosition.x, (Pole1.transform.localScale.y - baseHeight) * 2 * ((float)i / ((float)numOfSteps - 1f)), step.transform.localPosition.z);
            newStep.transform.localRotation = step.transform.localRotation;

        }


        step.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        GameObject player = other.gameObject;
        if (player.layer == 12 && player.name != "GroundCheck")
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.useGravity = false;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) /*rb.velocity.normalized == (rb.position - transform.position).normalized*/)
			{
                rb.velocity = new Vector3(rb.velocity.x,3f, rb.velocity.z);
			}
			else 
			{
                rb.velocity = new Vector3(rb.velocity.x, -3f, rb.velocity.z);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject player = other.gameObject;
        if (player.layer == 12 && player.name != "GroundCheck")
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.useGravity = true;
        }
    }
}
