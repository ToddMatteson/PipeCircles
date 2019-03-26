using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace PipeCircles
{
	public class Draggable : MonoBehaviour
	{
		public const string DRAGGABLE_TAG = "Draggable";
		public const string REPLACEABLE_TAG = "Replaceable";

		private Transform objectToDrag;
		private bool dragging = false;
		private Vector2 originalPos;
		private int originalSortingOrder;
		private Vector2 mouseOffset;

		[SerializeField] Transform upcomingTransform;

		void Update()
		{
			HandleMouseDown();
			HandleMouseDrag();
			HandleMouseUp();
		}

		private void HandleMouseDown()
		{
			if (Input.GetMouseButtonDown(0))
			{
				objectToDrag = GetDraggableTransformUnderMouse(true);

				if (objectToDrag != null)
				{
					dragging = true;

					objectToDrag.SetAsLastSibling();

					originalPos = objectToDrag.position;
					objectToDrag.GetComponent<Collider2D>().enabled = false; //So raycasts don't hit the piece being dragged
					originalSortingOrder = objectToDrag.GetComponent<SpriteRenderer>().sortingOrder;
					objectToDrag.GetComponent<SpriteRenderer>().sortingOrder += 100;
				}
			}
		}

		private void HandleMouseDrag()
		{
			if (dragging && objectToDrag != null)
			{
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				objectToDrag.position = new Vector2(mouseWorldPos.x, mouseWorldPos.y) + mouseOffset;
			}
		}

		private void HandleMouseUp()
		{
			if (Input.GetMouseButtonUp(0))
			{
				if (objectToDrag != null)
				{
					Transform objectToReplace = GetDraggableTransformUnderMouse(false);
					if (objectToReplace != null)
					{
						objectToDrag.position = objectToReplace.position;
						objectToDrag.tag = REPLACEABLE_TAG;
						upcomingTransform.GetComponent<Upcoming>().PieceClicked();

						Destroy(objectToReplace.gameObject); //TODO Needs to be reworked to show reconstruction time
					} else
					{
						objectToDrag.position = originalPos;
					}

					objectToDrag.GetComponent<Collider2D>().enabled = true; //Allowing raycasts to hit the piece again
					objectToDrag.GetComponent<SpriteRenderer>().sortingOrder = originalSortingOrder;
					objectToDrag = null;
				}
				dragging = false;
			}
		}

		private Transform GetDraggableTransformUnderMouse(bool draggable)
		{
			bool objectDraggable = draggable;
			GameObject clickedObject = FindObjectClicked();

			if (draggable)
			{
				if (clickedObject != null && clickedObject.tag == DRAGGABLE_TAG)
				{
					return clickedObject.transform;
				}
			} else
			{
				if (clickedObject != null && clickedObject.tag == REPLACEABLE_TAG)
				{
					return clickedObject.transform;
				}
			}
			return null;
		}

		private GameObject FindObjectClicked()
		{
			Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
			Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
			Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
			Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

			RaycastHit2D hit = Physics2D.Raycast(mousePosN, mousePosF - mousePosN);
			if (hit.collider != null)
			{
				Vector3 pos1 = hit.transform.position;
				mouseOffset = new Vector2(pos1.x, pos1.y) - hit.point;

				return hit.collider.gameObject;
			}
			return null;
		}
	}
}
