using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWallDetector : MonoBehaviour
{
    public GameObject m_Wall;
    public Transform m_EndPosition;
    public Area1Ui m_AreaNavigator;
    public float m_MoveSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(GoToTarget());
            m_AreaNavigator.ShowMessage();
            Destroy(m_Wall, 10);
        }
    }

    IEnumerator GoToTarget()
    {
        Debug.Log($"drop wall - {m_Wall.transform.position != m_EndPosition.position}");
        while (m_Wall.transform.position != m_EndPosition.position)
        {
            m_Wall.transform.position = Vector3.MoveTowards(m_Wall.transform.position, m_EndPosition.position, m_MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
