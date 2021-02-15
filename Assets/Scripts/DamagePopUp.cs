using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    public float destroyTime;
    public Vector3 offset;
	public Vector3 randomiseOffset;
	public Color damageColor;

    private TextMeshPro text;



	private GameObject player;

	private void Awake()
	{
		text = GetComponent<TextMeshPro>();
		transform.parent.localPosition += offset;
		transform.parent.localPosition += new Vector3(
			Random.Range(-randomiseOffset.x, randomiseOffset.x),
			Random.Range(-randomiseOffset.y, randomiseOffset.y),
			Random.Range(-randomiseOffset.z, randomiseOffset.z));
		Destroy(transform.parent.gameObject, destroyTime);

	}

	private void Update()
	{
		transform.LookAt(2*transform.position - player.transform.position);
	}

	public void Init(int Damage, GameObject newPlayer)
	{
		text.text = Damage.ToString();
		float size = 0.15f + Damage * 0.01f;
		transform.localScale = new Vector3(size, size, size);
		player = newPlayer;
	}
}
