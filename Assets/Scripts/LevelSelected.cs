using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelected : MonoBehaviour
{
    const int maxLevels = 80;
    [SerializeField] [Range (1, maxLevels)] int levelNum;

    public void OnMouseUp()
    {
        //TODO handle the selected level clicked
    }


}
