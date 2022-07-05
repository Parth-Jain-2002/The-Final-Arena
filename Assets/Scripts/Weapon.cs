using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.ParthJain.FPSShooter{
    public class Weapon : MonoBehaviourPunCallbacks
    {   
        #region Variables
        public Gun[] loadOut;
        public Transform weaponParent;

        public GameObject bulletHole;
        public LayerMask canBeShot;
        
        private float currentCoolDown;
        private int currentIndex;
        private GameObject currentWeapon;
        #endregion
        
        #region Monobehaviour
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {   
            if(photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) photonView.RPC("Equip",RpcTarget.All,0);
            // 0 - Left Mouse 1-Right Mouse
            if(currentWeapon!=null){
                if(photonView.IsMine){
                    Aim(Input.GetMouseButton(1));
                    
                    // If player hits left mouse button
                    if(Input.GetMouseButtonDown(0) && currentCoolDown<=0){
                        photonView.RPC("Shoot",RpcTarget.All);
                    }
                    
                    // Cooldown - controls interval between two bullet shoot
                    if(currentCoolDown > 0) currentCoolDown -= Time.deltaTime;
                }
                // Elasticity in weapon
                if(currentWeapon!= null){
                    currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition,Vector3.zero,Time.deltaTime * 4f);
                }
            }

        }
        #endregion
        
        #region Private
        [PunRPC]
        void Equip(int ind){
            if(currentWeapon!=null) Destroy(currentWeapon);

            currentIndex = ind;

            GameObject newEquipment = Instantiate(loadOut[ind].prefab,weaponParent.position,weaponParent.rotation,weaponParent) as GameObject;
            newEquipment.transform.localPosition = Vector3.zero;
            newEquipment.transform.localEulerAngles = Vector3.zero;
            newEquipment.GetComponent<Sway>().isMine = photonView.IsMine;

            currentWeapon = newEquipment;
        }

        void Aim(bool isAiming){
            Transform anchor = currentWeapon.transform.Find("Anchor");
            Transform stateADS = currentWeapon.transform.Find("States/ADS");
            Transform stateHip = currentWeapon.transform.Find("States/Hip");

            if(isAiming){
                anchor.position = Vector3.Lerp(anchor.position,stateADS.position,Time.deltaTime * loadOut[currentIndex].aimSpeed);
            }
            else{
                anchor.position = Vector3.Lerp(anchor.position,stateHip.position,Time.deltaTime * loadOut[currentIndex].aimSpeed);
            }
        }
        
        [PunRPC]
        void Shoot(){
            Transform  spawn = transform.Find("Cameras/NormalCamera");
            
            // Bloom - Tilting of the bullet trajectory to make it look more realistic
            Vector3 bloom = spawn.position + spawn.forward * 1000f;
            bloom += Random.Range(-loadOut[currentIndex].bloom,loadOut[currentIndex].bloom) * spawn.up;
            bloom += Random.Range(-loadOut[currentIndex].bloom,loadOut[currentIndex].bloom) * spawn.right;
            bloom += spawn.position;
            bloom.Normalize();

            // Raycast
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(spawn.position,bloom, out hit,1000f,canBeShot)){
                GameObject newHole = Instantiate(bulletHole,hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                newHole.transform.LookAt(hit.point + hit.normal );
                Destroy(newHole,5f);

                if(photonView.IsMine){
                    // Shooting other player
                    if(hit.collider.gameObject.layer == 9){
                        // Call to Damage the Player
                        hit.collider.gameObject.GetPhotonView().RPC("TakeDamage",RpcTarget.All, loadOut[currentIndex].damage);
                    }
                }
            }

            // Gun FX
            currentWeapon.transform.Rotate(-loadOut[currentIndex].recoil,0,0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadOut[currentIndex].kickback;

            // CoolDown
            currentCoolDown = loadOut[currentIndex].firerate;
        }

        [PunRPC]
        private void TakeDamage(int damage){
            GetComponent<Motion>().TakeDamage(damage);
        }

        #endregion
    }
}
