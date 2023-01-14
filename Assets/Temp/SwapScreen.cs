using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapScreen : MonoBehaviour
{
    public Canvas d1, d2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            d1.targetDisplay = d1.targetDisplay == 1 ? 0 : 1;
            d2.targetDisplay = d2.targetDisplay == 1 ? 0 : 1;
        }
    }
}
