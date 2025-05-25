using UnityEngine;
using System;

[Serializable]
public class Move
{
    [SerializeField] private string start;
	[SerializeField] private string end;
	[SerializeField] private string promotionPiece;

	public Move(string start, string end, string promotionPiece = "")
	{
		this.start = start;
		this.end = end;
		this.promotionPiece = promotionPiece;
	}
	public string Start
	{
		get { return start; }
	}
	public string End
	{
		get { return end; }
	}
	public string PromotionPiece
	{
		get { return promotionPiece; }
	}
}
