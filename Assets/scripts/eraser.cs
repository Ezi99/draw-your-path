using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    public Transform m_EndPosition;
    public Transform m_StartPosition;
    public float moveSpeed = 10.0f;

    public void erase()
    {
        StartCoroutine(GoToTarget());
    }

    IEnumerator GoToTarget()
    {
        while (transform.position != m_EndPosition.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_EndPosition.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = m_StartPosition.position;
    }
}