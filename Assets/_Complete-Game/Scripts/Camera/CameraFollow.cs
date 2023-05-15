using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

namespace CompleteProject
{
    public class CameraFollow : MonoBehaviourPun
    {
        public Transform target;            // The position that that camera will be following.
        public float smoothing = 5f;        // The speed with which the camera will be following.

        Vector3 offset;                     // The initial offset from the target.

        void Start()
        {
            Vector3 startPos = transform.position;
            startPos.x = target.position.x;
            transform.position = startPos;
            // Calculate the initial offset.
            offset = transform.position - target.position;
        }

        void FixedUpdate()
        {
            if (!GameController.Instance.IsGameOver())
            {
                // Create a postion the camera is aiming for based on the offset from the target.
                Vector3 targetCamPos = target.position + offset;

                // Smoothly interpolate between the camera's current position and it's target position.
                transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
            }
        }

        public void SetTarget(Transform _target)
        {
            target = _target;
        }
    }
}