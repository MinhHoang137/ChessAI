using UnityEngine;
using System;

public class ChessController : MonoBehaviour
{
	public class OnPawnPromotionEventArgs : EventArgs
	{
		public Pawn Pawn { get; private set; }
		public OnPawnPromotionEventArgs(Pawn pawn)
		{
			Pawn = pawn;
		}
	}
	public event EventHandler<OnPawnPromotionEventArgs> OnPawnPromotion;

	public class OnLogEventArgs : EventArgs
	{
		public string log;
		public OnLogEventArgs(string log)
		{
			this.log = log;
		}
	}
	public event EventHandler<OnLogEventArgs> OnLog;

	public static ChessController Instance { get; private set; }
	[SerializeField] private LayerMask blockLayerMask;
	private float rayDistance = 999;

	private InputSystem_Actions inputActions;
	private ChessPiece currentPiece = null;

	private BoardManager BoardManager => BoardManager.Instance;

	[SerializeField] private PawnPromotion pawnPromotion;
	[SerializeField] private AICommunicator aiCommunicator;
	private Move move = new();

	private void Awake()
	{
		Instance = this;
		inputActions = new();
		inputActions.Player.Enable();
		inputActions.Player.Select.performed += ctx => Select();
	}
	private void Start()
	{
		aiCommunicator.OnAIMove += (sender, e) => MoveOpponent(e.Move);
	}
	public Move GetMove() {  return move; }
	private void Select()
	{
		if (!BoardManager.IsPlayerTurn()) return;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, blockLayerMask))
		{
			Block block = hit.transform.GetComponent<Block>();
			if (currentPiece != null)
			{
				currentPiece.HideWay();
				if (currentPiece.MoveTo(block))
				{
					move.End = block.GetId();
					if (currentPiece is Pawn pawn && pawn.CanPromote())
					{
						OnPawnPromotion?.Invoke(this, new OnPawnPromotionEventArgs(pawn));
						SetSelectable(false);
					}
					else
					{
						Move moveToSend = new Move(move.Start, move.End);
						SendAIMove(moveToSend);
						BoardManager.Instance.SwitchSide();
						Log();
					}
				}
				currentPiece = null;
				return;
			}
			ChessPiece piece = block.GetChessPiece();
			if (piece != null && piece.GetSide() == BoardManager.Instance.GoingSide)
			{
				currentPiece = piece;
				currentPiece.ShowWay();
				move.Start = currentPiece.GetCurrentBlock().GetId();
			}
		}
		else
		{
			if (currentPiece != null)
			{
				currentPiece.HideWay();
				currentPiece = null;
			}
		}
	}
	public void SendAIMove(Move move)
	{
		aiCommunicator?.SendMove(move);
	}
	private void Update()
	{
		Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * rayDistance, Color.red);
	}
	public ChessPiece ChessPiece { get => currentPiece; }
	public void SetSelectable(bool value)
	{
		if (value)
		{
			inputActions.Player.Select.Enable();
		}
		else
		{
			inputActions.Player.Select.Disable();
		}
	}
	public void InvokeLog(string log)
	{
		OnLog?.Invoke(this, new OnLogEventArgs(log));
	}

	private void MoveOpponent(Move move)
	{
		Block startBlock = BoardManager.Instance.GetBlock(move.Start);
		Block endBlock = BoardManager.Instance.GetBlock(move.End);
		if (startBlock == null || endBlock == null)
		{
			Debug.LogError("Invalid move: " + move);
			return;
		}
		currentPiece = startBlock.GetChessPiece();
		if (currentPiece == null)
		{
			Debug.LogError("No piece at start block: " + move.Start);
			return;
		}
		currentPiece.FindWay();
		if (currentPiece.MoveTo(endBlock))
		{
			if (currentPiece is Pawn pawn && pawn.CanPromote())
			{
				pawnPromotion.Promote(move.PromotionPiece, pawn);
				//SetSelectable(false);
			}
			else
			{
				Log();
			}
			BoardManager.Instance.SwitchSide();
		}
		else
		{
			Debug.LogError("Move failed: " + move);
		}
		currentPiece = null;
	}
	private void Log()
	{
		string log = currentPiece.Log();
		StartCoroutine(DelayAction.Delay(2 * Time.deltaTime, () =>
		{
			if (BoardManager.Instance.CheckingBlocks.Count >= 3)
			{
				log += "++";
			}
			else if (BoardManager.Instance.CheckingBlocks.Count > 0)
			{
				log += "+";
			}
			InvokeLog(log);
		}));
	}
}
