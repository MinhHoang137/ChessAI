﻿using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public static BoardManager Instance { get; private set; }
	private List<ChessPiece> activePieces = new();
	private List<King> kings = new();
	[SerializeField] private Side goingSide = Side.White;
	[SerializeField] private Side playerSide = Side.White; // The side the player is playing as
	[SerializeField] private Transform cameraHolder;
	public Side WinSide { get; private set; }

	private Dictionary<string, Block> blockDict = new Dictionary<string, Block>();

	private List<Block> checkingBlocks = new(); 

	private int stepCount = 0;

	private void Awake()
	{
		Instance = this;
	}
	private void Update()
	{
		if (checkingBlocks.Count > 0)
		{
			ShowCheckingBlocks();
		}
	}
	public void AddPiece(ChessPiece piece)
	{
		activePieces.Add(piece);
		if (piece is King king)
		{
			kings.Add(king);
		}
	}
	public void RemovePiece(ChessPiece piece)
	{
		activePieces.Remove(piece);
		if (piece is King king)
		{
			kings.Remove(king);
		}
	}
	public bool IsKingInCheck(Side kingSide, ChessPiece disablePiece = null)
	{
		foreach (ChessPiece piece in activePieces)
		{
			if (piece == disablePiece) continue;
			if (piece.GetSide() != kingSide && piece is not King)
			{
				List<Block> edibleBlocks = piece.GetEdibleBlocks();
				foreach (Block block in edibleBlocks)
				{
					if (block.GetChessPiece() is King)
					{
						return true;
					}
				}
			}
		}
		return IsKingCloseToOthers(kingSide);
	}
	public bool IsCheckMate()
	{
		List<Block> blocks = new List<Block>();
		List<ChessPiece> pieces = new List<ChessPiece>();
		foreach (ChessPiece piece in activePieces) {
			if (piece.GetSide() == goingSide)
			{
				blocks.AddRange(piece.GetMoveableBlocks());
				pieces.Add(piece);
			}
		}
		//Debug.Log(blocks.Count + " " + pieces.Count);
		return blocks.Count == pieces.Count;
	}
	private void OnDestroy()
	{
		Instance = null;
	}
	private void GetCheckingBlock(Side side)
	{
		HideCheckingBlocks();
		foreach (ChessPiece piece in activePieces)
		{
			if (piece.GetSide() != side && piece is King) continue;
			List<Block> edibleBlocks = piece.GetEdibleBlocks();
			foreach (Block block in edibleBlocks)
			{
				if (block.GetChessPiece() is King)
				{
					checkingBlocks.Add(block);
					checkingBlocks.Add(piece.GetCurrentBlock());
				}
			}
		}
	}
	public void ShowCheckingBlocks()
	{
		foreach (Block block in checkingBlocks)
		{
			block.ShowRed();
		}
	}
	private void HideCheckingBlocks()
	{
		foreach (Block block in checkingBlocks)
		{
			block.HideRed();
		}
		checkingBlocks.Clear();
	}
	public void SwitchSide()
	{
		goingSide = goingSide == Side.White ? Side.Black : Side.White;
		stepCount++;
		HideCheckingBlocks();
		StartCoroutine(DelayAction.Delay(Time.deltaTime, () => { GetCheckingBlock(goingSide); }));
		StartCoroutine(DelayAction.Delay(0.4f, () => SwitchCamera()));
		if (IsCheckMate())
		{
			Debug.Log("Checkmate: " + goingSide + "loses");
			WinSide = goingSide == Side.White ? Side.Black : Side.White; // The other side wins
		}
	}
	private void SwitchCamera()
	{
		if (cameraHolder == null) return;
		//cameraHolder.Rotate(new Vector3(0, 180, 0));
	}
	public Side GoingSide { get => goingSide; }
	public Block GetBlock(string id)
	{
		if (blockDict.TryGetValue(id, out Block block))
		{
			return block;
		}
		return null;
	}
	public void AddBlock(string id, Block block)
	{
		if (blockDict.ContainsKey(id)) return;
		blockDict.Add(id, block);
	}
	private bool IsKingCloseToOthers(Side kingside)
	{
		King consider = null;
		foreach (King king in kings)
		{
			if (king.GetSide() == kingside)
			{
				consider = king;
				break; // Tìm thấy rồi thì dừng luôn
			}
		}

		if (consider == null) return false; // Tránh NullReferenceException

		Vector3 considerPos = consider.GetCurrentBlock().transform.position;
		float safeDis = Unit.Diagonal + 0.001f; 

		foreach (King king in kings)
		{
			if (king == consider) continue;
			Vector3 kingPos = king.GetCurrentBlock().transform.position;
			if (Vector3.Distance(considerPos, kingPos) < safeDis)
			{
				return true;
			}
		}
		return false;
	}
	public Block GetRelativeBlock(Block block, int dirX, int dirY)
	{
		if (block == null) return null;
		char x = block.GetId()[0];
		char y = block.GetId()[1];
		Block block1 = GetBlock($"{(char)(x + dirX)}{(char)(y + dirY)}");
		return block1;
	}
	public bool IsPlayerTurn()
	{
		return goingSide == playerSide;
	}
	public int StepCount
	{
		get => stepCount;
	}
	public List<Block> CheckingBlocks
	{
		get => checkingBlocks;
	}
	public Side PlayerSide
	{
		get => playerSide;
		set { 
			playerSide = value; 
			float angle = playerSide == Side.White ? 0 : 180; // Rotate camera based on player side
			if (cameraHolder != null)
			{
				cameraHolder.rotation = Quaternion.Euler(0, angle, 0);
			}
		}
	}
}
