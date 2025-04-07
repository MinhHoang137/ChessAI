using UnityEngine;
using UnityEngine.UI;

public class LogUI : MonoBehaviour
{
	private int logCount = 0;
	private float height = 50;
	[SerializeField] private RectTransform logHolder;
	[SerializeField] private Log logPrefab;
	[SerializeField] private Button showButton;
	[SerializeField] private GameObject logScrollView;
	private Log log;
	void Start()
	{
		// Set the height of the log holder to fit the number of logs
		logHolder.sizeDelta = new Vector2(logHolder.sizeDelta.x, height * logCount);
		ChessController.Instance.OnLog += (sender, e) => { AddLog(e.log); };
		showButton.onClick.AddListener(() =>
		{
			logScrollView.SetActive(!logScrollView.activeSelf);
			if (logScrollView.activeSelf)
			{
				logHolder.sizeDelta = new Vector2(logHolder.sizeDelta.x, height * logCount);
			}
		});
		logScrollView.SetActive(false);
	}
	private void AddLog(string log)
	{
		if (BoardManager.Instance.StepCount % 2 == 1)
		{
			logCount++;
			Log newLog = Instantiate(logPrefab, logHolder);
			newLog.SetStep(logCount);
			newLog.SetWhiteMove(log);
			this.log = newLog;
		}
		else
		{
			this.log.SetBlackMove(log);
		}
	}
}
