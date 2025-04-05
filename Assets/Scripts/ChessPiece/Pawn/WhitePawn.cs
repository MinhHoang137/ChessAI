using UnityEngine;

public class WhitePawn : Pawn
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Register();
    }
	protected override void FindWay()
	{
		int forward = 1;
        PawnFindWay(forward);
	}
}
