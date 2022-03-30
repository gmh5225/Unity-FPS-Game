using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
	public Transform respawnPoint;

	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Player")
		{
			col.gameObject.transform.position = respawnPoint.position;
			col.gameObject.transform.rotation = respawnPoint.rotation;
		}
	}
}
