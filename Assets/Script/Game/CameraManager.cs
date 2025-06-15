using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class CameraManager
{
        public CinemachineCamera cinemachineCamera;
        public CinemachineFollow cinemachineFollow;
        public CinemachineRotationComposer cinemachineRotationComposer;

        private Transform baseLookAtTransform;
        private Vector3 baseFollowOffsetPosition;
        
        public GameObject redCameraTarget;
        public GameObject blueCameraTarget;

        public void Start()
        {
                baseLookAtTransform = this.cinemachineCamera.LookAt;
                baseFollowOffsetPosition = this.cinemachineFollow.FollowOffset;
                ResetPositionAndLookAt();
        }

        public void ResetPositionAndLookAt()
        {
                this.cinemachineCamera.LookAt = baseLookAtTransform;
                this.cinemachineFollow.FollowOffset = baseFollowOffsetPosition;
        }

        public void SetNewLookAtTransform(Transform newLookAtTransform)
        {
                this.cinemachineCamera.LookAt = newLookAtTransform;
        }

        public void SetNewLookAtTransform(Transform newLookAtTransform, Vector3 newFollowOffsetPosition)
        {
                this.cinemachineCamera.LookAt = newLookAtTransform;
                this.cinemachineFollow.FollowOffset = newFollowOffsetPosition;
        }

        public void SetNewLookAtTransform(Transform newLookAtTransform, Vector3 newFollowOffsetPosition, Vector2 screenPositionRotationComposer)
        {
                this.cinemachineCamera.LookAt = newLookAtTransform;
                this.cinemachineFollow.FollowOffset = newFollowOffsetPosition;
                this.cinemachineRotationComposer.Composition.ScreenPosition = screenPositionRotationComposer;
        }
}