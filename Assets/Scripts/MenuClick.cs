using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClick : MonoBehaviour
{
	[SerializeField] GameObject mainMenuPanel;

	public void OnMouseUp()
	{
		print("reached OnMouseUp");
		if (mainMenuPanel != null)
		{
			mainMenuPanel.SetActive(true);
		}
	}
}
