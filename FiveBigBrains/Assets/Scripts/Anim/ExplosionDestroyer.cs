using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroyer : MonoBehaviour
{
    // destroy the last frame when animation is finished
    void DestroyGameObject() 
    {
        Destroy (gameObject);
    }
}
