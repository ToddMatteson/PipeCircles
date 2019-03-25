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

		private bool dragging = false;

		private Vector2 originalPos;

		private Transform objectToDrag;
		private Sprite objectToDragImage;

		List<RaycastResult> hitObjects = new List<RaycastResult>();

		[SerializeField] Transform upcomingTransform;

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				objectToDrag = GetDraggableTransformUnderMouse(true);
			
				if (objectToDrag != null)
				{
					dragging = true;

					objectToDrag.SetAsLastSibling();

					originalPos = objectToDrag.position;

					//Insert part about not being a raycast target here
					objectToDragImage = objectToDrag.GetComponent<Sprite>();
					//objectToDragImage.raycastTarget = false;
				}
			}

			if (dragging && objectToDrag != null)
			{
				print("Mouse: " + Input.mousePosition);
				print("ObjectToDrag: " + objectToDrag.position);
				print("ObjectToDrag original position: " + originalPos);

				objectToDrag.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//objectToDrag.position = Input.mousePosition;
			}

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

					//objectToDragImage.raycastTarget = true;
					objectToDrag = null;
				}
				dragging = false;
			}
		}

		private Transform GetDraggableTransformUnderMouse(bool draggable)
		{
			bool objectDraggable = draggable;
			GameObject clickedObject = FindObjectClicked();
			if (clickedObject == null)
			{
				print("null object");
			} else
			{
				print("clickedObject " + clickedObject.name);
			}

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

		private GameObject GetObjectUnderMouse()
		{
			var pointer = new PointerEventData(EventSystem.current);
			pointer.position = Input.mousePosition;

			EventSystem.current.RaycastAll(pointer,hitObjects);
			if (hitObjects.Count <= 0) { return null; }
			return hitObjects.First().gameObject;
		}

		private GameObject FindObjectClicked()
		{
			Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
			Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
			Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
			Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
			//Debug.DrawRay(mousePosN, mousePosF - mousePosN, Color.green);

			RaycastHit2D hit = Physics2D.Raycast(mousePosN, mousePosF - mousePosN);
			if (hit.collider != null)
			{
				print("Object found: " + hit.collider.name);
				return hit.collider.gameObject;
			}
			return null;
		}
	}
}
