using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBox : MonoBehaviour
{
    private bool isMagnified = false;

    public void Magnify(float factor)
    {
        if (!isMagnified)
        {
            transform.localScale *= factor;
            isMagnified = true;
        }
    }
}
