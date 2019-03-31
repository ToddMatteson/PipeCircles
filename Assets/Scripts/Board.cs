﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace PipeCircles
{
	public class Board : MonoBehaviour
	{
		private const int BOARD_UNITS_WIDE = 12;
		private const int BOARD_UNITS_HIGH = 9;
		private const int PIXELS_PER_BOARD_UNIT = 100;

		Transform[,] board = new Transform[BOARD_UNITS_WIDE, BOARD_UNITS_HIGH];

		Vector2Int boardStart = new Vector2Int(3, 4);

		Dictionary<Direction, Vector2Int> dirToVector2 = new Dictionary<Direction, Vector2Int>();

		private void Awake()
		{
			SingletonPattern();
		}

		private void SingletonPattern()
		{
			int classCount = FindObjectsOfType<Board>().Length;
			if (classCount > 1)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			} else
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		private void Start()
		{
			AddToDictionary();
			ClearBoard();
		}

		private void AddToDictionary()
		{
			dirToVector2.Add(Direction.Top, new Vector2Int(0, 1));
			dirToVector2.Add(Direction.Right, new Vector2Int(1, 0));
			dirToVector2.Add(Direction.Bottom, new Vector2Int(0, -1));
			dirToVector2.Add(Direction.Left, new Vector2Int(-1, 0));
			dirToVector2.Add(Direction.Nowhere, new Vector2Int(0, 0));
		}

		public void ClearBoard()
		{
			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					board[i, j] = null;
				}
			}
		}

		public void AddPieceToBoard(Transform transformToAdd) 
		{
			Vector2Int boardPos = CalcBoardPos(transformToAdd);
			if (CheckValidBoardPos(boardPos))
			{
				board[boardPos.x, boardPos.y] = transformToAdd;
			} else
			{
				Debug.LogError("Attempted to add invalid transform to game board");
			}

			FindPathFromStart();
		}

		private Vector2Int CalcBoardPos(Transform transformToCheck)
		{
			int boardX = Mathf.RoundToInt(transformToCheck.position.x / PIXELS_PER_BOARD_UNIT);
			int boardY = Mathf.RoundToInt(transformToCheck.position.y / PIXELS_PER_BOARD_UNIT);
			return new Vector2Int(boardX, boardY);
		}

		private bool CheckValidBoardPos(Vector2 boardPos)
		{
			if (boardPos.x < 0 || boardPos.x >= BOARD_UNITS_WIDE) { return false; }
			if (boardPos.y < 0 || boardPos.y >= BOARD_UNITS_HIGH) { return false; }
			return true;
		}

		private List<Transform> FindPathFromStart()
		{
			bool done = false;
			List<Transform> pathFromStart = new List<Transform>();
			if (board[boardStart.x, boardStart.y] == null) { return pathFromStart; } //No starting piece => no path

			pathFromStart.Add(board[boardStart.x, boardStart.y]);
			Direction exitDirection = pathFromStart[0].GetComponent<Piece>().GetTopGoesWhere(); //TODO read in the start piece's exit direction
			while (!done)
			{
				if (exitDirection == Direction.Nowhere) { return pathFromStart; } //Water doesn't exit => no more path

				int newBoardPosX = dirToVector2[exitDirection].x + CalcBoardPos(pathFromStart[pathFromStart.Count - 1]).x;
				int newBoardPosY = dirToVector2[exitDirection].y + CalcBoardPos(pathFromStart[pathFromStart.Count - 1]).y;
				Vector2 newBoardPos = new Vector2(newBoardPosX, newBoardPosY);

				if (!CheckValidBoardPos(newBoardPos)) { return pathFromStart; } //Piece on edge of board => no more path
				if (board[newBoardPosX, newBoardPosY] == null) { return pathFromStart; } //No piece placed yet => no more path

				Direction entranceDirection = ReverseDirection(exitDirection); //Top of one piece is the bottom of the one above it
				if (!board[newBoardPosX, newBoardPosY].GetComponent<Piece>().CanWaterEnter(entranceDirection)) { return pathFromStart; } //Piece is turned wrong way => no more path

				//Now have a valid piece, can add it to the path list
				pathFromStart.Add(board[newBoardPosX, newBoardPosY]);
				exitDirection = board[newBoardPosX, newBoardPosY].GetComponent<Piece>().WhereWaterExits(entranceDirection);
			}
			return pathFromStart; //Should be unreachable
		}

		private void TraversePath()
		{
			if (FindObjectOfType<Timer>().GetTimeRemaining() > 0.02f) { return; } //Water doesn't start until timer hits zero, so nothing to traverse

		}

		private Direction ReverseDirection(Direction directionToReverse)
		{
			switch (directionToReverse)
			{
				case Direction.Top:
					return Direction.Bottom;
				case Direction.Right:
					return Direction.Left;
				case Direction.Bottom:
					return Direction.Top;
				case Direction.Left:
					return Direction.Right;
				case Direction.Nowhere:
					return Direction.Nowhere;
				default:
					return Direction.Nowhere;
			}
		}
	}
}