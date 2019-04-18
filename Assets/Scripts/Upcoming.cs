using System;
using UnityEngine;

namespace PipeCircles
{
	public class Upcoming : MonoBehaviour
	{
		public const string DRAGGABLE_TAG = "Draggable";
		private static int numPieces = 4;

		[Header ("Location Data")]
		[SerializeField] float pos1StartX = 0;
		[SerializeField] float pos1StartY = 0;
		[SerializeField] float pieceSpacingY = 100f;
		[SerializeField] float waterDropYOffset = 20f;
		[SerializeField] float pieceMovementTime = 1f;

		[Header ("Prefabs and Weights")]
		[SerializeField] Transform[] piecePrefab = null;
		[SerializeField] [Range (0, 10000f)] float[] pieceWeighting = null;
		[SerializeField] Transform[] waterDropPrefabs = null;

		[Header ("Other")]
		[SerializeField] Transform placedPiecesContainer = null;
		[SerializeField] Transform waterDropsContainer = null;
		[SerializeField] Transform gameBoard = null;
		[Range (0, 2000f)] [SerializeField] float distDropDestroy;

		Transform[] upcomingPieces = new Transform[numPieces];
		Transform[] waterDrops = new Transform[numPieces];

		bool moveUpcomingPieces = false;
		float currentPieceMovementTime;
		Vector3 startingPieceMovePos;
		Vector3 endingPieceMovePos;
		Vector3 startingWaterDropMovePos;
		Vector3 endingWaterDropMovePos;
		
		bool removeWaterDrop = false;
		float currentDropDisposalTime;
		Vector3 startingDropDisposalPos;
		Vector3 endingDropDisposalPos;
		Transform waterDropToRemove;

		float totalWeight;

		
		
		Vector3 waterDropToRemoveInitialPos;

		private void Start()
		{
			GenerateInitialPiecesAndWaterDrops();	
		}

		private void Update()
		{
			if (moveUpcomingPieces)
			{
				MovePiecesAndWaterDrops();
			}

			if (removeWaterDrop)
			{
				DisposeOfUsedWaterDrop();
			}
		}

		private void GenerateInitialPiecesAndWaterDrops()
		{
			for (int i = 0; i < numPieces; i++)
			{
				GenerateNewPieceAndWaterDrop();
			}
		}

		public void PieceClicked()
		{
			SavePiecePlaced();
			DisposeOfUsedWaterDrop();
			ReindexPieceAndWaterDropArrays();
			GenerateNewPieceAndWaterDrop();
			ReverseChildren();
			MovePiecesAndWaterDrops();
		}

		public void ClearArrays()
		{
			for (int i = 0; i < numPieces; i++)
			{
				upcomingPieces[i] = null;
				waterDrops[i] = null;
			}
		}

		private void GenerateNewPieceAndWaterDrop()
		{
			int openSlot = FindFirstOpenArraySlot();
			if (openSlot == numPieces) { return; }  //No open spaces in the array
			CreatePiece(openSlot);
			CreateWaterDrop(openSlot);
			SetPieceDraggable();
			RepositionPiece(openSlot);
			RepositionWaterDrops(openSlot);
		}

		private int FindFirstOpenArraySlot()
		{
			int firstOpen = numPieces;
			for (int i = numPieces - 1; i >= 0; i--)
			{
				if (upcomingPieces[i] == null)
				{
					firstOpen = i;
				}
			}
			return firstOpen;
		}

		private void CreatePiece(int openSlot)
		{
			int piecePicked = PickRandomPieceToCreate();
			upcomingPieces[openSlot] = Instantiate(piecePrefab[piecePicked],Vector3.zero,Quaternion.identity);
			upcomingPieces[openSlot].transform.SetParent(this.transform);
			upcomingPieces[openSlot].transform.SetAsLastSibling();
			upcomingPieces[openSlot].GetComponent<Animator>().SetBool("Transition", false);
		}

		private void CreateWaterDrop(int openSlot)
		{
			int waterDropPicked = UnityEngine.Random.Range(0, waterDropPrefabs.Length);
			waterDrops[openSlot] = Instantiate(waterDropPrefabs[waterDropPicked], Vector3.zero, Quaternion.identity);
			waterDrops[openSlot].transform.SetParent(waterDropsContainer);
			waterDrops[openSlot].transform.SetAsLastSibling();
		}

		private int PickRandomPieceToCreate()
		{
			totalWeight = SumPieceWeighting();
			
			if (pieceWeighting.Length <= 1) { return 0;}  //Only 1 element, so pick it

			float[] cumulativeProbabilities = new float[pieceWeighting.Length];
			cumulativeProbabilities[0] = pieceWeighting[0] / totalWeight;
			for (int i = 1; i < cumulativeProbabilities.Length; i++)
			{
				cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + pieceWeighting[i] / totalWeight;
			}
			float randNum = UnityEngine.Random.Range(0f, 1f);

			for (int i = 0; i < cumulativeProbabilities.Length; i++)
			{
				if (randNum <= cumulativeProbabilities[i])
				{
					return i;
				}
			}

			return 0; //Should be unreachable
		}

		private float SumPieceWeighting()
		{
			if (piecePrefab.Length != pieceWeighting.Length)
			{
				Debug.LogError("piecePrefabs and pieceWeighting lengths do not match");
			}

			float sum = 0;
			foreach (float weight in pieceWeighting)
			{
				sum += weight;
			}

			if (sum < 0.0001)
			{
				Debug.LogError("Nonzero values for piece weights please");
			}

			return sum;
		}

		private void SetPieceDraggable()
		{
			upcomingPieces[0].gameObject.tag = DRAGGABLE_TAG;
			for (int i = 1; i < numPieces; i++)
			{
				if (upcomingPieces[i] != null)
				{
					upcomingPieces[i].gameObject.tag = "Untagged";
				}
			}
		}

		private void RepositionPiece(int openSlot)
		{
			float yPos = pos1StartY + openSlot * pieceSpacingY;
			Vector2 newPos = new Vector2(pos1StartX,yPos);
			upcomingPieces[openSlot].transform.position = newPos;
		}

		private void RepositionWaterDrops(int openSlot)
		{
			float yPos = pos1StartY + openSlot * pieceSpacingY + waterDropYOffset;
			Vector2 newPos = new Vector2(pos1StartX, yPos);
			waterDrops[openSlot].transform.position = newPos;
		}

		private void ReindexPieceAndWaterDropArrays()
		{
			for (int i = 0; i < numPieces - 1; i++)
			{
				upcomingPieces[i] = upcomingPieces[i + 1];
				waterDrops[i] = waterDrops[i + 1];
			}
			upcomingPieces[numPieces - 1] = null;
			waterDrops[numPieces - 1] = null;
		}

		private void ReverseChildren() //So the 3rd piece looks like it is sliding off the 4th piece, not other way around
		{
			for (int i = numPieces - 1; i >= 0; i--)
			{
				upcomingPieces[i].transform.SetAsLastSibling();
				waterDrops[i].transform.SetAsLastSibling();
			}
		}

		private void MovePiecesAndWaterDrops()
		{
			if (moveUpcomingPieces)
			{
				currentPieceMovementTime += Time.deltaTime;

				if(currentPieceMovementTime > pieceMovementTime)
				{
					currentPieceMovementTime = pieceMovementTime;
					moveUpcomingPieces = false;
				}

				float t = currentPieceMovementTime / pieceMovementTime;
				t = t * t * t * (t * (6f * t - 15f) + 10f);
				upcomingPieces[0].position = Vector3.Lerp(startingPieceMovePos, endingPieceMovePos, t);
				waterDrops[0].position = Vector3.Lerp(startingWaterDropMovePos, endingWaterDropMovePos, t);

				for (int i = 1; i < numPieces - 1; i++) //Last piece was created in correct spot, no need to move it
				{
					upcomingPieces[i].position = upcomingPieces[0].position + pieceSpacingY * i * Vector3.up;
					waterDrops[i].position = waterDrops[0].position + pieceSpacingY * i * Vector3.up;
				}
			} else
			{
				if (upcomingPieces[numPieces - 1] == null || upcomingPieces[numPieces - 2] == null) { return; }

				startingPieceMovePos = upcomingPieces[0].position;
				startingWaterDropMovePos = waterDrops[0].position;
				endingPieceMovePos = startingPieceMovePos + pieceSpacingY * Vector3.down;
				endingWaterDropMovePos = startingWaterDropMovePos + pieceSpacingY * Vector3.down;

				currentPieceMovementTime = 0;
				moveUpcomingPieces = true;
			}
		}

		private void SavePiecePlaced()
		{
			Transform pieceClicked = upcomingPieces[0];
			pieceClicked.SetParent(placedPiecesContainer);
			pieceClicked.SetAsLastSibling();
			gameBoard.GetComponent<Board>().AddPieceToBoardInWorldUnits(pieceClicked);
		}

		private void DisposeOfUsedWaterDrop()
		{
			if (removeWaterDrop)
			{
				currentDropDisposalTime += Time.deltaTime;

				if (currentDropDisposalTime > pieceMovementTime)
				{
					removeWaterDrop = false;
					Destroy(waterDropToRemove.gameObject);
				}

				float t = currentDropDisposalTime / pieceMovementTime;
				t = t * t * t * (t * (6f * t - 15f) + 10f);
				if (waterDropToRemove != null) //in case it has been destroyed already
				{
					waterDropToRemove.position = Vector3.Lerp(startingDropDisposalPos, endingDropDisposalPos, t);
				}
				
			} else
			{
				waterDropToRemove = waterDrops[0];
				startingDropDisposalPos = waterDropToRemove.position;
				endingDropDisposalPos = startingDropDisposalPos + pieceSpacingY * Vector3.down;
				currentDropDisposalTime = 0;
				removeWaterDrop = true;
			}
		}
	}
}