using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Concurrent;

public class AICommunicator : MonoBehaviour
{
	public class MoveEventArgs : EventArgs
	{
		public Move Move { get; private set; }
		public MoveEventArgs(Move move)
		{
			Move = move;
		}
	}

	public event EventHandler<MoveEventArgs> OnAIMove;

	private Thread listenThread;
	private volatile bool isRunning = false;
	private readonly ConcurrentQueue<Move> moveQueue = new ConcurrentQueue<Move>();

	private void Start()
	{
		isRunning = true;
		listenThread = new Thread(ListenLoop);
		listenThread.IsBackground = true;
		listenThread.Start();
	}

	private void Update()
	{
		while (moveQueue.TryDequeue(out Move move))
		{
			OnAIMove?.Invoke(this, new MoveEventArgs(move));
		}
	}

	private void OnDestroy()
	{
		isRunning = false;
		listenThread?.Join(); // Wait for thread to stop
	}

	private void ListenLoop()
	{
		TcpListener listener = null;
		try
		{
			listener = new TcpListener(IPAddress.Loopback, 5051);
			listener.Start();
			Debug.Log("AICommunicator listening on 127.0.0.1:5051");

			while (isRunning)
			{
				if (!listener.Pending())
				{
					Thread.Sleep(100); // Reduce CPU usage
					continue;
				}

				using (TcpClient client = listener.AcceptTcpClient())
				using (NetworkStream stream = client.GetStream())
				{
					byte[] buffer = new byte[1024];
					int bytesRead = stream.Read(buffer, 0, buffer.Length);
					if (bytesRead <= 0) continue;

					string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					try
					{
						Move move = JsonUtility.FromJson<Move>(json);
						moveQueue.Enqueue(move);
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to parse Move JSON: " + ex.Message);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Socket error: " + ex.Message);
		}
		finally
		{
			listener?.Stop();
		}
	}
	public void SendMove(Move move, int retryCount = 3, int timeoutMillis = 2000)
	{
		Thread sendThread = new Thread(() =>
		{
			string json = JsonUtility.ToJson(move);
			int attempts = 0;
			bool sent = false;

			while (attempts < retryCount && !sent)
			{
				try
				{
					using (TcpClient client = new TcpClient())
					{
						IAsyncResult result = client.BeginConnect(IPAddress.Loopback, 5052, null, null);
						bool success = result.AsyncWaitHandle.WaitOne(timeoutMillis);

						if (!success)
						{
							throw new TimeoutException("Connection timed out.");
						}

						using (NetworkStream stream = client.GetStream())
						{
							byte[] data = Encoding.UTF8.GetBytes(json);
							stream.WriteTimeout = timeoutMillis;
							stream.Write(data, 0, data.Length);
							sent = true;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"[SendMove] Attempt {attempts + 1} failed: {ex.Message}");
					attempts++;
					Thread.Sleep(200); // Đợi một chút rồi thử lại
				}
			}

			if (!sent)
			{
				Debug.LogError("[SendMove] Failed to send move after multiple attempts.");
			}
		});

		sendThread.IsBackground = true;
		sendThread.Start();
	}

}
