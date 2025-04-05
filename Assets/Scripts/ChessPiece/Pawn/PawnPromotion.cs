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
		queenButton.onClick.AddListener(() => SpawnPiece(queenPrefabs));
		rookButton.onClick.AddListener(() => SpawnPiece(rookPrefabs));
		bishopButton.onClick.AddListener(() => SpawnPiece(bishopPrefabs));
		knightButton.onClick.AddListener(() => SpawnPiece(knightPrefabs));
		gameObject.SetActive(false);
	}
	private void SpawnPiece<T>(T[] prefabs) where T : ChessPiece
	{
		if (pawn == null || prefabs == null || prefabs.Length == 0) return;

		foreach (T prefab in prefabs)
		{
			if (pawn.GetSide() == prefab.GetSide())
			{
				ChessPiece newPiece = Instantiate(prefab, pawn.GetCurrentBlock().GetChessHolder());
				newPiece.transform.localPosition = Vector3.zero;
				break;
			}
		}

		BoardManager.Instance.RemovePiece(pawn);
		Destroy(pawn.gameObject);

		gameObject.SetActive(false);
		BoardManager.Instance.SwitchSide();
	}
}
