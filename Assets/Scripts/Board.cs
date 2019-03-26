using System.Collections;
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

		Dictionary<Direction, Vector2Int> directionToVector2 = new Dictionary<Direction, Vector2Int>();

		List<Transform> pathFromStart = new List<Transform>();

		// Singleton pattern
		private void Awake()
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
			directionToVector2.Add(Direction.Top, new Vector2Int(0, 1));
			directionToVector2.Add(Direction.Right, new Vector2Int(1, 0));
			directionToVector2.Add(Direction.Bottom, new Vector2Int(0, -1));
			directionToVector2.Add(Direction.Left, new Vector2Int(-1, 0));
			directionToVector2.Add(Direction.Nowhere, new Vector2Int(0, 0));
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
			Vector2Int boardPos = DetermineBoardPos(transformToAdd);
			if (CheckValidBoardPos(boardPos))
			{
				board[boardPos.x, boardPos.y] = transformToAdd;
			} else
			{
				Debug.LogError("Attempted to add invalid transform to game board");
			}
		}

		private Vector2Int DetermineBoardPos(Transform transformToAdd)
		{
			int boardX = Mathf.RoundToInt(transformToAdd.position.x / PIXELS_PER_BOARD_UNIT);
			int boardY = Mathf.RoundToInt(transformToAdd.position.y / PIXELS_PER_BOARD_UNIT);
			return new Vector2Int(boardX, boardY);
		}

		private bool CheckValidBoardPos(Vector2 boardPos)
		{
			if (boardPos.x < 0 || boardPos.x >= BOARD_UNITS_WIDE) { return false; }
			if (boardPos.y < 0 || boardPos.y >= BOARD_UNITS_HIGH) { return false; }
			return true;
		}

		private void FindPathFromStart()
		{
			if (board[boardStart.x, boardStart.y] != null)
			{
				pathFromStart.Add(board[boardStart.x, boardStart.y]);
			}
			Direction exitDirection = pathFromStart[0].GetComponent<Piece>().GetTopGoesWhere(); //TODO make it so we read in the start piece's exit direction
			
			
			//See if the direction heading leads to an existing piece
				//if no existing piece, path is finished
				//if existing piece check if it can accept from that direction
					//if can't exist, do not add to the path
					//if can exist, add to the path
						//determine the new direction heading
			//See if the direction heading leads ...

		}
	}
}