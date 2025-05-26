using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const int MAX_DEPTH = 5;
	private const int MIN_DEPTH = 1;

	[SerializeField] private Toggle blackCheck;
    [SerializeField] private TextMeshProUGUI depthText;
	[SerializeField] private Button increaseDepthButton;
	[SerializeField] private Button decreaseDepthButton;
	[SerializeField] private Button StartButton;
	[SerializeField] private GameObject startUI;
	[SerializeField] private AICommunicator aiCommunicator;

	[SerializeField] private GameObject endUI;
	[SerializeField] private TextMeshProUGUI endText;
	private int depth = 1;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		ChessController.Instance.SetSelectable(false);
		increaseDepthButton.onClick.AddListener(() =>
		{
			if (depth <= MAX_DEPTH)
			{
				depth++;
				depthText.text = "Depth: " + depth;
			}
		});
		decreaseDepthButton.onClick.AddListener(() =>
		{
			if (depth >= MIN_DEPTH)
			{
				depth--;
				depthText.text = "Depth: " + depth;
			}
		});
		StartButton.onClick.AddListener(() =>
		{
			GameInfo gameInfo = new GameInfo();
			BoardManager.Instance.PlayerSide = (blackCheck.isOn ? Side.Black : Side.White);
			gameInfo.playerSide = BoardManager.Instance.PlayerSide.ToString();
			Debug.Log("Player side: " + gameInfo.playerSide);
			gameInfo.depth = depth;
			SendAIGameInfo(gameInfo);
			startUI.SetActive(false);
			ChessController.Instance.SetSelectable(true);
		});
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	private void OnDestroy()
	{
		Instance = null;
	}

	private void SendAIGameInfo(GameInfo gameInfo)
	{
		aiCommunicator.SendGameInfo(gameInfo);
	}
	public void GameOver(Side winSide)
	{
		Side playerSide = BoardManager.Instance.PlayerSide;
		endUI.SetActive(true);
		if (winSide == playerSide)
		{
			endText.text = "You Win!";
		}
		else
		{
			endText.text = "You Lose!";
		}
	}
}
