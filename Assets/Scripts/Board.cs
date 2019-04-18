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
		[SerializeField] [Tooltip ("Actual object, not prefab")] Transform[] preplacedPieces = null;
		[SerializeField] Vector2Int[] preplacedPositions = null;

		[Header("Teleporters")]
		[SerializeField] [Tooltip("Board Location, also add to Preplaced Pieces")] Vector2Int[] teleportPos = null;
		[SerializeField] [Tooltip("Board Location to emerge from")] Vector2Int[] matchingTeleportPos = null;

		Transform[,] board = new Transform[BOARD_UNITS_WIDE, BOARD_UNITS_HIGH];
		Dictionary<Direction, Vector2Int> dirToVector2 = new Dictionary<Direction, Vector2Int>();
		List<List<PathTransformDirection>> pathFromStart = new List<List<PathTransformDirection>>();
		Dictionary<PathTransformDirection, bool> animStartedDict = new Dictionary<PathTransformDirection, bool>();
		Dictionary<PathTransformDirection, bool> animDoneDict = new Dictionary<PathTransformDirection, bool>();
		Dictionary<Vector2Int, Vector2Int> teleportDict = new Dictionary<Vector2Int, Vector2Int>();

		bool waterFlowStarted = false;
		int numSplits = 0;
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
			AddToVectorDictionary();
			ClearBoard();
			ClearAnimDicts();
			AddPreplacedPiecesToBoard();
			AddTeleporters();
			timer = FindObjectOfType<Timer>().GetComponent<Timer>();
		}
		
		private void Update()
		{
			pathFromStart = FindPathFromStart();
			WaterStarts();
		}

		private void AddToVectorDictionary()
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

		private void ClearAnimDicts()
		{
			animStartedDict.Clear();
			animDoneDict.Clear();
		}

		private void AddPreplacedPiecesToBoard()
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

		private void AddTeleporters()
		{
			teleportDict.Clear();

			if (teleportPos.Length != matchingTeleportPos.Length)
			{
				Debug.LogError("teleportPos is not the same length as matchingTeleportPos");
				return;
			}

			if (teleportPos.Length == 1)
			{
				Debug.LogError("Please add the other teleport as well");
				return;
			}

			if (teleportPos.Length != 0)
			{
				for (int i = 0; i < teleportPos.Length; i++)
				{
					if (teleportPos[i] == null || matchingTeleportPos[i] == null) { continue; } //In case a row gets skipped in the inspector, just move on to the next one
					teleportDict.Add(teleportPos[i], matchingTeleportPos[i]);
				}
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

		private List<List<PathTransformDirection>> FindPathFromStart()
		{
			numSplits = 0;
			List<List<PathTransformDirection>> path = new List<List<PathTransformDirection>>();
			if (board[preplacedPositions[0].x, preplacedPositions[0].y] == null) { return path; } //No starting piece => no path
			path.Add(new List<PathTransformDirection>());
			path[0].Add(new PathTransformDirection(board[preplacedPositions[0].x, preplacedPositions[0].y], Direction.Top));

			FindColumnPath(path, Direction.Nowhere, 0);
			UpdateAnimDictionaries();
			return path;
		}

		private void FindColumnPath(List<List<PathTransformDirection>> path, Direction secondaryExitDirection, int oldColumn)
		{
			int currentColumn = numSplits;
			Direction firstExitDirection = FirstEntranceConditions(path, secondaryExitDirection);
			Direction secondExitDirection = SecondEntranceConditions(path);
			bool done = false;
			while (!done)
			{
				if (firstExitDirection == Direction.Nowhere) { return; } //Water doesn't exit => no more path
				if (secondExitDirection != Direction.Nowhere) //Splitter detected
				{
					numSplits++;
					FindColumnPath(path, secondExitDirection, currentColumn);
				}

				if (currentColumn >= path.Count) //New column needed before GetNewBoardPos checks it
				{
					path.Add(new List<PathTransformDirection>());
				}

				Vector2Int newBoardPos = GetNewBoardPos(path, firstExitDirection, currentColumn, oldColumn);
			
				if (!CheckValidBoardPos(newBoardPos)) { return; } //Piece on edge of board => no more path
				if (board[newBoardPos.x, newBoardPos.y] == null) { return; } //No piece placed yet => no more path

				Direction entranceDirection = ReverseDirection(firstExitDirection);
				if (!board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().CanWaterEnter(entranceDirection)) { return; } //Piece is turned wrong way => no more path

				//Now have a valid piece, can add it to the path list
				path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], entranceDirection));
				firstExitDirection = board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().FirstExitDirection(entranceDirection);
				secondExitDirection = board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().SecondExitDirection();
			}
		}

		private Direction FirstEntranceConditions(List<List<PathTransformDirection>> path, Direction secondaryExitDirection)
		{
			if(numSplits == 0)
			{
				return path[0][0].pathTransform.GetComponent<Piece>().GetTopLeads();
			} else
			{
				return secondaryExitDirection;
			}
		}

		private Direction SecondEntranceConditions(List<List<PathTransformDirection>> path)
		{
			if (numSplits == 0)
			{
				return path[0][0].pathTransform.GetComponent<Piece>().SecondExitDirection();
			} else
			{
				return Direction.Nowhere;
			}
		}

		private Vector2Int GetNewBoardPos(List<List<PathTransformDirection>> path, Direction firstExitDirection, int currentColumn, int oldColumn)
		{
			int currentBoardPosX;
			int currentBoardPosY;

			if (path[currentColumn].Count > 0) //Not directly after a splitter
			{
				currentBoardPosX = CalcBoardPos(path[currentColumn][path[currentColumn].Count - 1].pathTransform).x;
				currentBoardPosY = CalcBoardPos(path[currentColumn][path[currentColumn].Count - 1].pathTransform).y;
			} else //Directly after a splitter
			{
				currentBoardPosX = CalcBoardPos(path[oldColumn][path[oldColumn].Count - 1].pathTransform).x;
				currentBoardPosY = CalcBoardPos(path[oldColumn][path[oldColumn].Count - 1].pathTransform).y;
			}

			//Teleporter special case
			Vector2Int currentBoardPos = new Vector2Int(currentBoardPosX, currentBoardPosY);
			if (teleportDict.ContainsKey(currentBoardPos))
			{
				return new Vector2Int (teleportDict[currentBoardPos].x, teleportDict[currentBoardPos].y);
			}

			return new Vector2Int(currentBoardPosX + dirToVector2[firstExitDirection].x, currentBoardPosY + dirToVector2[firstExitDirection].y);
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

		private void UpdateAnimDictionaries()
		{
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					if (!animStartedDict.ContainsKey(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Top)))
					{
						animStartedDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Top), false);
						animStartedDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Right), false);
						animStartedDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Bottom), false);
						animStartedDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Left), false);
						animDoneDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Top), false);
						animDoneDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Right), false);
						animDoneDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Bottom), false);
						animDoneDict.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Left), false);
					}
				}
			}
		}

		private void WaterStarts()
		{	//Imagine a cross getting traversed from orthogonal directions at almost the same time.
			//Does this mean that I might have to do additional GIMP animations for this?
			//What about water coming from both ends of a simple curve piece? Does this cause issues for pathfinding, too? 

			if (timer.GetTimeRemaining() > 0.02f) { return; } //Water doesn't start until timer hits zero, so nothing to traverse
			if (!waterFlowStarted)
			{
				waterFlowStarted = true;
				if (pathFromStart.Count == 0 || pathFromStart[0].Count == 0) { LevelOver(); } //No starting piece, so just kill the round
				StartCoreAnimation(pathFromStart[0][0].pathTransform, 0, 0);
			}
		}

		private void StartCoreAnimation(Transform activePieceTransform, int colIndex, int rowIndex)
		{
			Animator animator = activePieceTransform.gameObject.GetComponent<Animator>();
			Direction startingDirection = pathFromStart[colIndex][rowIndex].direction;
			SetAnimatorEvents(animator, startingDirection);
			activePieceTransform.gameObject.tag = UNTAGGED_TAG; //Prevents the piece from being replaced by another
			FindObjectOfType<Scoring>().GetComponent<Scoring>().PieceTraveled();
			animStartedDict[new PathTransformDirection(pathFromStart[colIndex][rowIndex].pathTransform, startingDirection)] = true;
		}

		private void SetAnimatorEvents(Animator animator, Direction startingDirection)
		{
			animator.SetBool("LeftOrRightStart", startingDirection == Direction.Left || startingDirection == Direction.Right);
			animator.SetBool("LeftOrBottomStart", startingDirection == Direction.Left || startingDirection == Direction.Bottom);
			animator.SetTrigger("Transition");
		}

		private List<PathTransformDirection> FindCurrentlyActiveAnimations()
		{
			List<PathTransformDirection> activeAnimations = new List<PathTransformDirection>();
			foreach (KeyValuePair<PathTransformDirection, bool> i in animStartedDict)
			{
				if (animDoneDict.ContainsKey(i.Key) && i.Value != animDoneDict[i.Key])
				{
					activeAnimations.Add(i.Key);
				}
			}

			return activeAnimations;
		}

		public void AnimationComplete(Transform transformAnimComplete, Direction startingDirection)
		{
			Vector2Int completedIndexes = FindCompletedIndexes(transformAnimComplete, startingDirection);
			int activeColumn = completedIndexes.x;
			int activeRow = completedIndexes.y;
			PathTransformDirection completedAnim = new PathTransformDirection(pathFromStart[activeColumn][activeRow].pathTransform, startingDirection);
			animDoneDict[completedAnim] = true;
			Transform activeTransform = completedAnim.pathTransform;
			Piece activePiece = activeTransform.GetComponent<Piece>();
			Direction secondExitDirection = activePiece.SecondExitDirection();
			bool isSplitter = secondExitDirection != Direction.Nowhere;

			bool primaryElementMissing = false; //Could possibly use this to call LevelOver, but that depends on game rules
			bool secondaryElementMissing = false; //Could possibly use this to call LevelOver, but that depends on game rules

			//^^^^ DEBUG purposes
			//print("Active Column: " + activeColumn.ToString());
			//print("Active Row: " + activeRow.ToString());
			//print("Length of main path: " + pathFromStart[0].Count);
			//print("Number of columns in pathFromStart: " + pathFromStart.Count);
			//print("Is Splitter: " + isSplitter.ToString());

			if (!isSplitter)
			{
				if (activeRow + 1 < pathFromStart[activeColumn].Count) //Check for next element
				{
					Transform newTransform = pathFromStart[activeColumn][activeRow + 1].pathTransform;
					StartCoreAnimation(newTransform, activeColumn, activeRow + 1);
				} else
				{
					//LevelOver(); //Could be level over here depending on the rules I want to use but very likely not
				}
			} else
			{
				//Primary path
				if (activeRow + 1 < pathFromStart[activeColumn].Count) //Check for next primary element
				{
					Transform newTransform = pathFromStart[activeColumn][activeRow + 1].pathTransform;
					StartCoreAnimation(newTransform, activeColumn, activeRow + 1);
				} else
				{
					primaryElementMissing = true;
				}

				//Secondary path
				int currentXPos = CalcBoardPos(pathFromStart[activeColumn][activeRow].pathTransform).x;
				int newBoardPosX = dirToVector2[secondExitDirection].x + currentXPos;
				int currentYPos = CalcBoardPos(pathFromStart[activeColumn][activeRow].pathTransform).y;
				int newBoardPosY = dirToVector2[secondExitDirection].y + currentYPos;
				Vector2Int newBoardPos = new Vector2Int(newBoardPosX, newBoardPosY);

				if(CheckValidBoardPos(newBoardPos)
				   && board[newBoardPos.x, newBoardPos.y] != null
				   && board[newBoardPos.x, newBoardPos.y].GetComponent<Piece>().CanWaterEnter(ReverseDirection(secondExitDirection))) 
				{   
					//Valid piece now exists at the second exit direction
					Direction entranceDirection = ReverseDirection(secondExitDirection);
					PathTransformDirection newLocation = new PathTransformDirection(board[newBoardPosX, newBoardPosY], entranceDirection);
					Vector2Int newSpot = FindNewPathIndexes(newLocation);
					Transform transformToStartAnim = pathFromStart[newSpot.x][newSpot.y].pathTransform;
					StartCoreAnimation(transformToStartAnim, newSpot.x, newSpot.y);
				} else
				{
					secondaryElementMissing = true;
				}

				if (primaryElementMissing && secondaryElementMissing)
				{
					//TODO Maybe call LevelOver here, unknown at this point
				}
			}
		}
		
		private Vector2Int FindCompletedIndexes(Transform transformAnimComplete, Direction startingDirection)
		{
			List<PathTransformDirection> activeAnimations = FindCurrentlyActiveAnimations();
			PathTransformDirection completedAnim = new PathTransformDirection(transformAnimComplete, startingDirection);

			if (!activeAnimations.Contains(completedAnim))
			{
				Debug.LogError("Completed animation not found in active animations list");
			}

			int completedColumn = 0;
			int completedRow = 0;
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					if (pathFromStart[i][j].pathTransform == completedAnim.pathTransform
						&& pathFromStart[i][j].direction == completedAnim.direction)
					{
						completedColumn = i;
						completedRow = j;
					}
				}
			}
			return new Vector2Int(completedColumn, completedRow);
		}

		private Vector2Int FindNewPathIndexes(PathTransformDirection newLocation) {
			int newPathColumn = 0;
			int newPathRow = 0;
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					if (pathFromStart[i][j].pathTransform == newLocation.pathTransform
						&& pathFromStart[i][j].direction == newLocation.direction)
					{
						newPathColumn = i;
						newPathRow = j;
					}
				}
			}
			return new Vector2Int(newPathColumn, newPathRow);
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
	}

	struct PathTransformDirection
	{
		public Transform pathTransform;
		public Direction direction;

		public PathTransformDirection (Transform pathTransforms, Direction dir)
		{
			pathTransform = pathTransforms;
			direction = dir;
		}
	}

	struct PathIndexesDirection
	{
		public int columnIndex;
		public int rowIndex;
		public Direction direction;

		public PathIndexesDirection (int col, int row, Direction dir)
		{
			columnIndex = col;
			rowIndex = row;
			direction = dir;
		}
	}
}