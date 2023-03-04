using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eraser : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float moveSpeed = 10.0f;

    public void erase()
    {
        StartCoroutine(GoToTarget());
    }

    IEnumerator GoToTarget()
    {
        while (transform.position != endPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
    }
}