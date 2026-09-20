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
        [Header("Properties")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSmoothTime = 0.1f;
        [SerializeField] private bool isActive = true;
        [Header("Channels")]
        [SerializeField] private BoolEventChannelSO controlActivationEventChannelSO;

        private float turnSmoothVelocity;

        private void Update()
        {
            if (!isActive) return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * (moveSpeed * Time.deltaTime));
            }
        }
    }
}
