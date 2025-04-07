using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rook : ChessPiece
{
	private bool firstMove = true;
	private Tuple<int, int>[] directions = new Tuple<int, int>[]
	{
		new Tuple<int, int>(1, 0),
		new Tuple<int, int>(-1, 0),
		new Tuple<int, int>(0, 1),
		new Tuple<int, int>(0, -1)
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
	public override bool MoveTo(Block block)
	{
		if (!moveableBlocks.Contains(block) || block == currentBlock)
		{
			return false;
		}
		firstMove = false;
		log = NormalLog(block);
		StartCoroutine(MoveToCoroutine(block));
		return true;
	}
	public void Castle(Block block)
	{
		StartCoroutine(MoveToCoroutine(block));
	}
	public bool FirstMove { get => firstMove; }
	protected override void Register()
	{
		base.Register();
		pieceName = "R";
	}
}
