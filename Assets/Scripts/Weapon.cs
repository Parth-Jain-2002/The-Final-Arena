using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ParthJain.FPSShooter{
    public class Weapon : MonoBehaviour
    {   
        #region Variables
        public Gun[] loadOut;
        public Transform weaponParent;
        
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
            if(Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
            // 0 - Left Mouse 1-Right Mouse
            if(currentWeapon!=null) Aim(Input.GetMouseButton(1));
        }
        #endregion
        
        #region Private
        void Equip(int ind){
            if(currentWeapon!=null) Destroy(currentWeapon);

            currentIndex = ind;

            GameObject newEquipment = Instantiate(loadOut[ind].prefab,weaponParent.position,weaponParent.rotation,weaponParent) as GameObject;
            newEquipment.transform.localPosition = Vector3.zero;
            newEquipment.transform.localEulerAngles = Vector3.zero;

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
        #endregion
    }
}
