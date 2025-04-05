using UnityEngine;

public class HorizontalMarker : MonoBehaviour
{
    [SerializeField] private string horizontal;
    void Start()
    {
        Vector3 origin = transform.position;
		Vector3 direction = transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, 10 * Unit.Length);
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.TryGetComponent(out Block block))
			{
				block.SetHorizontal(horizontal);
			}
		}
	}
}
