using System;
using UnityEngine;

namespace PipeCircles
{
	public class Upcoming : MonoBehaviour
	{
		public const string DRAGGABLE_TAG = "Draggable";

		[SerializeField] int numPieces = 4;
		[SerializeField] float pos1StartX = 0;
		[SerializeField] float pos1StartY = 0;
		[SerializeField] float pieceSpacingY = 100f;
		[SerializeField] float moveSpeed = 100f;

		[SerializeField] Transform[] piecePrefab;
		[SerializeField] [Range (0, 10000f)] float[] pieceWeighting;

		[SerializeField] Transform placedPiecesContainer;
		[SerializeField] Transform gameBoard;

		Transform[] upcomingPieces = new Transform[4];

		bool moveUpcomingPieces = false;
		float totalWeight;

		private void Start()
		{
			GenerateInitialPieces();	
		}

		private void Update()
		{
			if (moveUpcomingPieces)
			{
				MoveUpcomingTransforms();
			}
		}

		private void GenerateInitialPieces()
		{
			for (int i = 0; i < numPieces; i++)
			{
				GenerateNewPiece();
			}
		}

		public void PieceClicked()
		{
			SavePiecePlaced();
			ReindexPieceArray();
			GenerateNewPiece();
			ReverseChildren();
			MoveUpcomingTransforms();
		}

		private void GenerateNewPiece()
		{
			int openSlot = FindFirstOpenArraySlot();
			if (openSlot == numPieces) { return; }  //No open spaces in the array
			CreatePiece(openSlot);
			SetDraggable();
			RepositionPiece(openSlot);
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

			return 0; //should be unreachable
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

		private void SetDraggable()
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
			float yPos = pos1StartY - openSlot * pieceSpacingY;
			Vector2 newPos = new Vector2(pos1StartX,yPos);
			upcomingPieces[openSlot].transform.position = newPos;
		}

		private void ReindexPieceArray()
		{
			for (int i = 0; i < numPieces - 1; i++)
			{
				upcomingPieces[i] = upcomingPieces[i + 1];
			}
			upcomingPieces[numPieces - 1] = null;
		}

		private void ReverseChildren() //So the 3rd piece looks like it is sliding off the 4th piece, not other way around
		{
			for (int i = numPieces - 1; i >= 0; i--)
			{
				upcomingPieces[i].transform.SetAsLastSibling();
			}
		}

		private void MoveUpcomingTransforms() //Without a full array, nothing moves
		{
			if (upcomingPieces[numPieces - 1] == null || upcomingPieces[numPieces - 2] == null) { return; }
			Vector3 lastPieces = upcomingPieces[numPieces - 1].position - upcomingPieces[numPieces - 2].position;
			if (lastPieces.magnitude < pieceSpacingY)
			{
				moveUpcomingPieces = true;

				for (int i = 0; i < numPieces - 1; i++) //Last piece was already placed in correct spot when created
				{
					float xPos = upcomingPieces[i].transform.position.x;
					float yPos = upcomingPieces[i].transform.position.y;
					float yNewPos = yPos + moveSpeed * Time.deltaTime;
					upcomingPieces[i].transform.position = new Vector2(xPos, yNewPos);
				}
			} else
			{
				moveUpcomingPieces = false;
			}
		}

		private void SavePiecePlaced()
		{
			Transform pieceClicked = upcomingPieces[0];
			pieceClicked.SetParent(placedPiecesContainer);
			pieceClicked.SetAsLastSibling();
			gameBoard.GetComponent<Board>().AddPieceToBoard(pieceClicked);
		}
	}
}