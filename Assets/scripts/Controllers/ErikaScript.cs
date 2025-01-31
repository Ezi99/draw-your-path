using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ErikaScript : MonoBehaviour
{
    public List<Rigidbody> m_RigidBodies;
    private Animator m_Animator;
    public GameObject arrowObj;
    public Transform arrowPoint;
    public Transform playerTransform;
    public GameObject enemyCanvas;
    public Slider healthBar;
    public int health = 100;
    private bool died=false;

    private void Update()
    {
        healthBar.value = health;
    }
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void shoot()
    {
        Debug.Log("Arrow shot");
        GameObject arrow = Instantiate(arrowObj, arrowPoint.position, Quaternion.identity);
        Vector3 direction = playerTransform.position + new Vector3(0, 1.7f, 0) - arrowPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        arrow.transform.rotation = rotation;
        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * 25, ForceMode.Impulse);
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"Erika Health - {dmg} current health {health}");
        if (!died && health <= 0)
        {
            Debug.Log("shiiiiiikakaka");
            //m_Animator.enabled = false;
            Die();
            /*foreach (var item in rigidbodies)
            {
                item.isKinematic = false;
                item.useGravity = true;
            }*/
        }
    }

    private void Die()
    {
        Debug.Log("boakakaka");
        died = true;
        enemyCanvas.SetActive(false);
        Debug.Log("Death animation triggered");
        m_Animator.SetTrigger("Death");

    }
    public void PlayAnimation(string animationName)
    {
        m_Animator.Play(animationName);
        StartCoroutine(WaitForAnimationToEnd());
    }
    private IEnumerator WaitForAnimationToEnd()
    {
        while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        // Animation has completed, perform any desired actions here
    }
    public void destroy()
    {
        Destroy(gameObject);
    }
}
