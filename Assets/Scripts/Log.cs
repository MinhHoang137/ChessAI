using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
	[SerializeField] private TMP_Text step;
	[SerializeField] private TMP_Text whiteMove;
	[SerializeField] private TMP_Text blackMove;
	public void SetStep(int stepCount)
	{
		step.text = stepCount.ToString();
	}
	public void SetWhiteMove(string move)
	{
		whiteMove.text = move;
	}
	public void SetBlackMove(string move)
	{
		blackMove.text = move;
	}
}
