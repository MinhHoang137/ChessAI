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
	public static ChessController Instance { get; private set; }
	[SerializeField] private LayerMask blockLayerMask;

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
		
		if (Physics.Raycast(ray, out RaycastHit hit, 999, blockLayerMask))
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
					}
					else BoardManager.Instance.SwitchSide();
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
		BoardManager.Instance.ShowCheckingBlocks();
	}
	private void Update()
	{
		Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 999, Color.red);
	}
	public ChessPiece ChessPiece { get => currentPiece; }
}
