using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTutorial : MonoBehaviour
{
    public Transform m_EndPosition;
    public float moveSpeed;
    
    public void CollapseWall()
    {
        StartCoroutine(GoToTarget());
        Destroy(gameObject, 20);
    }

    IEnumerator GoToTarget()
    {
        while (transform.position != m_EndPosition.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_EndPosition.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
