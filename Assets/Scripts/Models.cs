using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Models
{

    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone,
    }

    [Serializable]
    public class PlayerSettingModel
    {

        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted;
        public bool ViewYInverted;


        [Header("Movement - Running")]
        public float RunningForwardSpeed;
        public float RunningStrafeSpeed;



        [Header("Movement - Walking")]
        public float WalkingForwardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;


        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFallof;
    }

    [Serializable]
    public class CharacterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;

    }
    #endregion

}
