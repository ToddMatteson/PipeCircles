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
		private const string UNTAGGED_TAG = "Untagged";

		[Header("Preplaced Pieces")]
		[SerializeField] Transform[] preplacedPieces;
		[SerializeField] Vector2Int[] preplacedPositions;

		Transform[,] board = new Transform[BOARD_UNITS_WIDE, BOARD_UNITS_HIGH];
		Dictionary<Direction, Vector2Int> dirToVector2 = new Dictionary<Direction, Vector2Int>();
		List<List<Transform>> pathFromStart = new List<List<Transform>>();
		bool animationComplete = true;
		int numSplits = 0;
		int waterFlowIndex = 0;
		Timer timer;

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
			AddPreplacedPieces();
			timer = FindObjectOfType<Timer>().GetComponent<Timer>();
		}

		private void Update()
		{
			pathFromStart = FindPathFromStart();
			WaterFlows();
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

		private void AddPreplacedPieces()
		{
			if (preplacedPieces.Length != preplacedPositions.Length)
			{
				Debug.LogError("preplacedPieces is not the same length as preplacedPositions");
			}

			if (preplacedPieces.Length == 0)
			{
				Debug.LogError("Please add a starting piece to the GameBoard");
			}

			for(int i = 0; i < preplacedPieces.Length; i++)
			{
				if (preplacedPieces[i] == null || preplacedPositions[i] == null) { continue; } //In case a row gets skipped in the inspector, just move on to the next one
				AddPieceToBoardInBoardUnits(preplacedPieces[i], preplacedPositions[i]);
			}
		}

		public void AddPieceToBoardInWorldUnits(Transform transformToAdd) 
		{
			Vector2Int boardPos = CalcBoardPos(transformToAdd);
			AddPieceToBoardInBoardUnits(transformToAdd, boardPos);
		}

		public void AddPieceToBoardInBoardUnits(Transform transformToAdd, Vector2Int boardPos)
		{
			if (CheckValidBoardPos(boardPos))
			{
				board[boardPos.x, boardPos.y] = transformToAdd;
			} else
			{
				Debug.LogError("Attempted to add invalid transform to game board");
			}

			pathFromStart = FindPathFromStart();
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

		private List<List<Transform>> FindPathFromStart()
		{
			List<List<Transform>> pathFromStart = new List<List<Transform>>();

			if (board[preplacedPositions[0].x, preplacedPositions[0].y] == null) { return pathFromStart; } //No starting piece => no path
			pathFromStart[0].Add(board[preplacedPositions[0].x, preplacedPositions[0].y]);

			FindColumnPath(pathFromStart, Direction.Nowhere, 0);
			return pathFromStart;
		}

		private void FindColumnPath(List<List<Transform>> pathFromStart, Direction secondaryExitDirection, int oldColumn)
		{
			int currentColumn = numSplits;
			Direction firstExitDirection = FirstEntranceConditions(pathFromStart, secondaryExitDirection);
			Direction secondExitDirection = SecondEntranceConditions(pathFromStart);
			bool done = false;
			while (!done)
			{
				if (firstExitDirection == Direction.Nowhere) { return; } //Water doesn't exit => no more path
				if (secondExitDirection != Direction.Nowhere) //Splitter detected
				{
					numSplits++;
					FindColumnPath(pathFromStart, secondExitDirection, currentColumn);
				}

				Vector2Int newBoardPos = GetNewBoardPos(firstExitDirection, currentColumn, oldColumn);
			
				if (!CheckValidBoardPos(newBoardPos)) { return; } //Piece on edge of board => no more path
				if (board[newBoardPos.x, newBoardPos.y] == null) { return; } //No piece placed yet => no more path

				Direction entranceDirection = ReverseDirection(firstExitDirection);
				if (!board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().CanWaterEnter(entranceDirection)) { return; } //Piece is turned wrong way => no more path

				//Now have a valid piece, can add it to the path list
				pathFromStart[currentColumn].Add(board[newBoardPos.x, newBoardPos.y]);
				firstExitDirection = board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().FirstExitDirection(entranceDirection);
				secondExitDirection = board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().SecondExitDirection();
			}
		}

		private Direction FirstEntranceConditions(List<List<Transform>> pathFromStart, Direction secondaryExitDirection)
		{
			if(numSplits == 0)
			{
				return pathFromStart[0][0].GetComponent<Piece>().GetTopLeads();
			} else
			{
				return secondaryExitDirection;
			}
		}

		private Direction SecondEntranceConditions(List<List<Transform>> pathFromStart)
		{
			if (numSplits == 0)
			{
				return pathFromStart[0][0].GetComponent<Piece>().SecondExitDirection();
			} else
			{
				return Direction.Nowhere;
			}
		}

		private Vector2Int GetNewBoardPos(Direction firstExitDirection, int currentColumn, int oldColumn)
		{
			int newBoardPosX;
			int newBoardPosY;
			if (pathFromStart[currentColumn].Count > 0)
			{
				newBoardPosX = dirToVector2[firstExitDirection].x + CalcBoardPos(pathFromStart[currentColumn][pathFromStart[currentColumn].Count - 1]).x;
				newBoardPosY = dirToVector2[firstExitDirection].y + CalcBoardPos(pathFromStart[currentColumn][pathFromStart[currentColumn].Count - 1]).y;
			} else
			{
				newBoardPosX = dirToVector2[firstExitDirection].x + CalcBoardPos(pathFromStart[oldColumn][pathFromStart[oldColumn].Count - 1]).x;
				newBoardPosY = dirToVector2[firstExitDirection].y + CalcBoardPos(pathFromStart[oldColumn][pathFromStart[oldColumn].Count - 1]).y;
			}
			return new Vector2Int(newBoardPosX, newBoardPosY);
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

		private void WaterFlows()
		{
			if (timer.GetTimeRemaining() > 0.02f) { return; } //Water doesn't start until timer hits zero, so nothing to traverse
			if (!animationComplete) { return; } //Only want one instance of WaterFlows to be waiting on an animation to finish

			if (waterFlowIndex < pathFromStart[0].Count)  //Remove reference to [0] to return to pre-splitter
			{
				Transform activePieceTransform = pathFromStart[0][waterFlowIndex];  //Remove reference to [0] to return to pre-splitter
				Animator animator = activePieceTransform.gameObject.GetComponent<Animator>();
				Direction startingDirection = FindStartingDirection(activePieceTransform);
				SetAnimatorEvents(animator, startingDirection);
				activePieceTransform.gameObject.tag = UNTAGGED_TAG; //Prevents the piece from being replaced by another
				FindObjectOfType<Scoring>().GetComponent<Scoring>().PieceTraveled();
				animationComplete = false;
				waterFlowIndex++;
			} else
			{
				LevelOver();
			}
		}

		private void LevelOver()
		{
			Piece[] allPieces = FindObjectsOfType<Piece>();
			foreach (Piece piece in allPieces)
			{
				piece.gameObject.tag = UNTAGGED_TAG;
			}
			//TODO What to do about a piece being dragged? Just delete it? Shouldn't charge player for it as an unused piece
			//TODO Bring up the round scoring explanation screen
		}

		private Direction FindStartingDirection(Transform activePieceTransform)
		{
			if (waterFlowIndex == 0)
			{
				return Direction.Top; //TODO fix this
			} else
			{
				Transform previousPieceTransform = pathFromStart[0][waterFlowIndex - 1]; //Remove reference to [0] to return to pre-splitter
				Vector2 previousPieceBoardPos = CalcBoardPos(previousPieceTransform);
				Vector2 activePieceBoardPos = CalcBoardPos(activePieceTransform);
				Vector2 differenceBoardPos = activePieceBoardPos - previousPieceBoardPos;

				if (differenceBoardPos.x == 1)
				{
					return Direction.Left;
				} else if (differenceBoardPos.x == -1)
				{
					return Direction.Right;
				} else if (differenceBoardPos.y == 1)
				{
					return Direction.Bottom;
				} else if (differenceBoardPos.y == -1)
				{
					return Direction.Top;
				} else
				{
					Debug.LogError("startingDirection unknown");
					return Direction.Nowhere;
				}
			}
		}

		private void SetAnimatorEvents(Animator animator, Direction startingDirection)
		{
			animator.SetBool("LeftOrRightStart", startingDirection == Direction.Left || startingDirection == Direction.Right);
			animator.SetBool("LeftOrBottomStart", startingDirection == Direction.Left || startingDirection == Direction.Bottom);
			animator.SetTrigger("Transition");
		}

		public void AnimationComplete()
		{
			animationComplete = true;
		}
	}
}