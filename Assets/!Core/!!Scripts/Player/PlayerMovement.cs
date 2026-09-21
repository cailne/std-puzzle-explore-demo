using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform camTransform;
        [SerializeField] private PlayerAnimation playerAnimation;
        [Header("Properties")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSmoothTime = 0.1f;
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private bool isActive = true;
        [Header("Channels")]
        [SerializeField] private BoolEventChannelSO controlActivationEventChannelSO;

        private float turnSmoothVelocity;
        private float airVelocity;
        private float groundedTimer = 0.2f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            controlActivationEventChannelSO?.RegisterListener(SetActiveMovement);
        }

        private void OnDisable()
        {
            controlActivationEventChannelSO?.RemoveListener(SetActiveMovement);
        }

        private void Update()
        {
            if (!isActive)
            {
                playerAnimation.SetAnimationFloat(0f);
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            playerAnimation.SetAnimationFloat(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude < 0.1f) return;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir.x = Mathf.Clamp(moveDir.x, -1f, 1f) * moveSpeed;
            moveDir.z = Mathf.Clamp(moveDir.z, -1f, 1f) * moveSpeed;

            if (controller.isGrounded) groundedTimer = 0.2f;
            if (groundedTimer > 0) groundedTimer -= Time.deltaTime;

            if (controller.isGrounded && airVelocity < 0)
                airVelocity = 0f;

            airVelocity -= gravity * Time.deltaTime;
            moveDir.y = airVelocity;

            controller.Move(moveDir * Time.deltaTime);
        }

        private void SetActiveMovement(bool active)
        {
            isActive = active;
            if (!active)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
