using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        public void MoveUpWard()
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        public void MoveDownWard()
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        public void MoveLeftSide()
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        public void MoveRightSide()
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
}
