using System;
using UnityEngine;

public abstract class Bishop : ChessPiece
{
	private Tuple<int, int>[] directions = new Tuple<int, int>[]
	{
		new Tuple<int, int>(1, 1),
		new Tuple<int, int>(1, -1),
		new Tuple<int, int>(-1, 1),
		new Tuple<int, int>(-1, -1)
	};
	protected override void InitializeRay()
	{
		base.InitializeRay();
		Vector3 origin = currentBlock.transform.position;
		foreach (Tuple<int, int> direction in directions)
		{
			Vector3 directionVector = new Vector3(direction.Item1, 0, direction.Item2);
			Ray ray = new Ray(origin, directionVector);
			rays.Add(ray);
		}
	}
	override protected void FindWay()
	{
		StartCoroutine(FindWayCoroutine(8 * Unit.Diagonal));
	}
	protected override void Register()
	{
		base.Register();
		pieceName = "B";
	}
}
