using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform m_PlayersHead;

    private void Awake()
    {
        m_PlayersHead = GameObject.Find("XR Origin").transform;
    }
    void Update()
    {
        gameObject.transform.LookAt(new Vector3(m_PlayersHead.position.x, gameObject.transform.position.y, m_PlayersHead.position.z));
        gameObject.transform.forward *= -1;
    }
}
