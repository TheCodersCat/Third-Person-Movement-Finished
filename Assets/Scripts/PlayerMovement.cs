using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;

    [SerializeField] Transform cameraRig;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float animSpeed;

    [SerializeField] float jumpSpeed;
    [SerializeField] float gravity;
    [SerializeField] float floorDistance;

    Vector3 verticalVector;

    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        verticalVector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);

        Move();
        VerticalMovement();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal") * GetCurrentSpeed() * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * GetCurrentSpeed() * Time.deltaTime;

        Vector3 moveVector = new Vector3(horizontal, 0f, vertical);
        Vector3 relativeMoveVector = Quaternion.AngleAxis(cameraRig.eulerAngles.y, Vector3.up) * moveVector;

        characterController.Move(relativeMoveVector);

        animator.SetFloat("Speed", relativeMoveVector == Vector3.zero ? 0f : (isRunning ? 1f : .5f), animSpeed, Time.deltaTime);

        if (relativeMoveVector != Vector3.zero)
            Rotate(relativeMoveVector);
    }

    void Rotate(Vector3 moveVector)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveVector, Vector3.up);

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        transform.rotation = newRotation;
    }

    void VerticalMovement()
    {
        if (IsGrounded())
        {
            if (Input.GetButtonDown("Jump"))
            {
                verticalVector.y = jumpSpeed;
                animator.SetTrigger("Jump");
            }
            else if (verticalVector.y < 0)
                verticalVector.y = 0;
        }
        else
            verticalVector.y -= gravity * Time.deltaTime;

        characterController.Move(verticalVector);
    }

    bool IsGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
            return hit.distance <= floorDistance;

        return false;
    }

    float GetCurrentSpeed()
    {
        return isRunning ? runSpeed : walkSpeed;
    }
}
