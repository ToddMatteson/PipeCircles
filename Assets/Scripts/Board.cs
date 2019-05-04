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
		Dictionary<Vector2Int, Vector2Int> teleportDict = new Dictionary<Vector2Int, Vector2Int>(); //Board pos
		Dictionary<Vector2Int, Vector2Int> splitterCameFromPathIndexes = new Dictionary<Vector2Int, Vector2Int>(); //working backwards to find starting path pos 
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
		Dictionary<Vector2Int, int> parkInPathsConnected = new Dictionary<Vector2Int, int>();
		Dictionary<Vector2Int, int> parkTimesTraversed = new Dictionary<Vector2Int, int>();

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
			AddPreplacedPiecesToBoard();
			AddTeleporters();
			AddWaterParks();
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
		#endregion BoardSetup

		#region TeleporterSetUp
		private void AddTeleporters()
		{
			teleportDict.Clear();

			#region ClearProjectileListsRegion
			startingBoardPos.Clear();
			endingBoardPos.Clear();
			bezier1WorldPos.Clear();
			bezier2WorldPos.Clear();
			projectiles.Clear();
			projectileCompleted.Clear();
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
					teleportDict.Add(teleportPos[i], matchingTeleportPos[i]);
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
		private void AddWaterParks()
		{
			parkInPathsConnected.Clear();
			for (int i = 0; i < waterParkPos.Length; i++)
			{
				parkInPathsConnected.Add(waterParkPos[i], 0);
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
			parkInPathsConnected.Clear();

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

				if (currentColumn >= path.Count) //New column needed before GetNewBoardPos checks it
				{
					path.Add(new List<PathTransformDirection>());
				}

				if (currentColumn >= columnTraversed.Count) //Can't use path.Count here since path is reset often
				{
					columnTraversed.Add(false);
				}

				Vector2Int potentialBoardPos = GetNewBoardPos(path, firstExitDirection, currentColumn, oldColumn);
				if (!CheckValidBoardPos(potentialBoardPos)) { return; } //Piece on edge of board => no more path

				Direction entranceDirection = ReverseDirection(firstExitDirection);
				if (IsWithinWaterPark(potentialBoardPos))
				{ //Water park
					if (!DoesParkAcceptWaterThere(potentialBoardPos, entranceDirection)) { return; }
					int parkIndex = FindWaterParkPosIndex(potentialBoardPos);
					potentialBoardPos = waterParkPos[parkIndex]; //Changing the location to the park since the park is on the board
					parkInPathsConnected[potentialBoardPos] = parkInPathsConnected[potentialBoardPos] + 1;

					//Don't let path continue until both streams appear at the park
					if (parkInPathsConnected[waterParkPos[parkIndex]] == 1) { return; }
				} else
				{ //Not a water park
					if (board[potentialBoardPos.x, potentialBoardPos.y] == null) { return; } //No piece placed yet => no more path
					if (!board[potentialBoardPos.x, potentialBoardPos.y].GetComponent<Piece>().CanWaterEnter(entranceDirection)) { return; } //Piece is turned wrong way => no more path
				}

				//Now have a valid piece, can add it to the path list
				Vector2Int newBoardPos = potentialBoardPos;
				path[currentColumn].Add(new PathTransformDirection(board[newBoardPos.x, newBoardPos.y], entranceDirection));

				if(path[currentColumn].Count == 1) //Just added the piece after a splitter
				{
					Vector2Int currentPathIndexes = new Vector2Int(currentColumn, 0);
					if (!splitterCameFromPathIndexes.ContainsKey(currentPathIndexes))
					{
						splitterCameFromPathIndexes.Add(currentPathIndexes, new Vector2Int(oldColumn, path[oldColumn].Count - 1));
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
			if (teleportDict.ContainsKey(currentBoardPos))
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
					return new Vector2Int(teleportDict[currentBoardPos].x, teleportDict[currentBoardPos].y);
				}
			}
			#endregion TeleporterSpecialCase

			#region WaterParkSpecialCase
			//Water Park special handling of next position
			if(IsWithinWaterPark(currentBoardPos))
			{
				//oooN	Needs to go from x to N when the ReturnValue would place it at P
				//oxP	So, add another in the direction of the firstExitDirection,
				//ooo	Then, adjust the orthogonal direction by 1 if exit slot is 1 or 3, none if 2

				int parkIndex = FindWaterParkPosIndex(currentBoardPos);
				Transform parkTransform = board[waterParkPos[parkIndex].x, waterParkPos[parkIndex].y];
				Piece parkPiece = parkTransform.GetComponent<Piece>();
				Direction parkExitDir = parkPiece.GetParkExit();
				int parkExitSlot = parkPiece.GetParkExitSlot();
				int parkXValue = returnXValue + dirToVector2[firstExitDirection].x;
				int parkYValue = returnYValue + dirToVector2[firstExitDirection].y;

				if (parkExitSlot == 2)
				{
					return new Vector2Int(parkXValue, parkYValue);
				}

				if(parkExitSlot == 1)
				{
					switch (firstExitDirection)
					{
						case Direction.Right:
							parkYValue++;
							break;
						case Direction.Left:
							parkYValue--;
							break;
						case Direction.Top:
							parkXValue--;
							break;
						case Direction.Bottom:
							parkXValue++;
							break;
						default:
							break;
					}
				} else
				{
					switch (firstExitDirection)
					{
						case Direction.Right:
							parkYValue--;
							break;
						case Direction.Left:
							parkYValue++;
							break;
						case Direction.Top:
							parkXValue++;
							break;
						case Direction.Bottom:
							parkXValue--;
							break;
						default:
							break;
					}
				}
				return new Vector2Int(parkXValue, parkYValue);
			}
			#endregion WaterParkSpecialCase

			return returnValue;
		}

		private Vector2Int FindPreviousPiecePathIndexes(int colIndex, int rowIndex)
		{
			if (rowIndex > 0)
			{
				return new Vector2Int(colIndex, rowIndex - 1);
			} else //Must be directly after a splitter
			{
				return splitterCameFromPathIndexes[new Vector2Int(colIndex, rowIndex)];
			}
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
			if (parkIndex == -1)
			{ return false; } //Something went wrong if this happens

			int entranceSlot = 0;

			entranceSlot = FindParkSlot(potentialBoardPos, parkIndex, entranceDirection);

			if (entranceSlot == 0)
			{ return false; } //Something went wrong if this happens

			Transform parkTransform = board[waterParkPos[parkIndex].x, waterParkPos[parkIndex].y];
			Piece parkPiece = parkTransform.GetComponent<Piece>();

			if (parkPiece.GetPark1stEntrance() == Direction.Nowhere || parkPiece.GetPark2ndEntrance() == Direction.Nowhere)
			{
				return false;
			}

			if (parkPiece.GetPark1stEntrance() == entranceDirection)
			{
				if (entranceSlot == parkPiece.GetParkEntrance1Slot())
				{
					return true;
				}
			}

			if (parkPiece.GetPark2ndEntrance() == entranceDirection)
			{
				if (entranceSlot == parkPiece.GetParkEntrance2Slot())
				{
					return true;
				}
			}

			return false;
		}

		private int FindParkSlot(Vector2Int potentialBoardPos, int parkIndex, Direction entranceDirection)
		{
			if (potentialBoardPos.x == waterParkPos[parkIndex].x
							|| potentialBoardPos.y == waterParkPos[parkIndex].y)
			{ // Middle slot
				return 2;
			} else //Corner slot, each corner could be either slot 1 or 3, depending on the entrance direction
			{
				if (potentialBoardPos.x < waterParkPos[parkIndex].x && potentialBoardPos.y < waterParkPos[parkIndex].y)
				{ //Lower left corner
					if (entranceDirection == Direction.Bottom)
					{
						return 3;
					} else
					{
						return 1;
					}
				}
				if (potentialBoardPos.x < waterParkPos[parkIndex].x && potentialBoardPos.y > waterParkPos[parkIndex].y)
				{ //Upper left corner
					if (entranceDirection == Direction.Top)
					{
						return 1;
					} else
					{
						return 3;
					}
				}
				if (potentialBoardPos.x > waterParkPos[parkIndex].x && potentialBoardPos.y < waterParkPos[parkIndex].y)
				{ //Lower right corner
					if (entranceDirection == Direction.Bottom)
					{
						return 1;
					} else
					{
						return 3;
					}
				}
				if (potentialBoardPos.x > waterParkPos[parkIndex].x && potentialBoardPos.y > waterParkPos[parkIndex].y)
				{ //Upper right corner
					if (entranceDirection == Direction.Top)
					{
						return 3;
					} else
					{
						return 1;
					}
				}
			}
			return 0;
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
				StartCoreAnimation(pathFromStart[0][0].pathTransform, 0, 0);
			}
		}

		private void StartCoreAnimation(Transform activePieceTransform, int colIndex, int rowIndex)
		{
			Animator animator = activePieceTransform.gameObject.GetComponent<Animator>();
			Direction startingDirection = pathFromStart[colIndex][rowIndex].direction;
			bool teleporterIn = IsTeleporterIn(activePieceTransform, colIndex, rowIndex);
			bool waterPark = IsWithinWaterPark(activePieceTransform);
			int waterParkSlot = GetWaterParkSlot(activePieceTransform, startingDirection);
			bool waterParkIn = TraversalCounter(activePieceTransform);
			SetAnimatorEvents(animator, startingDirection, teleporterIn, waterPark, waterParkSlot, waterParkIn);
			activePieceTransform.gameObject.tag = UNTAGGED_TAG; //Prevents the piece from being replaced by another
			FindObjectOfType<Scoring>().GetComponent<Scoring>().PieceTraveled();
			animState[new PathTransformDirection(pathFromStart[colIndex][rowIndex].pathTransform, startingDirection)] = AnimStatus.Started;
		}

		private bool IsTeleporterIn(Transform activePieceTransform, int colIndex, int rowIndex)
		{
			/*/////
			This algorithm will fail if two teleporters are placed adjacent to each other
			Don't do this!
			It would be silly anyway!
			*//////

			if (colIndex == 0 && rowIndex == 0)	{ return true; } //Starting piece doesn't have a previous piece

			Vector2Int previousPiecePathIndexes = FindPreviousPiecePathIndexes(colIndex, rowIndex);
			
			Transform previousPieceTransform = pathFromStart[previousPiecePathIndexes.x][previousPiecePathIndexes.y].pathTransform;

			Vector2Int currentPieceBoardPos = CalcBoardPos(activePieceTransform);
			Vector2Int previousPieceBoardPos = CalcBoardPos(previousPieceTransform);
			int xDiff = currentPieceBoardPos.x - previousPieceBoardPos.x;
			int yDiff = currentPieceBoardPos.y - previousPieceBoardPos.y;

			if (xDiff * yDiff != 0) //Either x or y has to be unchanged to be adjacent, so the product must be 0
			{
				return false; 
			}
			if (Mathf.Abs(xDiff + yDiff) != 1) //(1, 0) or (-1, 0) or (0, 1) or (0, -1)
			{
				return false;
			}

			return true;
		}

		private int GetWaterParkSlot(Transform activePieceTransform, Direction startingDirection)
		{
			if(!IsWithinWaterPark(activePieceTransform)) { return 0; }
			Vector2Int boardPos = CalcBoardPos(activePieceTransform);
			int parkIndex = FindWaterParkPosIndex(boardPos);
			return FindParkSlot(boardPos, parkIndex, startingDirection);
		}

		private bool TraversalCounter(Transform activePieceTransform)
		{
			Vector2Int boardPos = CalcBoardPos(activePieceTransform);
			if (parkTimesTraversed.ContainsKey(boardPos))
			{
				parkTimesTraversed[boardPos] = parkTimesTraversed[boardPos] + 1;
			} else
			{
				parkTimesTraversed.Add(boardPos, 1);
			}
			return parkTimesTraversed[boardPos] <= 2;
		}

		private void SetAnimatorEvents(Animator animator, Direction startingDirection, bool teleporterIn, 
			bool waterPark, int waterParkSlot, bool waterParkIn)
		{
			Direction exitingDirection = ReverseDirection(startingDirection);
			if (teleporterIn)
			{
				animator.SetBool("LeftOrRightStart", startingDirection == Direction.Left || startingDirection == Direction.Right);
				animator.SetBool("LeftOrBottomStart", startingDirection == Direction.Left || startingDirection == Direction.Bottom);
				animator.SetBool("In", true);
			} else if (waterPark)
			{
				animator.SetBool("LeftOrRightStart", startingDirection == Direction.Left || startingDirection == Direction.Right);
				animator.SetBool("LeftOrBottomStart", startingDirection == Direction.Left || startingDirection == Direction.Bottom);
				animator.SetBool("In", waterParkIn);

				if (waterParkSlot == 1)
				{
					animator.SetBool("FirstSlot", true);
				} else
				{
					animator.SetBool("FirstSlot", false);
				}

				if (waterParkSlot == 2)
				{
					animator.SetBool("SecondSlot", true);
				} else
				{
					animator.SetBool("SecondSlot", false);
				}

				if (waterParkSlot == 3)
				{
					animator.SetBool("ThirdSlot", true);
				} else
				{
					animator.SetBool("ThirdSlot", false);
				}
			} else
			{//Standard
				animator.SetBool("LeftOrRightStart", exitingDirection == Direction.Left || exitingDirection == Direction.Right);
				animator.SetBool("LeftOrBottomStart", exitingDirection == Direction.Left || exitingDirection == Direction.Bottom);
				animator.SetBool("In", false);
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
			if (teleportDict.Count < 2) { return; } //No teleporters => no fancy animation
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
		public void AnimationComplete(Transform transformAnimComplete, Direction startingDirection)
		{
			Vector2Int completedIndexes = FindCompletedPathIndexes(transformAnimComplete, startingDirection);
			int activeColumn = completedIndexes.x;
			int activeRow = completedIndexes.y;
			PathTransformDirection completedAnim = new PathTransformDirection(pathFromStart[activeColumn][activeRow].pathTransform, startingDirection);
			animState[completedAnim] = AnimStatus.Done;
			Transform activeTransform = completedAnim.pathTransform;
			Piece activePiece = activeTransform.GetComponent<Piece>();
			Direction secondExitDirection = activePiece.SecondExitDirection();
			bool isSplitter = secondExitDirection != Direction.Nowhere;

			bool primaryElementMissing = false; 
			bool secondaryElementMissing = false;

			if (!isSplitter)
			{
				if (activeRow + 1 < pathFromStart[activeColumn].Count) //Check for next element existance
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

				if (primaryElementMissing || secondaryElementMissing)
				{
					if (deadEndStopsLevel)
					{
						LevelOver();
						return;
					}
				}

				if (primaryElementMissing && secondaryElementMissing)
				{
					if (!deadEndStopsLevel) //Checking here to avoid calling LevelOver twice
					{
						columnTraversed[activeColumn] = true;
					}
				}
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
		
		private Vector2Int FindCompletedPathIndexes(Transform transformAnimComplete, Direction startingDirection)
		{
			PathTransformDirection completedAnim = new PathTransformDirection(transformAnimComplete, startingDirection);

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
			//TODO Bring up the round scoring explanation screen
		}
		#endregion LevelOver
	}

	#region Structs
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

	struct SplashStatus
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
	#endregion Structs

	#region Enums
	public enum AnimStatus { NotStarted, Started, Done }

	public enum ProjectileStatus { NotStarted, LaunchStarted, LaunchFinished }
	#endregion Enums

}