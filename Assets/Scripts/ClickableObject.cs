using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    bool highlighted;

    public void ToggleHighlight()
    {
        highlighted = !highlighted;
        Debug.Log(highlighted);
    }
}