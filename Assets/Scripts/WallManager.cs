using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public string targetTag = "Wall";       
    public bool wasWallHit;

    public bool WasWallHit() => wasWallHit;


    void OnCollisionEnter2D(Collision2D collider)
    {
        if(!wasWallHit) {
            wasWallHit = collider.gameObject.tag == targetTag;
        }

    }
}
