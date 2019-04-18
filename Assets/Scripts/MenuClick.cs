using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClick : MonoBehaviour
{
	[SerializeField] GameObject mainMenuPanel = null;

	public void OnMouseUp()
	{
		print("reached OnMouseUp");
		if (mainMenuPanel != null)
		{
			mainMenuPanel.SetActive(true);
		}
	}
}
