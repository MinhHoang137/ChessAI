using UnityEngine;

public class VerticalMarker : MonoBehaviour
{
    [SerializeField] private string vertical;
	private void Start()
	{
		Vector3 origin = transform.position;
		Vector3 direction = transform.forward;
		RaycastHit[] hits = Physics.RaycastAll(origin, direction, 10 * Unit.Length);
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.TryGetComponent(out Block block))
			{
				block.SetVertical(vertical);
			}
		}
	}
}
