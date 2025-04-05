using UnityEngine;

public abstract class Pawn : ChessPiece
{
	protected int stepCount = 0;
	protected bool step2 = false;
	private Block rightEnPassant;
	private Block leftEnPassant;
	private Pawn rightEnPassantPawn;
	private Pawn leftEnPassantPawn;
	private int lastStep;
	protected void PawnFindWay(int forward) // 1: whitePawn, -1: blackPawn
	{
		ClearMoveable();
		moveableBlocks.Add(currentBlock);
		int distance = 2;
		Block block;
		if (stepCount > 0) distance = 1;
		// forward
		for (int i = 1; i <= distance; i++)
		{
			block = BoardManager.Instance.GetRelativeBlock(currentBlock, 0, i * forward);
			if (block.GetChessPiece() != null) break;
			if (IsSafeToMoveTo(block))
			{
				moveableBlocks.Add(block);
			}
		}
		// eat right
		GetEatBlock(1, forward);
		// eat left
		GetEatBlock(-1, forward);
		FindEnPassant(forward);
	}
	public override bool MoveTo(Block block)
	{
		if (!moveableBlocks.Contains(block) || block == currentBlock)
		{
			return false;
		}
		stepCount++;
		if (stepCount == 1)
		{
			float distanceBlock = Vector3.Distance(block.transform.position, currentBlock.transform.position);
			if (Mathf.Abs(distanceBlock - 2 * Unit.Length) < 0.01f)
			{
				step2 = true;
			}
		}
		if (block == rightEnPassant)
		{
			Eat(rightEnPassantPawn);
		}
		if (block == leftEnPassant)
		{
			Eat(leftEnPassantPawn);
		}
		ClearEnPassant();
		lastStep = BoardManager.Instance.StepCount;
		StartCoroutine(MoveToCoroutine(block));
		return true;
	}
	protected void GetEatBlock(int rightMul, int forward)
	{
		Block block = BoardManager.Instance.GetRelativeBlock(currentBlock, rightMul, forward);
		if (block == null) return;
		ChessPiece piece = block.GetChessPiece();
		if (piece != null && piece.GetSide() != GetSide() && IsSafeToMoveTo(block)) AddEdibleBlock(block);
	}
	public int StepCount { get { return stepCount; } }
	public bool Step2 { get { return step2; } }
	private void FindEnPassant(int forward)
	{
		CheckEnPassant(-1, forward, ref leftEnPassant, ref leftEnPassantPawn);
		CheckEnPassant(1, forward, ref rightEnPassant, ref rightEnPassantPawn);
	}
	private void CheckEnPassant(int dx, int forward, ref Block enPassantBlock, ref Pawn enPassantPawn)
	{
		Block sideBlock = BoardManager.Instance.GetRelativeBlock(currentBlock, dx, 0);
		if (sideBlock == null) return;

		Pawn pawn = sideBlock.GetChessPiece() as Pawn;
		if (pawn == null) return;

		int stepDiff = BoardManager.Instance.StepCount - pawn.lastStep;
		if (pawn.GetSide() != GetSide() && pawn.StepCount == 1 && pawn.Step2 && stepDiff <= 1)
		{
			enPassantBlock = BoardManager.Instance.GetRelativeBlock(currentBlock, dx, forward);
			moveableBlocks.Add(enPassantBlock);
			edibleBlocks.Add(sideBlock);
			enPassantPawn = pawn;
		}
	}
	private void ClearEnPassant()
	{
		rightEnPassant = null;
		leftEnPassant = null;
		rightEnPassantPawn = null;
		leftEnPassantPawn = null;
	}
	public bool CanPromote()
	{
		string end = GetSide() == Side.White ? "8" : "1";
		return currentBlock.GetVertical() == end;
	}
}
