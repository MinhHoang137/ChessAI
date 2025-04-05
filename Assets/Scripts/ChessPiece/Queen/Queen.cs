using UnityEngine;

public abstract class Queen : ChessPiece
{
	protected override void InitializeRay()
	{
		base.InitializeRay();

		// 4 hướng thẳng
		rays.Add(new Ray(currentBlock.transform.position, currentBlock.transform.forward));  // Lên
		rays.Add(new Ray(currentBlock.transform.position, -currentBlock.transform.forward)); // Xuống
		rays.Add(new Ray(currentBlock.transform.position, -currentBlock.transform.right));   // Trái
		rays.Add(new Ray(currentBlock.transform.position, currentBlock.transform.right));    // Phải

		// 4 hướng chéo
		rays.Add(new Ray(currentBlock.transform.position, (currentBlock.transform.forward + currentBlock.transform.right).normalized));  // Lên-Phải
		rays.Add(new Ray(currentBlock.transform.position, (currentBlock.transform.forward - currentBlock.transform.right).normalized));  // Lên-Trái
		rays.Add(new Ray(currentBlock.transform.position, (-currentBlock.transform.forward + currentBlock.transform.right).normalized)); // Xuống-Phải
		rays.Add(new Ray(currentBlock.transform.position, (-currentBlock.transform.forward - currentBlock.transform.right).normalized)); // Xuống-Trái
	}

	protected override void FindWay()
	{
		StartCoroutine(FindWayCoroutine(8 * Unit.Diagonal));
	}
}
