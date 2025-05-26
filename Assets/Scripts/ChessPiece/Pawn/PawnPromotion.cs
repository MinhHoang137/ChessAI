using UnityEngine;
using UnityEngine.UI;

public class PawnPromotion : MonoBehaviour
{
    [SerializeField] private Queen[] queenPrefabs;
	[SerializeField] private Rook[] rookPrefabs;
	[SerializeField] private Bishop[] bishopPrefabs;
	[SerializeField] private Knight[] knightPrefabs;
	[SerializeField] private Button queenButton;
	[SerializeField] private Button rookButton;
	[SerializeField] private Button bishopButton;
	[SerializeField] private Button knightButton;
	private Pawn pawn;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		ChessController.Instance.OnPawnPromotion += (sender, e) => { pawn = e.Pawn;
			gameObject.SetActive(true);
		};
		queenButton.onClick.AddListener(() => Promote(queenPrefabs));
		rookButton.onClick.AddListener(() => Promote(rookPrefabs));
		bishopButton.onClick.AddListener(() => Promote(bishopPrefabs));
		knightButton.onClick.AddListener(() => Promote(knightPrefabs));
		gameObject.SetActive(false);
	}
	private void Promote(ChessPiece[] prefabs)
	{
		if (pawn == null) return;
		string log = pawn.Promote(prefabs);
		ChessController.Instance.SetSelectable(true);

		// Send the move to AI
		Move move = ChessController.Instance.GetMove();
		move.PromotionPiece = prefabs[0].GetName();
		Move moveToSend = new Move(move.Start, move.End, move.PromotionPiece);
		ChessController.Instance.SendAIMove(moveToSend);

		// Hide the promotion UI
		gameObject.SetActive(false);
		BoardManager.Instance.SwitchSide();
		Debug.Log(log);
		ChessController.Instance.InvokeLog(log);
	}
	public void Promote(string pieceName, Pawn pawn)
	{
		if (pawn == null) return;
		ChessPiece[] prefabs = null;
		switch (pieceName)
		{
			case "Q":
				prefabs = queenPrefabs;
				break;
			case "R":
				prefabs = rookPrefabs;
				break;
			case "B":
				prefabs = bishopPrefabs;
				break;
			case "N":
				prefabs = knightPrefabs;
				break;
			default:
				Debug.LogWarning("Unknown piece name: " + pieceName);
				return;
		}
		string log = pawn.Promote(prefabs);
		ChessController.Instance.InvokeLog(log);
		gameObject.SetActive(false);
	}
}
