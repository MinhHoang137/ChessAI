using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class ChessPiece : MonoBehaviour
{
    [SerializeField] private Side side;
    protected List<Ray> rays = new List<Ray>();
    protected List<Block> moveableBlocks = new List<Block>();
    protected List<Block> edibleBlocks = new List<Block>();
    [SerializeField] protected LayerMask blockLayerMask;
    protected Block currentBlock;
    protected float speed = 10;

    protected string log = "";
    [SerializeField] protected string pieceName;
    protected const string NORMAL_MOVE = "-";
	protected const string EAT_MOVE = "x";

	protected virtual void InitializeRay()
    {
        rays.Clear();
    }
    public virtual IEnumerator<WaitForSeconds> FindWayCoroutine(float rayLength = 0)
    {
		InitializeRay();
        ClearMoveable();
        moveableBlocks.Add(currentBlock);
        foreach (Ray ray in rays)
        {
            List<RaycastHit> hits = Physics.RaycastAll(ray, rayLength, blockLayerMask).OrderBy(hit => hit.distance).ToList();
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.TryGetComponent(out Block block))
                {
                    if (block == currentBlock) continue; // skip the current block
					ChessPiece piece = block.GetChessPiece();
                    if (piece != null)
                    {
                        if (piece.side != side && IsSafeToMoveTo(block)) {
                             AddEdibleBlock(block);
						}
                        break; // stop at the first piece
					}
					if (!IsSafeToMoveTo(block)) continue;
                    moveableBlocks.Add(block);
                }
            }
		}
		yield return null;
	}

	public virtual void FindWay()
    {
        StartCoroutine(FindWayCoroutine(8 * Unit.Length));
	}

    public void ShowWay()
    {
        FindWay();
        foreach (Block block in moveableBlocks)
        {
            block.ShowMoveable();
        }
        foreach (Block block in edibleBlocks)
        {
            block.ShowRed();
        }
    }
    public void HideWay()
    {
        foreach (Block block in moveableBlocks)
        {
            block.HideSignal();
        }
		foreach (Block block in edibleBlocks)
		{
			block.HideSignal();
		}
	}
    protected bool IsSafeToMoveTo(Block block)
    {
        if (side != BoardManager.Instance.GoingSide) return true;
		ChessPiece capturedPiece = block.GetChessPiece();
        Block standingBlock = currentBlock;
        SetCurrentBlock(block);
        bool isSafe = !BoardManager.Instance.IsKingInCheck(side, capturedPiece);
        SetCurrentBlock(standingBlock);
        if (capturedPiece != null)
        {
            capturedPiece.SetCurrentBlock(block);
        }
        return isSafe;
	}
    public Side GetSide() { return side; }
	/// <summary>
	/// Add a block to the edible blocks list, also to moveableBlocks. if just add to edibleBlocks, dont use this method.
	/// </summary>
	/// <param name="block"></param>
	protected void AddEdibleBlock(Block block)
    {
        moveableBlocks.Add(block);
        edibleBlocks.Add(block);
    }
	protected virtual void Register()
	{
		BoardManager.Instance.AddPiece(this);
		Vector3 origin = transform.position + transform.up;
		Vector3 direction = -transform.up;
		float distance = 5 * Unit.Length;
		if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, blockLayerMask))
		{
			if (hit.transform.TryGetComponent(out Block block))
			{
				SetCurrentBlock(block);
				transform.localPosition = Vector3.zero;
			}
		}
	}
    protected void Eat(ChessPiece piece)
	{
		BoardManager.Instance.RemovePiece(piece);
        Destroy(piece.gameObject);
	}

	protected void SetCurrentBlock(Block block)
    {
		currentBlock = block;
        transform.SetParent(block.GetChessHolder());
        transform.SetAsFirstSibling();
	}
	protected IEnumerator<WaitForSeconds> MoveToCoroutine(Block block)
	{
        if (block.GetChessPiece() != null)
		{
			Eat(block.GetChessPiece());
		}
		SetCurrentBlock(block);
		Vector3 destination = block.transform.position;
		while (Vector3.Distance(transform.position, destination) > 0.1f)
		{
			transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
        transform.localPosition = Vector3.zero;
	}
    public virtual bool MoveTo(Block block)
	{
		if (!moveableBlocks.Contains(block) || block == currentBlock)
		{
			return false;
		}
		log = NormalLog(block);
		StartCoroutine(MoveToCoroutine(block));
		return true;
	}
	public List<Block> GetEdibleBlocks() {
        try
        {
            FindWay();
        }
        catch (StackOverflowException e)
        { 
            Debug.LogWarning(name + " causes StackOverflowException: " + e.Message);
		}
		catch (Exception e)
		{
			Debug.LogWarning(name + " causes Exception: " + e.Message);
		}
		return edibleBlocks; 
    }

    public List<Block> GetMoveableBlocks()
    {
        FindWay();
        return moveableBlocks;
    }
    public string Log() { return log; }
    public Block GetCurrentBlock() { return currentBlock; }

	/// <summary>
	/// clear moveable and edible blocks list.
	/// </summary>
	protected void ClearMoveable()
    {
		moveableBlocks.Clear();
		edibleBlocks.Clear();
	}
    protected string NormalLog(Block block)
    {
		string log = pieceName + currentBlock.GetId() + (block.GetChessPiece() != null ? EAT_MOVE : NORMAL_MOVE) + block.GetId();
		return log;
	}
    public string GetName()
	{
		return pieceName;
	}
}
