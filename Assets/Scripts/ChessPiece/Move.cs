using UnityEngine;
using System;

[Serializable]
public class Move
{
    [SerializeField] private string start;
	[SerializeField] private string end;
	[SerializeField] private string promotionPiece;

	public Move() { }

	public Move(string start, string end, string promotionPiece = "")
	{
		this.start = start;
		this.end = end;
		this.promotionPiece = promotionPiece;
	}
	public string Start
	{
		get { return start; }
		set { start = value; }
	}
	public string End
	{
		get { return end; }
		set { end = value; }
	}
	public string PromotionPiece
	{
		get { return promotionPiece; }
		set { promotionPiece = value; }
	}
	public override string ToString()
	{
		return $"Move(Start: {start}, End: {end}, PromotionPiece: {promotionPiece})";
	}
}
