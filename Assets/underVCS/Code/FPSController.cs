using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    //---
    public WorldManager worldManager;
    public GameObject selectionCube;
    public GameObject normalCube;
    public GameObject hitSphere;
    public float voxelTransformRange = 5f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        //for debug todo remove
        // for (int i = 0; i < 10000; i++)
        // {
        //     Physics.Raycast(transform.position, playerCamera.transform.forward + (new Vector3(Random.value, Random.value, Random.value)).normalized, out hit, 90f);
        // }
    }
    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        IntVector3 blockPos, normalBlock;
        bool isBlockSelcted = GetBlockUnderScope(out blockPos, out normalBlock);
        selectionCube.transform.position = blockPos.ToVec3() + new Vector3(0.525f,0.525f,0.525f);
        normalCube.transform.position = normalBlock.ToVec3() + 0.525f * Vector3.one;
        if (Input.GetMouseButtonDown(1))
        {
            if (isBlockSelcted)
            {
                //Destroy block
                // type 0 means air. TODO: add enum
                worldManager.ChangeBlock(blockPos, 0);
                AiManager.pf.RemoveBlock(blockPos);
            }
        }
        if (Input.GetMouseButtonDown(0) && isBlockSelcted)
        {
            //add block
            worldManager.ChangeBlock(normalBlock, 4);
            AiManager.pf.AddBlock(normalBlock);
        }
    }

    private bool GetBlockUnderScope(out IntVector3 blockPos, out IntVector3 normalBlock)
    {
        RaycastHit hit;
        bool didHit;
        
        didHit = Physics.Raycast(transform.position, playerCamera.transform.forward, out hit, voxelTransformRange);
        if (didHit) {
            //print(hit.point);
            hitSphere.transform.position = hit.point;
            // Debug.DrawLine(transform.position, hit.point, Color.white);
            Vector3 centerOfHitBlock = hit.point - 0.1f * hit.normal;
            blockPos = new IntVector3(centerOfHitBlock);
            Vector3 centerOfNormalBlock = hit.point + 0.1f * hit.normal;
            normalBlock = new IntVector3(centerOfNormalBlock);
            return true;
        }
        blockPos = new IntVector3(0, 0, 0);
        normalBlock = new IntVector3(0, 0, 0);
        return false;
    }
}