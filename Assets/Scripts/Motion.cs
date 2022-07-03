using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ParthJain.FPSShooter{
    public class Motion : MonoBehaviour
    {   
        #region  Variables
        public float speed;
        public float sprintMultiplier;
        public float jumpForce;
        public Camera normalCam;
        public Transform weaponParent;
        public Transform groundDetector;
        public LayerMask ground;

        private Rigidbody rig;
        private Vector3 weaponParentOrigin;

        private float baseFOV;
        private float sprintFOVModifier=1.25f;
        
        #endregion
        
        #region Monobehaviour
        // Start is called before the first frame update
        void Start()
        {   
            baseFOV = normalCam.fieldOfView;
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
        }
        
        void Update(){
            // Axises
            float hmove = Input.GetAxisRaw("Horizontal");
            float vmove = Input.GetAxisRaw("Vertical");

            // Controls for sprint (In Update for more smooth movement)
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            // Condition of sprinting based on two inputs
            bool isGrounded = Physics.Raycast(groundDetector.position,Vector3.down,0.3f,ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && vmove>0 && !isJumping && isGrounded;
            
            // Jump
            if(isJumping){
                rig.AddForce(Vector3.up * jumpForce);
            }
        }   
        
        // Update is called once per frame
        void FixedUpdate()
        {   
            // Axises
            float hmove = Input.GetAxisRaw("Horizontal");
            float vmove = Input.GetAxisRaw("Vertical");
            
            // Controls for sprint (In Update for more smooth movement)
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            // Condition of sprinting based on two inputs
            bool isGrounded = Physics.Raycast(groundDetector.position,Vector3.down,0.3f,ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && vmove>0 && !isJumping && isGrounded;

            // Motion
            Vector3 direction = new Vector3(hmove,0,vmove);
            direction.Normalize();

            float adjSpeed = speed;
            if(isSprinting) adjSpeed *= sprintMultiplier;
            
            // To compensate for jump
            Vector3 targetVelocity = transform.TransformDirection(direction) * adjSpeed * Time.fixedDeltaTime;
            targetVelocity.y = rig.velocity.y;
            rig.velocity = targetVelocity;
            
            // Field of View to create the effect of sprinting
            if(isSprinting) normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 8f);
            else normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV , Time.deltaTime * 8f);
        }
        #endregion
    
        #region Private
        
        void HeadBob(float x, float xintensity, float yintensity){

        }
        #endregion
    }
}
