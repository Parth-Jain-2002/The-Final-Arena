using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.ParthJain.FPSShooter{
    public class Player : MonoBehaviourPunCallbacks
    {   
        #region  Variables
        public float speed;
        public float sprintMultiplier;
        public float jumpForce;
        public float maxHealth;
        public Camera normalCam;
        public GameObject cameraParent;
        public Transform weaponParent;
        public Transform groundDetector;
        public LayerMask ground;
        
        private Transform uiHealthBar;
        private Rigidbody rig;
        private Vector3 weaponParentOrigin;
        private Vector3 targetWeaponPosition;

        private float movementCounter;
        private float idleCounter;

        private float baseFOV;
        private float sprintFOVModifier=1.25f;
        
        private float currentHealth;

        private Manager manager;

        #endregion
        
        #region Monobehaviour
        // Start is called before the first frame update
        void Start()
        {   
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            currentHealth = maxHealth;

            cameraParent.SetActive(photonView.IsMine);
            
            if(!photonView.IsMine) gameObject.layer = 9;

            baseFOV = normalCam.fieldOfView;
            if(Camera.main) Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
            weaponParentOrigin = weaponParent.localPosition;
            
            if(photonView.IsMine){
               uiHealthBar = GameObject.Find("HUD/Health/Bar").transform;
               RefreshHealthBar();
            }
        }
        
        void Update(){
            
            // So that we don't control other player
            if(!photonView.IsMine) return;

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

            // If the player is head bobing or not
            if(hmove == 0 && vmove == 0) {
                HeadBob(idleCounter,0.025f,0.025f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponPosition, Time.deltaTime * 3f );
            }
            else if(!isSprinting){
                HeadBob(movementCounter,0.035f,0.035f);
                movementCounter+=Time.deltaTime * 3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponPosition, Time.deltaTime * 9f );
            }
            else{
                HeadBob(movementCounter,0.15f,0.075f);
                movementCounter+=Time.deltaTime * 3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponPosition, Time.deltaTime * 15f );
            }
            
            // Update Health Bar
            RefreshHealthBar();
        }   
        
        // Update is called once per frame
        void FixedUpdate()
        {   
            // So that we don't control other player
            if(!photonView.IsMine) return;

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
        
        // Headbob is basically the swaying the camera up and down as the character walks, to simulate the way a person's body moves up 
        // and down while taking steps. Bascially we will mimic this
        void HeadBob(float z, float xintensity, float yintensity){
            targetWeaponPosition = weaponParentOrigin + new Vector3(Mathf.Cos(z) * xintensity, Mathf.Sin(z * 2) * yintensity, 0);
        }
        
        void RefreshHealthBar(){
            float healthRatio = (float) currentHealth/ (float) maxHealth;
            uiHealthBar.localScale = Vector3.Lerp(uiHealthBar.localScale,new Vector3(healthRatio,1,1),Time.deltaTime * 8f); 
        }

        #endregion

        #region Public
        
        public void TakeDamage(int damage){
            if(photonView.IsMine){
                currentHealth -= damage;
                RefreshHealthBar();

                if(currentHealth <= 0){
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}
