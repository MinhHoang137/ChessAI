using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class King : ChessPiece
{
    private Tuple<int, int>[] directions = new Tuple<int, int>[]
	{
		new Tuple<int, int>(1, 1),
		new Tuple<int, int>(1, 0),
		new Tuple<int, int>(1, -1),
		new Tuple<int, int>(0, 1),
		new Tuple<int, int>(0, -1),
		new Tuple<int, int>(-1, 1),
		new Tuple<int, int>(-1, 0),
		new Tuple<int, int>(-1, -1)
	};
	private Block rightCastleBlock;
	private Block leftCastleBlock;
	private bool firstMove = true;
	private Rook rightRook;
	private Rook leftRook;
	protected override void InitializeRay()
	{
		rays.Clear();
		Vector3 origin = currentBlock.transform.position;
		Vector3 forward = currentBlock.transform.forward;
		Vector3 right = currentBlock.transform.right;
		foreach (Tuple<int, int> direction in directions)
		{
			rays.Add(new Ray(origin, (forward * direction.Item1 + right * direction.Item2).normalized));
		}
	}
	public override void FindWay()
	{
		StartCoroutine(FindWayCoroutine(1 * Unit.Diagonal));
	}
	public override IEnumerator<WaitForSeconds> FindWayCoroutine(float rayLength = 0)
	{
		StartCoroutine(base.FindWayCoroutine(rayLength));
		if (firstMove) FindCastleBlocks();
		yield return null;
	}
	public override bool MoveTo(Block block)
	{
		if (!moveableBlocks.Contains(block) || block == currentBlock)
		{
			return false;
		}
		if (firstMove) {
			if (block == rightCastleBlock)
			{
				// rook's position is 1 block away from original king
				rightRook.Castle(BoardManager.Instance.GetRelativeBlock(currentBlock, 1, 0));
				log = "0 - 0";
			}
			else if (block == leftCastleBlock)
			{
				// rook's position is 1 block away from original king
				leftRook.Castle(BoardManager.Instance.GetRelativeBlock(currentBlock, -1, 0));
				log = "0 - 0 - 0";
			}
			else {
				log = NormalLog(block);
			}
		}
		firstMove = false;
		ClearCastle();
		StartCoroutine(MoveToCoroutine(block));
		return true;
	}
	private void FindCastleBlocks()
	{
		if (BoardManager.Instance.IsKingInCheck(GetSide())) {
			leftCastleBlock = null;
			rightCastleBlock = null;
			return;
		}
		// find right castle block
		int right = 1;
		int distance = 2;
		for (int i = 1; i <= distance; i++) {
			Block block = BoardManager.Instance.GetRelativeBlock(currentBlock, right * i, 0);
			if (block == null || block.GetChessPiece() != null || !IsSafeToMoveTo(block))
			{
				rightCastleBlock = null;
				break;
			}
			if (i == distance)
			{
				// right rook is 3 blocks away from king
				Block rightRookBlock = BoardManager.Instance.GetRelativeBlock(currentBlock, 3, 0);
				rightRook = rightRookBlock?.GetChessPiece() as Rook;
				if (rightRook != null && rightRook.FirstMove) {
					rightCastleBlock = block;
					moveableBlocks.Add(rightCastleBlock);
				}
			}
		}
		// find left castle block
		int left = -1;
		for (int i = 1; i <= distance; i++)
		{
			Block block = BoardManager.Instance.GetRelativeBlock(currentBlock, left * i, 0);
			if (block == null || block.GetChessPiece() != null || !IsSafeToMoveTo(block))
			{
				leftCastleBlock = null;
				break;
			}
			if (i == distance)
			{
				// left rook is 4 blocks away from king
				Block leftRookBlock = BoardManager.Instance.GetRelativeBlock(currentBlock, -4, 0);
				leftRook = leftRookBlock?.GetChessPiece() as Rook;
				// check if there is a knight between king and left rook, left knight is 3 blocks away from king
				bool isKightOccupied = BoardManager.Instance.GetRelativeBlock(currentBlock, -3, 0)?.GetChessPiece() != null;
				if (leftRook != null && leftRook.FirstMove && !isKightOccupied)
				{
					leftCastleBlock = block;
					moveableBlocks.Add(leftCastleBlock);
				}
			}
		}
	}
	private void ClearCastle()
	{
		rightCastleBlock = null;
		leftCastleBlock = null;
		rightRook = null;
		leftRook = null;
	}
	protected override void Register()
	{
		base.Register();
		pieceName = "K";
	}
}
