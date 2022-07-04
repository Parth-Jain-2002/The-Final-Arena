using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.ParthJain.FPSShooter{
    public class Look : MonoBehaviourPunCallbacks
    {   

        #region Variables

        public static bool cursorLocked = true;
        // Start is called before the first frame update
        public Transform player;
        public Transform cams;
        public Transform weapon;

        public float xSense;
        public float ySense;
        public float maxAngle;

        private Quaternion camCenter;

        #endregion

        #region Monobehaviour 
        void Start()
        {   
            camCenter = cams.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            SetY();
            SetX();
            UpdateCursorLock();
        }

        #endregion
        
        #region Private
        // Allow the player to look up and down through camera (Handled through camera)
        void SetY(){
            float mouseY = Input.GetAxis("Mouse Y") * ySense * Time.deltaTime;
            Quaternion adj = Quaternion.AngleAxis(mouseY, -Vector3.right);
            Quaternion change = cams.localRotation * adj;

            if(Quaternion.Angle(camCenter,change)<maxAngle){
               cams.localRotation = change;
            }

            weapon.rotation = cams.rotation;
        }

        void SetX(){
            float mouseY = Input.GetAxis("Mouse X") * xSense * Time.deltaTime;
            Quaternion adj = Quaternion.AngleAxis(mouseY, Vector3.up);
            Quaternion change = player.localRotation * adj;
            player.localRotation = change;
        }

        void UpdateCursorLock(){
            if(cursorLocked){
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
    
                if(Input.GetKeyDown(KeyCode.Escape)){
                    cursorLocked = false;
                }
            }
            else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if(Input.GetKeyDown(KeyCode.Escape)){
                    cursorLocked = true;
                }
            }
        }

        #endregion
    }
}
