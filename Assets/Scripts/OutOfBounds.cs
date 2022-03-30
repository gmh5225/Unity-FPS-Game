using UnityEngine;

public class OutOfBounds : MonoBehaviour
{

	[SerializeField] private Transform respawnPoint;
	[SerializeField] private Transform Player;

	private void OnTriggerEnter(Collider other)
	{
		Player.transform.position = respawnPoint.transform.position;
	}

	//   private void OnCollisionEnter(Collision col)
	//{
	//	if (col.gameObject.name == "Player")
	//	{
	//		col.gameObject.transform.position = respawnPoint.position;
	//		col.gameObject.transform.rotation = respawnPoint.rotation;
	//	}
	//}
}
