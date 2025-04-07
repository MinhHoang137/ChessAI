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
		gameObject.SetActive(false);
		BoardManager.Instance.SwitchSide();
		Debug.Log(log);
		ChessController.Instance.InvokeLog(log);
	}
}
