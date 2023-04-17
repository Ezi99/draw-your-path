using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 9f; // speed of movement
    public float jumpForce = 10f; // force of jump

    private Rigidbody rb; // rigidbody component of the capsule

    private bool isGrounded = true; // check if player is on the ground

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input for horizontal and vertical movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Move the capsule based on input
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        rb.AddForce(movement * moveSpeed);

        // Jump if player is on the ground and spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Set isGrounded to true if player collides with ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
