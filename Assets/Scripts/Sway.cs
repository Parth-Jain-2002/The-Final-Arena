using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ParthJain.FPSShooter{
    public class Sway : MonoBehaviour
    {   
        // Basically Sway is to add the 

        #region Variables
        public float intensity;
        public float smooth;

        private Quaternion origin_rotation;
        #endregion
        
        #region Monobehaviour
        // Start is called before the first frame update
        void Start()
        {
            origin_rotation = transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSway();
        }
        #endregion

        #region Private

        private void UpdateSway(){
            // controls for input
            float xmouse = Input.GetAxis("Mouse X");
            float ymouse = Input.GetAxis("Mouse Y");
            
            // calculate target rotation
            Quaternion x_adj = Quaternion.AngleAxis(-xmouse* intensity, Vector3.up);
            Quaternion y_adj = Quaternion.AngleAxis(ymouse* intensity, Vector3.right);
            Quaternion target_rotation = origin_rotation * x_adj * y_adj;
            
            // rotate towards target
            transform.localRotation = Quaternion.Lerp(transform.localRotation,target_rotation,Time.deltaTime * smooth);
        }

        #endregion
    }
}
