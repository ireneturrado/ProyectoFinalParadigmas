using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    public Transform spawnPoint;

    private Animator animator;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

    }

    void Start()
    {
        if (spawnPoint != null)
        {
            controller.enabled = false;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            controller.enabled = true;
        }
    }

    public float turnSpeed = 120f; // puedes ajustar 90-180

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // girar
        float v = Input.GetAxisRaw("Vertical");   // avanzar

        // 1) Girar
        if (h != 0)
            transform.Rotate(0f, h * turnSpeed * Time.deltaTime, 0f);

        // 2) Movimiento adelante/atrás
        Vector3 move = transform.forward * v * speed;

        // 3) Gravedad
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // 4) UN SOLO Move con TODO
        Vector3 finalMove = move + velocity;
        controller.Move(finalMove * Time.deltaTime);

        // 5) Animación
        if (animator != null)
            animator.SetBool("isWalking", v != 0);
    }


}