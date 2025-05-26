using UnityEngine;
using System;

public abstract class Knight : ChessPiece
{
    private Tuple<int, int>[] directions = new Tuple<int, int>[]
	{
		new Tuple<int, int>(1, 2),
		new Tuple<int, int>(1, -2),
		new Tuple<int, int>(-1, 2),
		new Tuple<int, int>(-1, -2),
		new Tuple<int, int>(2, 1),
		new Tuple<int, int>(2, -1),
		new Tuple<int, int>(-2, 1),
		new Tuple<int, int>(-2, -1)
	};
	override protected void InitializeRay()
	{
		base.InitializeRay();
		Vector3 directionVector = -transform.up;
		foreach (Tuple<int, int> direction in directions)
		{
			Vector3 origin = currentBlock.transform.position + currentBlock.transform.forward * direction.Item1 * Unit.Length + currentBlock.transform.right * direction.Item2 * Unit.Length + currentBlock.transform.up;
			Ray ray = new Ray(origin, directionVector);
			rays.Add(ray);
		}
	}
	override public void FindWay()
	{
		StartCoroutine(FindWayCoroutine(3 * Unit.Length));
	}
	protected override void Register()
	{
		base.Register();
		pieceName = "N";
	}
}
