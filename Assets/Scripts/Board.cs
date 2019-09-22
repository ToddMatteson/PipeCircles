using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace PipeCircles
{
	public class Board : MonoBehaviour
	{
		#region VariableDeclaration
		public const int BOARD_UNITS_WIDE = 12;
		public const int BOARD_UNITS_HIGH = 9;
		public const int PIXELS_PER_BOARD_UNIT = 100;
		public const string UNTAGGED_TAG = "Untagged";
		public const int SPLASH_FRAMES_PER_SECOND = 30;

		[Header("Preplaced Pieces")]
		[SerializeField] [Tooltip("Actual object, not prefab")] Transform[] preplacedPieces = null;
		[SerializeField] Vector2Int[] preplacedPositions = null;
		[SerializeField] Transform preplacedPiecesHolder = null;

		[Header("Level Over")]
		[SerializeField] bool deadEndStopsLevel = false;
		
		[Header("Teleporters")]
		[SerializeField] [Tooltip("Board Location, also add to Preplaced Pieces")] Vector2Int[] teleportPos = null;
		[SerializeField] [Tooltip("Board Location to emerge from")] Vector2Int[] matchingTeleportPos = null;

		[Header("Teleporter Fountains")]
		[SerializeField] [Tooltip("Set interval to 0 to turn off")] [Range(0f, 60f)] float launchInterval = 0;
		[SerializeField] Transform projectilePrefab = null;
		[SerializeField] [Range(0, 1000f)] float dropletsPerSecond = 100f;
		[SerializeField] [Range(0, 1000)] int dropletsPerStream = 30;
		[SerializeField] Transform projectileParent = null;
		[SerializeField] bool streamJittering = true;

		[Header("Splashes")]
		[SerializeField] Sprite[] splashSprites = null;

		[Header("Water Parks")]
		[SerializeField] [Tooltip("Center square location")] Vector2Int[] waterParkPos = null;

		Transform[,] board = new Transform[BOARD_UNITS_WIDE, BOARD_UNITS_HIGH];
		Dictionary<Direction, Vector2Int> dirToVector2 = new Dictionary<Direction, Vector2Int>();
		List<List<PathTransformDirection>> pathFromStart = new List<List<PathTransformDirection>>();
		Dictionary<PathTransformDirection, AnimStatus> animState = new Dictionary<PathTransformDirection, AnimStatus>();
		List<Vector4> splitterCameFromPathIndexes = new List<Vector4>(); //working backwards to find starting path pos 
		List<bool> columnTraversed = new List<bool>();

		bool waterFlowStarted = false;
		int numSplits = 0;
		Timer timer;

		//Teleporter projectile and splash stuff
		Trajectory trajectory;
		List<Vector2Int> startingBoardPos = new List<Vector2Int>();
		List<Vector2Int> endingBoardPos = new List<Vector2Int>();
		List<Vector3> bezier1WorldPos = new List<Vector3>();
		List<Vector3> bezier2WorldPos = new List<Vector3>();
		List<Transform[]> projectiles = new List<Transform[]>();
		List<bool[]> projectileCompleted = new List<bool[]>();
		List<Vector3[]> jitterVector = new List<Vector3[]>();
		ProjectileStatus projectileStatus = ProjectileStatus.NotStarted;
		float waitingPeriodStart = 0;
		List<SplashStatus> splashStatus = new List<SplashStatus>();

		//Water Park stuff
		List<Vector3Int> parkInPathsConnected = new List<Vector3Int>();
		List<Vector3Int> parkTimesTraversed = new List<Vector3Int>();
		List<Vector4> parkPosAndInitialPathIndex = new List<Vector4>();
		#endregion VariableDeclaration

		#region Awake
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
		#endregion Awake

		private void Start()
		{
			trajectory = FindObjectOfType<Trajectory>().GetComponent<Trajectory>();
			timer = FindObjectOfType<Timer>().GetComponent<Timer>();
			waitingPeriodStart = Time.time;

			AddToVectorDictionary();
			ClearBoard();
			ClearAnimDict();
			ClearColumnTraversed();
			AddTeleporters();
			ClearAndAddWaterParks();
			AddPreplacedPiecesToBoard();
			PreplacedPieceExplosions();
		}
		
		private void Update()
		{
			WaterStarts();
			ProjectileDirector();
		}

		#region BoardSetUp
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

		private void ClearAnimDict()
		{
			animState.Clear();
		}

		private void ClearColumnTraversed ()
		{
			columnTraversed.Clear();
		}

		private void AddPreplacedPiecesToBoard()
		{
			#region ErrorChecking
			if (preplacedPieces.Length != preplacedPositions.Length)
			{
				Debug.LogError("preplacedPieces is not the same length as preplacedPositions");
			}

			if (preplacedPieces.Length == 0)
			{
				Debug.LogError("Please add a starting piece to the GameBoard");
			}
			#endregion ErrorChecking

			for(int i = 0; i < preplacedPieces.Length; i++)
			{
				if (preplacedPieces[i] == null || preplacedPositions[i] == null) { continue; } //In case a row gets skipped in the inspector, just move on to the next one
				AddPieceToBoardInBoardUnits(preplacedPieces[i], preplacedPositions[i]);
			}
		}

		private void PreplacedPieceExplosions()
		{
			for (int i = 0; i < preplacedPiecesHolder.childCount; i++)
			{
				Animator animator = preplacedPiecesHolder.GetChild(i).GetComponent<Animator>();
				if (animator == null) { continue; }
				animator.logWarnings = false;
				animator.SetTrigger("PlayVFX");
				animator.logWarnings = true;
			}
		}

		#endregion BoardSetup

		#region TeleporterSetUp
		private void AddTeleporters()
		{
			#region ClearProjectileListsRegion
			startingBoardPos.Clear();
			endingBoardPos.Clear();
			bezier1WorldPos.Clear();
			bezier2WorldPos.Clear();
			projectiles.Clear();
			projectileCompleted.Clear();
			jitterVector.Clear();
			#endregion ClearProjectileListsRegion

			#region ErrorCheckingRegion
			if (teleportPos.Length != matchingTeleportPos.Length)
			{
				Debug.LogError("teleportPos is not the same length as matchingTeleportPos");
				return;
			}

			if (teleportPos.Length == 1)
			{
				Debug.LogError("Please add the other teleporter as well");
				return;
			}
			#endregion ErrorCheckingRegion

			if (teleportPos.Length != 0)
			{
				for (int i = 0; i < teleportPos.Length; i++)
				{
					if (teleportPos[i] == null || matchingTeleportPos[i] == null) { continue; } //In case a row gets skipped in the inspector, just move on to the next one
					//teleportDict.Add(teleportPos[i], matchingTeleportPos[i]);
					if (launchInterval > 0.001f)
					{
						startingBoardPos.Add(teleportPos[i]);
						endingBoardPos.Add(matchingTeleportPos[i]);
						bezier1WorldPos.Add(trajectory.BezierCubicMiddlePos(teleportPos[i], matchingTeleportPos[i], i).middlePos1);
						bezier2WorldPos.Add(trajectory.BezierCubicMiddlePos(teleportPos[i], matchingTeleportPos[i], i).middlePos2);
						projectiles.Add(new Transform[dropletsPerStream]);
						projectileCompleted.Add(new bool[dropletsPerStream]);
						jitterVector.Add(new Vector3[dropletsPerStream]);
					}
				}
			}
		}
		#endregion TeleporterSetUp

		#region WaterParkSetUp
		private void ClearAndAddWaterParks()
		{
			parkInPathsConnected.Clear();
			for (int i = 0; i < waterParkPos.Length; i++)
			{
				parkInPathsConnected.Add(new Vector3Int(waterParkPos[i].x, waterParkPos[i].y, 0));
			}
		}
		#endregion WaterParkSetUp

		#region AddPieceToBoard
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

		private static Vector2Int CalcBoardPos(Transform transformToCheck)
		{
			int boardX = Mathf.RoundToInt(transformToCheck.position.x / PIXELS_PER_BOARD_UNIT);
			int boardY = Mathf.RoundToInt(transformToCheck.position.y / PIXELS_PER_BOARD_UNIT);
			return new Vector2Int(boardX, boardY);
		}

		private static bool CheckValidBoardPos(Vector2 boardPos)
		{
			if (boardPos.x < 0 || boardPos.x >= BOARD_UNITS_WIDE) { return false; }
			if (boardPos.y < 0 || boardPos.y >= BOARD_UNITS_HIGH) { return false; }
			return true;
		}
		#endregion AddPieceToBoard

		#region Pathing
		private List<List<PathTransformDirection>> FindPathFromStart()
		{
			numSplits = 0;
			List<List<PathTransformDirection>> path = new List<List<PathTransformDirection>>();
			splitterCameFromPathIndexes.Clear();
			ClearAndAddWaterParks();

			if (board[preplacedPositions[0].x, preplacedPositions[0].y] == null) { return path; } //No starting piece => no path
			path.Add(new List<PathTransformDirection>());
			path[0].Add(new PathTransformDirection(board[preplacedPositions[0].x, preplacedPositions[0].y], Direction.Top));

			if (columnTraversed.Count == 0) //Might already exist since pathing is called many times 
			{
				columnTraversed.Add(false);
			}

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

				#region AddColumnsToListsIfNeeded
				if (currentColumn >= path.Count) //New column needed before GetNewBoardPos checks it
				{
					path.Add(new List<PathTransformDirection>());
				}

				if (currentColumn >= columnTraversed.Count) //Setup for water traversal later, not a pathing need
				//Can't use path.Count here since path is reset often
				{
					columnTraversed.Add(false);
				}
				#endregion AddColumnsToListsIfNeeded

				Vector2Int potentialBoardPos = GetNewBoardPos(path, firstExitDirection, currentColumn, oldColumn);
				if (!CheckValidBoardPos(potentialBoardPos)) { return; } //Piece on edge of board => no more path

				Direction entranceDirection = ReverseDirection(firstExitDirection);
				Vector2Int newBoardPos;
				if (IsWithinWaterPark(potentialBoardPos))
				{
					#region WaterParkSpecific
					if (!DoesParkAcceptWaterThere(potentialBoardPos, entranceDirection)) { return; }
					int parkIndex = FindWaterParkPosIndex(potentialBoardPos);
					potentialBoardPos = waterParkPos[parkIndex]; //Changing the location to the park since the park is on the board
					IncrementWaterParkPathsIn(potentialBoardPos);

					newBoardPos = potentialBoardPos;
					if (parkInPathsConnected.Contains(new Vector3Int (potentialBoardPos.x, potentialBoardPos.y, 1)))
					{
						path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], entranceDirection));
						return; //Don't let path continue until both streams appear at the park
					} else if (parkInPathsConnected.Contains(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 2)))
					{
						path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], entranceDirection));
					} else if (parkInPathsConnected.Contains(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 3)))
					{
						path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], Direction.Nowhere));
					} else 
					{
						return;
					}
					#endregion WaterParkSpecific
				} else
				{ 
					if (board[potentialBoardPos.x, potentialBoardPos.y] == null) { return; } //No piece placed yet => no more path
					if (!board[potentialBoardPos.x, potentialBoardPos.y].GetComponent<Piece>().CanWaterEnter(entranceDirection)) { return; } //Piece is turned wrong way => no more path
					//Now have a valid piece, can add it to the path list
					newBoardPos = potentialBoardPos;
					path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], entranceDirection));
				}

				if(path[currentColumn].Count == 1) //Just added the piece after a splitter
				{
					Vector2Int currentPathIndexes = new Vector2Int(currentColumn, 0);
					if (!splitterCameFromPathIndexes.Contains(new Vector4(currentColumn, 0, oldColumn, path[oldColumn].Count - 1)))
					{
						splitterCameFromPathIndexes.Add(new Vector4(currentColumn, 0, oldColumn, path[oldColumn].Count - 1));
					}
				}

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
			#region TeleporterExplanationComments
			/*/////
			This algorithm will fail if two teleporters are placed adjacent to each other
			Don't do this!
			It would be silly anyway!
			*//////

			//There are 3 distinct teleporter related cases:
			//1. The current piece is nowhere near a teleport or is the one before a teleport
			//		Normal operation - new piece is the adjacent piece, doesn't get into special case
			//2. The current piece is the entrance teleport
			//		Special operation - new piece is the exit teleporter, use the results of the
			//			teleporter dictionary.
			//			Previous piece and current piece == adjacent
			//3. The current piece is the exit teleport
			//		Special operation - new piece is the adjacent piece, but does need to get into and out of
			//			the special case, since it would also pass the teleportDict.ContainsKey check.
			//			Previous piece and current piece != adjacent
			#endregion TeleporterExplanationComments

			int currentColumnPathIndex;
			int currentRowPathIndex;
			if (path[currentColumn].Count > 0) //Not directly after a splitter
			{
				currentColumnPathIndex = currentColumn;
				currentRowPathIndex = path[currentColumn].Count - 1;
				
			} else //Directly after a splitter
			{
				currentColumnPathIndex = oldColumn;
				currentRowPathIndex = path[oldColumn].Count - 1;
			}

			//Find standard next position
			Transform currentTransform = path[currentColumnPathIndex][currentRowPathIndex].pathTransform;
			int currentBoardPosX = CalcBoardPos(currentTransform).x;
			int currentBoardPosY = CalcBoardPos(currentTransform).y;
			int returnXValue = currentBoardPosX + dirToVector2[firstExitDirection].x;
			int returnYValue = currentBoardPosY + dirToVector2[firstExitDirection].y;
			Vector2Int currentBoardPos = new Vector2Int(currentBoardPosX, currentBoardPosY);
			Vector2Int returnValue = new Vector2Int(returnXValue, returnYValue);

			#region TeleporterSpecialCase
			//Teleporter special handling of next position
			int teleporterIndex = FindTeleporterIndex(currentBoardPos);
			if (teleportPos.Length > 0 && teleporterIndex != teleportPos.Length)
			{
				//Can assume a previous piece exists since teleporters can't be a starting piece
				Vector2Int previousPathIndexes = FindPreviousPiecePathIndexes(currentColumnPathIndex, currentRowPathIndex);
				Transform previousTransform = path[previousPathIndexes.x][previousPathIndexes.y].pathTransform;
				int previousBoardPosX = CalcBoardPos(previousTransform).x;
				int previousBoardPosY = CalcBoardPos(previousTransform).y;
				int xDiff = currentBoardPosX - previousBoardPosX;
				int yDiff = currentBoardPosY - previousBoardPosY;

				if (xDiff * yDiff == 0 && Mathf.Abs(xDiff + yDiff) == 1)
				{   //Either x or y has to be unchanged to be adjacent, so the product must be 0 
					//(1, 0) or (-1, 0) or (0, 1) or (0, -1)
					return new Vector2Int(matchingTeleportPos[teleporterIndex].x, matchingTeleportPos[teleporterIndex].y);
				}
			}
			#endregion TeleporterSpecialCase

			#region WaterParkSpecialCase
			//Water Park special handling of next position
			if(IsWithinWaterPark(currentBoardPos))
			{
				//ooo	Needs to go from x to 3 if park is done, or back to 2 if exiting
				//o2x3	So, add another in the direction of the park exit,
				//ooo	or just go back to the park location

				int parkIndex = FindWaterParkPosIndex(currentBoardPos);
				Transform parkTransform = board[waterParkPos[parkIndex].x, waterParkPos[parkIndex].y];

				Piece parkPiece = parkTransform.GetComponent<Piece>();
				Direction parkExitDir = parkPiece.GetParkExit();

				if (parkInPathsConnected.Contains(new Vector3Int(currentBoardPosX, currentBoardPosY, 2)))
				{
					return new Vector2Int(currentBoardPosX, currentBoardPosY);

				} else if (parkInPathsConnected.Contains(new Vector3Int(currentBoardPosX, currentBoardPosY, 3)))
				{
					int newXValue = returnXValue + dirToVector2[parkExitDir].x;
					int newYValue = returnYValue + dirToVector2[parkExitDir].y;
					return new Vector2Int(newXValue, newYValue);
				} //If neither 2 nor 3, something went wrong and just let it pass through the water park section
			}
			#endregion WaterParkSpecialCase

			return returnValue;
		}

		private int FindTeleporterIndex(Vector2Int currentBoardPos)
		{
			for (int i = 0; i < teleportPos.Length; i++)
			{
				if (teleportPos[i].x == currentBoardPos.x && teleportPos[i].y == currentBoardPos.y && matchingTeleportPos[i] != null) { return i; }
			}
			return teleportPos.Length;
		}

		private Vector2Int FindPreviousPiecePathIndexes(int colIndex, int rowIndex)
		{
			if (rowIndex > 0)
			{
				return new Vector2Int(colIndex, rowIndex - 1);
			} else //Must be directly after a splitter
			{
				for (int i = 0; i < BOARD_UNITS_WIDE; i++)
				{
					for (int j = 0; j < BOARD_UNITS_HIGH; j++)
					{
						if (splitterCameFromPathIndexes.Contains(new Vector4(colIndex, rowIndex, i, j)))
						{
							return new Vector2Int(i, j);
						}
					}
				}
			}

			return new Vector2Int(0, 0); //Shouldn't ever get here
		}
		
		private int FindWaterParkPosIndex(Vector2Int potentialBoardPos)
		{
			int parkIndex = -1;
			for (int i = 0; i < waterParkPos.Length; i++)
			{
				if (Mathf.Abs(potentialBoardPos.x - waterParkPos[i].x) <= 1
					&& Mathf.Abs(potentialBoardPos.y - waterParkPos[i].y) <= 1)
				{
					parkIndex = i;
				}
			}
			return parkIndex;
		}

		private static Direction ReverseDirection(Direction directionToReverse)
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

		private bool IsWithinWaterPark(Vector2Int potentialBoardPos)
		{
			//oo1		o1o
			//oxo	or	oxo
			//ooo		ooo

			for (int i = 0; i < waterParkPos.Length; i++)
			{
				if(Mathf.Abs(potentialBoardPos.x - waterParkPos[i].x) <= 1 
					&& Mathf.Abs(potentialBoardPos.y - waterParkPos[i].y) <= 1)
				{
					return true;
				}
			}

			return false;
		}

		private bool IsWithinWaterPark(Transform pieceTransform)
		{
			return IsWithinWaterPark(CalcBoardPos(pieceTransform));
		}

		private bool DoesParkAcceptWaterThere(Vector2Int potentialBoardPos, Direction entranceDirection)
		{
			int parkIndex = FindWaterParkPosIndex(potentialBoardPos);
			if (parkIndex == -1) { return false; } //Something went wrong if this happens

			if (potentialBoardPos.x != waterParkPos[parkIndex].x && potentialBoardPos.y != waterParkPos[parkIndex].y) { return false; } 
			//If get to here, in the middle position of one of the sides

			Transform parkTransform = board[waterParkPos[parkIndex].x, waterParkPos[parkIndex].y];
			Piece parkPiece = parkTransform.GetComponent<Piece>();

			if (parkPiece.GetPark1stEntrance() == Direction.Nowhere || parkPiece.GetPark2ndEntrance() == Direction.Nowhere)
			{
				return false;
			}

			if (parkPiece.GetPark1stEntrance() == entranceDirection) { return true; }
			if (parkPiece.GetPark2ndEntrance() == entranceDirection) { return true; }
			return false;
		}

		private void IncrementWaterParkPathsIn(Vector2Int potentialBoardPos)
		{
			//Replace (x, y, 2) with (x, y, 3)
			if (parkInPathsConnected.Contains(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 2)))
			{
				parkInPathsConnected.Remove(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 2));
				parkInPathsConnected.Add(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 3));
			}

			//Replace (x, y, 1) with (x, y, 2)
			if (parkInPathsConnected.Contains(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 1)))
			{
				parkInPathsConnected.Remove(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 1));
				parkInPathsConnected.Add(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 2));
			}
			
			//Replace (x, y, 0) with (x, y, 1)
			if (parkInPathsConnected.Contains(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 0)))
			{
				parkInPathsConnected.Remove(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 0));
				parkInPathsConnected.Add(new Vector3Int(potentialBoardPos.x, potentialBoardPos.y, 1));
			}
		}

		private void UpdateAnimDictionaries()
		{
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					if (!animState.ContainsKey(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Top)))
					{
						animState.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Top), AnimStatus.NotStarted);
						animState.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Right), AnimStatus.NotStarted);
						animState.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Bottom), AnimStatus.NotStarted);
						animState.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Left), AnimStatus.NotStarted);
						animState.Add(new PathTransformDirection(pathFromStart[i][j].pathTransform, Direction.Nowhere), AnimStatus.NotStarted);
					}
				}
			}
		}
		#endregion Pathing

		#region Traversal
		private void WaterStarts()
		{
			if (timer.GetTimeRemaining() > 0.02f) { return; } //Water doesn't start until timer hits zero, so nothing to traverse
			if (!waterFlowStarted)
			{
				waterFlowStarted = true;
				if (pathFromStart.Count == 0 || pathFromStart[0].Count == 0) { LevelOver(); } //No starting piece, so just kill the round
				parkTimesTraversed.Clear();
				parkPosAndInitialPathIndex.Clear();
				StartCoreAnimation(pathFromStart[0][0].pathTransform, 0, 0);
			}
		}

		private void StartCoreAnimation(Transform activePieceTransform, int colIndex, int rowIndex)
		{
			Animator animator = activePieceTransform.gameObject.GetComponent<Animator>();
			Direction startingDirection = pathFromStart[colIndex][rowIndex].direction;
			int teleporterIndex = FindTeleporterIndex(CalcBoardPos(activePieceTransform));
			bool isTeleporter = teleportPos.Length > 0 && teleporterIndex != teleportPos.Length;
			bool teleporterIn = IsTeleporterIn(activePieceTransform, isTeleporter, colIndex, rowIndex);
			bool waterPark = IsWithinWaterPark(activePieceTransform);
			TraversalCounter(activePieceTransform, waterPark);
			Direction waterParkExit = GetWaterParkExit(activePieceTransform, waterPark);
			Direction waterPark2ndDir = Get2ndDir(activePieceTransform, waterPark, startingDirection);
			//Debug.Log("StartCoreAnimation with indexes (" + colIndex.ToString() + ", " + rowIndex.ToString() + ")"
			//	+ ", startingDirection: " + startingDirection.ToString()
			//	+ ", teleporterIndex: " + teleporterIndex.ToString()
			//	+ ", isTeleporter: " + isTeleporter.ToString()
			//	+ ", teleporterIn: " + teleporterIn.ToString()
			//	//+ ", waterPark: " + waterPark.ToString()
			//	//+ ", waterParkExit: " + waterParkExit.ToString()
			//	//+ ", waterPark2ndDir: " + waterPark2ndDir.ToString()
			//	+ ", position (" + activePieceTransform.position.x.ToString() + ", " + activePieceTransform.position.y.ToString() + ")"
			//	);
			if (isTeleporter && !teleporterIn)
			{
				SetAnimatorEvents(animator, ReverseDirection(startingDirection), isTeleporter, teleporterIn, waterPark, waterParkExit, waterPark2ndDir);
			} else
			{
				SetAnimatorEvents(animator, startingDirection, isTeleporter, teleporterIn, waterPark, waterParkExit, waterPark2ndDir);
			}

			activePieceTransform.gameObject.tag = UNTAGGED_TAG; //Prevents the piece from being replaced by another
			FindObjectOfType<Scoring>().GetComponent<Scoring>().PieceTraveled();
			animState[new PathTransformDirection(pathFromStart[colIndex][rowIndex].pathTransform, startingDirection)] = AnimStatus.Started;
		}
		
		private bool IsTeleporterIn(Transform activePieceTransform, bool isTeleporter, int colIndex, int rowIndex)
		{
			/*/////
			This algorithm will fail if two teleporters are placed adjacent to each other
			Don't do this!
			It would be silly anyway!
			*//////

			if (colIndex == 0 && rowIndex == 0)	{ return true; } //Starting piece doesn't have a previous piece
			if (!isTeleporter) { return true; }

			Vector2Int previousPiecePathIndexes = FindPreviousPiecePathIndexes(colIndex, rowIndex);
			//Debug.Log("previousPiecePathIndexes X: " + previousPiecePathIndexes.x.ToString() + ", Y: " + previousPiecePathIndexes.y.ToString());

			Transform previousPieceTransform = pathFromStart[previousPiecePathIndexes.x][previousPiecePathIndexes.y].pathTransform;

			Vector2Int currentPieceBoardPos = CalcBoardPos(activePieceTransform);
			Vector2Int previousPieceBoardPos = CalcBoardPos(previousPieceTransform);
			int xDiff = currentPieceBoardPos.x - previousPieceBoardPos.x;
			int yDiff = currentPieceBoardPos.y - previousPieceBoardPos.y;

			//Debug.Log("XDiff: " + xDiff.ToString() + ", YDiff: " + yDiff.ToString());
			if (xDiff * yDiff != 0) { return false; } //Either x or y has to be unchanged to be adjacent, so the product must be 0
			if (Mathf.Abs(xDiff + yDiff) != 1) { return false; } //(1, 0) or (-1, 0) or (0, 1) or (0, -1)
			return true;
		}

		private void TraversalCounter(Transform activePieceTransform, bool waterPark) //Needed later for AnimationComplete
		{
			if (!waterPark) { return; } //Don't want standard pieces clogging up parkTimesTraversed

			Vector2Int boardPos = CalcBoardPos(activePieceTransform);

			if (parkTimesTraversed.Contains(new Vector3Int(boardPos.x, boardPos.y, 2)))
			{
				parkTimesTraversed.Add(new Vector3Int(boardPos.x, boardPos.y, 3));
				parkTimesTraversed.Remove(new Vector3Int(boardPos.x, boardPos.y, 2));
			} else if (parkTimesTraversed.Contains(new Vector3Int(boardPos.x, boardPos.y, 1)))
			{
				parkTimesTraversed.Add(new Vector3Int(boardPos.x, boardPos.y, 2));
				parkTimesTraversed.Remove(new Vector3Int(boardPos.x, boardPos.y, 1));
			} else
			{
				parkTimesTraversed.Add(new Vector3Int(boardPos.x, boardPos.y, 1));
			}
		}

		private Direction GetWaterParkExit(Transform activePieceTransform, bool waterPark)
		{
			if (!waterPark) { return Direction.Nowhere; }
			Piece activePiece = activePieceTransform.GetComponent<Piece>();
			return activePiece.GetParkExit();
		}

		private Direction Get2ndDir(Transform activePieceTransform, bool waterPark, Direction startingDirection)
		{
			if (!waterPark) { return Direction.Nowhere; }
			Piece activePiece = activePieceTransform.GetComponent<Piece>();
			if (activePiece.GetPark1stEntrance() != startingDirection) { return activePiece.GetPark1stEntrance(); }
			if (activePiece.GetPark2ndEntrance() != startingDirection) { return activePiece.GetPark2ndEntrance(); }
			return Direction.Nowhere;
		}

		private void SetAnimatorEvents(Animator animator, Direction startingDirection, bool isTeleporter, bool teleporterIn, 
			bool waterPark, Direction waterParkExit, Direction waterPark2ndDir)
		{
			animator.SetBool("LeftOrRightStart", startingDirection == Direction.Left || startingDirection == Direction.Right);
			animator.SetBool("LeftOrBottomStart", startingDirection == Direction.Left || startingDirection == Direction.Bottom);
			//animator.SetBool("In", false);

			if (isTeleporter)
			{
				animator.SetBool("In", teleporterIn);
			}

			if (waterPark)
			{
				animator.SetBool("LeftOrRightExit", waterParkExit == Direction.Left || waterParkExit == Direction.Right);
				animator.SetBool("LeftOrBottomExit", waterParkExit == Direction.Left || waterParkExit == Direction.Bottom);
				animator.SetBool("LeftOrRight2nd", waterPark2ndDir == Direction.Left || waterPark2ndDir == Direction.Right);
				animator.SetBool("LeftOrBottom2nd", waterPark2ndDir == Direction.Left || waterPark2ndDir == Direction.Bottom);
			}

			animator.SetTrigger("Transition");
		}
		#endregion Traversal

		#region Projectiles
		private void ProjectileDirector()
		{
			switch (projectileStatus)
			{
				case ProjectileStatus.NotStarted:
					LaunchTeleportProjectiles();
					break;
				case ProjectileStatus.LaunchStarted:
					LaunchTeleportProjectiles();
					MoveProjectiles();
					break;
				case ProjectileStatus.LaunchFinished:
					MoveProjectiles();
					break;
				default:
					break;
			}

			if (splashStatus.Count > 0)
			{
				ChangeSplashSprite();
			}
		}

		private void LaunchTeleportProjectiles()
		{
			if (teleportPos.Length < 2) { return; } //No teleporters => no fancy animation
			if (launchInterval < 0.001f) { return; } //Easy way to disable

			if (Time.time - waitingPeriodStart < launchInterval) { return; }

			projectileStatus = ProjectileStatus.LaunchStarted;

			for (int i = 0; i < startingBoardPos.Count; i++)
			{
				for (int j = 0; j < dropletsPerStream; j++)
				{
					if (projectiles[i][j] == null) //No need to keep instantiating new projectiles if already done
					{
						float spawningDelay = j / dropletsPerSecond;
						if (Time.time - waitingPeriodStart > launchInterval + spawningDelay)
						{
							Transform newProjectile = Instantiate(
										projectilePrefab, 
										BoardPosToWorldPos(startingBoardPos[i]),
										Quaternion.identity);
							projectiles[i][j] = newProjectile;
							projectiles[i][j].transform.SetParent(projectileParent);
							projectiles[i][j].transform.SetAsLastSibling();
							projectileCompleted[i][j] = false;
							jitterVector[i][j] = new Vector3(0, 0, 0);
						}
					}
				}
			}

			if (Time.time - waitingPeriodStart > launchInterval + (dropletsPerStream - 1) / dropletsPerSecond)
			{
				projectileStatus = ProjectileStatus.LaunchFinished;
			}
		}

		private static Vector3 BoardPosToWorldPos(Vector2Int boardPos)
		{
			float x = (float) boardPos.x * PIXELS_PER_BOARD_UNIT;
			float y = (float) boardPos.y * PIXELS_PER_BOARD_UNIT;
			float z = 0;
			return new Vector3(x, y, z);
		}

		private void MoveProjectiles()
		{
			if (launchInterval < 0.001f) { return; }

			for (int i = 0; i < startingBoardPos.Count; i++)
			{
				for (int j = 0; j < dropletsPerStream; j++)
				{
					if (projectiles[i][j] != null) //Uncreated / unlaunched projectiles ignored
					{
						float spawningDelay = j / dropletsPerSecond;
						float percentComplete = Mathf.Min(Time.time - waitingPeriodStart - launchInterval - spawningDelay, 1f);
						projectiles[i][j].position = trajectory.CalcBezierCubicPos(
								startingBoardPos[i],
								endingBoardPos[i],
								bezier1WorldPos[i],
								bezier2WorldPos[i],
								percentComplete);

						if(streamJittering)
						{
							projectiles[i][j].position = projectiles[i][j].position + jitterVector[i][j];
						}

						if (percentComplete >= 1f)
						{				
							CreateSplashAnim(endingBoardPos[i]);
							Destroy(projectiles[i][j].gameObject);
							projectiles[i][j] = null;
							projectileCompleted[i][j] = true;
						}
					}
				}
			}

			bool allProjectilesCompleted = true;
			for (int i = 0; i < startingBoardPos.Count; i++)
			{
				for (int j = 0; j < dropletsPerStream; j++)
				{
					if (!(allProjectilesCompleted && projectileCompleted[i][j]))
					{
						allProjectilesCompleted = false;
					}
				}
			}

			if(allProjectilesCompleted)
			{
				waitingPeriodStart = Time.time;
				projectileStatus = ProjectileStatus.NotStarted;
			}
		}

		private void CreateSplashAnim(Vector2Int endingPosition)
		{
			if (endingPosition == null) { return; }

			Vector3 endingPos = BoardPosToWorldPos(endingPosition);
			GameObject splashHolder = new GameObject();
			splashHolder.name = "Splash Object";
			splashHolder.transform.position = endingPos;
			
			float randRotation = UnityEngine.Random.Range(0f, 360f);
			splashHolder.transform.rotation = Quaternion.Euler(Vector3.forward * randRotation);

			SpriteRenderer renderer = splashHolder.AddComponent<SpriteRenderer>();
			renderer.sortingLayerName = "VFX";

			if (splashSprites.Length > 0)
			{
				int maxIndex = UnityEngine.Random.Range(1, splashSprites.Length);
				splashStatus.Add(new SplashStatus(splashHolder, Time.time, maxIndex));
			}

			Destroy(splashHolder, 1f);
		}

		private void ChangeSplashSprite()
		{
			for (int i = splashStatus.Count - 1; i >= 0; i--) //Going backwards because of list removal
			{
				if (splashStatus[i].spriteContainer == null) { continue; }
				GameObject splashHolder = splashStatus[i].spriteContainer;
				if (splashHolder == null) { continue; }			
				SpriteRenderer renderer = splashHolder.GetComponent<SpriteRenderer>();
				if (renderer == null) { continue; }

				float timeElapsed = Time.time - splashStatus[i].createTime;
				int spriteIndex = (int)(timeElapsed * SPLASH_FRAMES_PER_SECOND);
				if (spriteIndex > splashStatus[i].maxFrames)
				{
					splashStatus.RemoveAt(i);
					Destroy(splashHolder);
					continue;
				}

				if (spriteIndex < splashSprites.Length)
				{
					renderer.sprite = splashSprites[spriteIndex];
				}
			}
		}

		#endregion Projectiles

		#region AnimationComplete
		public void AnimationComplete(PathTransformDirection completedAnim)
		{
			Vector2Int completedIndexes = FindCompletedPathIndexes(completedAnim);
			int activeColumn = completedIndexes.x;
			int activeRow = completedIndexes.y;
			animState[completedAnim] = AnimStatus.Done;
			Piece activePiece = completedAnim.pathTransform.GetComponent<Piece>();
			Direction secondExitDirection = activePiece.SecondExitDirection();
			bool isSplitter = secondExitDirection != Direction.Nowhere;
			bool isWaterPark = IsWithinWaterPark(completedAnim.pathTransform);

			bool primaryElementMissing = false;
			bool secondaryElementMissing = false;

			//Debug.Log("AnimationComplete path index (" + activeColumn.ToString()
			//	+ ", " + activeRow.ToString()
			//	+ "), isSplitter is " + isSplitter.ToString()
			//	+ ", isWaterPark is " + isWaterPark.ToString());

			if (!isSplitter && !isWaterPark)
			{
				#region NormalStuff

				if (activeRow + 1 < pathFromStart[activeColumn].Count) //Check for next element existance
				{
					StartCoreAnimation(pathFromStart[activeColumn][activeRow + 1].pathTransform, activeColumn, activeRow + 1);
				} else
				{
					if (deadEndStopsLevel)
					{
						LevelOver();
						return;
					} else
					{
						columnTraversed[activeColumn] = true;
					}
				}
				#endregion NormalStuff

			} else if (isSplitter)
			{
				#region SplitterStuff
				//Primary path
				if (activeRow + 1 < pathFromStart[activeColumn].Count) //Check for next primary element
				{
					StartCoreAnimation(pathFromStart[activeColumn][activeRow + 1].pathTransform, activeColumn, activeRow + 1);
				} else
				{
					primaryElementMissing = true;
				}

				//Secondary path
				int currentXPos = CalcBoardPos(pathFromStart[activeColumn][activeRow].pathTransform).x;
				int currentYPos = CalcBoardPos(pathFromStart[activeColumn][activeRow].pathTransform).y;
				int newBoardPosX = currentXPos + dirToVector2[secondExitDirection].x;
				int newBoardPosY = currentYPos + dirToVector2[secondExitDirection].y;
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

				if ((primaryElementMissing || secondaryElementMissing) && deadEndStopsLevel)
				{
					LevelOver();
					return;
				}

				if (primaryElementMissing && secondaryElementMissing && !deadEndStopsLevel) //Checking deadEndStopsLevel here to avoid calling LevelOver twice
				{
					columnTraversed[activeColumn] = true;
				}
				#endregion SplitterStuff

			} else
			{
				#region WaterParkStuff
				Vector2Int boardPos = CalcBoardPos(completedAnim.pathTransform);

				if (parkTimesTraversed.Contains(new Vector3Int(boardPos.x, boardPos.y, 3)))
				{
					FirstPathNextPiece firstPathNextPiece = DoesFirstPathHaveNextPiece(boardPos);
					bool secondPathHasNextPiece = activeRow + 1 < pathFromStart[activeColumn].Count;

					if (firstPathNextPiece.hasNextPiece) //Check for next piece existance along first path to water park
					{
						int nextColumn = firstPathNextPiece.pathIndexes.x;
						int nextRow = firstPathNextPiece.pathIndexes.y;
						Transform newTransform = pathFromStart[nextColumn][nextRow].pathTransform;
						StartCoreAnimation(newTransform, nextColumn, nextRow);
					} else if (secondPathHasNextPiece) //Check for next piece existance along second path to water park
					{
						Transform newTransform = pathFromStart[activeColumn][activeRow + 1].pathTransform;
						StartCoreAnimation(newTransform, activeColumn, activeRow + 1);
					} else
					{
						if (deadEndStopsLevel)
						{
							LevelOver();
							return;
						} else
						{
							columnTraversed[activeColumn] = true;
						}
					}
				} else if (parkTimesTraversed.Contains(new Vector3Int(boardPos.x, boardPos.y, 2)))
				{
					//Debug.Log("Length of pathFromStart[0]: " + pathFromStart[0].Count().ToString());
					//Debug.Log("Length of pathFromStart[1]: " + pathFromStart[1].Count().ToString());
					StartCoreAnimation(completedAnim.pathTransform, activeColumn, activeRow);
					columnTraversed[activeColumn] = true;
				} else if (parkTimesTraversed.Contains(new Vector3Int(boardPos.x, boardPos.y, 1)))
				{
					parkPosAndInitialPathIndex.Add(new Vector4(boardPos.x, boardPos.y, activeColumn, activeRow));
				}
				#endregion WaterParkStuff
			}

			bool allColumnsTraversed = true;
			for (int i = 0; i < columnTraversed.Count; i++)
			{
				if (!(columnTraversed[i] && allColumnsTraversed))
				{
					allColumnsTraversed = false;
				}
			}

			if (allColumnsTraversed)
			{
				LevelOver();
				return;
			}
		}

		private Vector2Int FindCompletedPathIndexes(PathTransformDirection completedAnim)
		{
			float tolerance = 1f;
			int completedColumn = -1;
			int completedRow = -1;
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					bool xMatch = Math.Abs(pathFromStart[i][j].pathTransform.position.x - completedAnim.pathTransform.position.x) < tolerance;
					bool yMatch = Math.Abs(pathFromStart[i][j].pathTransform.position.y - completedAnim.pathTransform.position.y) < tolerance;
					bool dirMatch = pathFromStart[i][j].direction == completedAnim.direction;
					bool matchesComplete = xMatch && yMatch && dirMatch;

					int teleporterIndex = FindTeleporterIndex(CalcBoardPos(completedAnim.pathTransform));
					bool isTeleporter = teleportPos.Length > 0 && teleporterIndex != teleportPos.Length;

					if (isTeleporter)
					{
						PathTransformDirection animCheck = new PathTransformDirection(pathFromStart[i][j].pathTransform, pathFromStart[i][j].direction);
						bool animMatch = animState.ContainsKey(animCheck) && animState[animCheck] == AnimStatus.Started;
						if (matchesComplete && animMatch)
						{
							return new Vector2Int(i, j);
						}
					} else //Don't need the extra check here because water parks gunk up stuff by using more complicated animations
					{
						if (matchesComplete)
						{
							return new Vector2Int(i, j);
						}
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

		private FirstPathNextPiece DoesFirstPathHaveNextPiece(Vector2Int boardPos)
		{
			int pathOffset = 2;
			for (int i = 0; i < pathFromStart.Count; i++)
			{
				for (int j = 0; j < pathFromStart[i].Count; j++)
				{
					if (parkPosAndInitialPathIndex.Contains(new Vector4(boardPos.x, boardPos.y, i, j)))
					{
						bool hasPiece = pathFromStart[i].Count >= j + pathOffset;
						Vector2Int pathIndexes = new Vector2Int(i, j + pathOffset);
						return new FirstPathNextPiece(hasPiece, pathIndexes);
					}
				}
			}
			return new FirstPathNextPiece(false, new Vector2Int(0, 0));
		}
		
		#endregion AnimationComplete

		#region LevelOver
		private void LevelOver()
		{
			Piece[] allPieces = FindObjectsOfType<Piece>();
			foreach (Piece piece in allPieces)
			{
				piece.gameObject.tag = UNTAGGED_TAG;
			}
            //TODO What to do about a piece being dragged? Just delete it? Shouldn't charge player for it as an unused piece

            //Bring up the round scoring explanation screen
            RoundCanvas roundCanvas = GameObject.FindGameObjectWithTag("RoundOver").GetComponent<RoundCanvas>();
            roundCanvas.ShowRoundOverScreen();
		}
		#endregion LevelOver
	}

	#region Structs
	public struct PathTransformDirection
	{
		public Transform pathTransform;
		public Direction direction;

		public PathTransformDirection (Transform pathTransforms, Direction dir)
		{
			pathTransform = pathTransforms;
			direction = dir;
		}
	}

	public struct PathIndexesDirection
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

	public struct SplashStatus
	{
		public GameObject spriteContainer;
		public float createTime;
		public int maxFrames;
		
		public SplashStatus (GameObject spriteContain, float creationTime, int maxFrame)
		{
			spriteContainer = spriteContain;
			createTime = creationTime;
			maxFrames = maxFrame;
		}
	}

	public struct FirstPathNextPiece
	{
		public bool hasNextPiece;
		public Vector2Int pathIndexes;

		public FirstPathNextPiece(bool haveNextPiece, Vector2Int pathBoardIndexes)
		{
			hasNextPiece = haveNextPiece;
			pathIndexes = pathBoardIndexes;
		}
	}

	#endregion Structs

	#region Enums
	public enum AnimStatus { NotStarted, Started, Done }

	public enum ProjectileStatus { NotStarted, LaunchStarted, LaunchFinished }
	#endregion Enums
}