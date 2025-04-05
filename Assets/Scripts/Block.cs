using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private Transform chessHolder;
    [SerializeField] private GameObject moveableSign;
    [SerializeField] private GameObject edibleSign;
    [SerializeField] private GameObject selectSign;
	private string horizontal;
	private string vertical;
	private string id;
	private void Start()
	{
		StartCoroutine(DelayAction.Delay(Time.deltaTime, Register));
		
	}
	public ChessPiece GetChessPiece()
	{
		if (chessHolder.childCount == 0) return null;
		ChessPiece piece = chessHolder.GetChild(0)?.GetComponent<ChessPiece>();
		return piece;
	}
	public void ShowMoveable()
	{
		moveableSign.SetActive(true);
	}
	public void HideMoveable()
	{
		moveableSign.SetActive(false);
	}
	public void ShowRed()
	{
		edibleSign.SetActive(true);
	}
	public void HideRed()
	{
		edibleSign.SetActive(false);
	}
	public void ShowSelect()
	{
		selectSign.SetActive(true);
	}
	public void HideSelect()
	{
		selectSign.SetActive(false);
	}
	public void HideSignal(){ HideMoveable(); HideRed(); HideSelect(); }
	public Transform GetChessHolder() { return chessHolder; }
	private void OnMouseEnter()
	{
		if (ChessController.Instance.ChessPiece != null)
		{
			ShowSelect();
		}
	}
	private void OnMouseExit()
	{
		HideSelect();
	}
	private void Register()
	{
		id = horizontal + vertical;
		BoardManager.Instance.AddBlock(id, this);
	}
	public void SetHorizontal(string horizontal) { this.horizontal = horizontal; }
	public string GetHorizontal() { return horizontal; }

	public void SetVertical(string vertical) { this.vertical = vertical; }
	public string GetVertical() { return vertical; }
	public string GetId() { return id; }
}

