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

	private void Awake()
	{
		Instance = this;
		inputActions = new();
		inputActions.Player.Enable();
		inputActions.Player.Select.performed += ctx => Select();
	}
	private void Select()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, blockLayerMask))
		{
			Block block = hit.transform.GetComponent<Block>();
			if (currentPiece != null)
			{
				currentPiece.HideWay();
				if (currentPiece.MoveTo(block))
				{
					if (currentPiece is Pawn pawn && pawn.CanPromote())
					{
						OnPawnPromotion?.Invoke(this, new OnPawnPromotionEventArgs(pawn));
						SetSelectable(false);
					}
					else {
						BoardManager.Instance.SwitchSide();
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
				currentPiece = null;
				return;
			}
			ChessPiece piece = block.GetChessPiece();
			if (piece != null && piece.GetSide() == BoardManager.Instance.GoingSide)
			{
				currentPiece = piece;
				currentPiece.ShowWay();
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
	public void InvokeLog(string log) {
		OnLog?.Invoke(this, new OnLogEventArgs(log));
	}
}
