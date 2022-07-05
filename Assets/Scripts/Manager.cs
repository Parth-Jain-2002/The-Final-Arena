using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.ParthJain.FPSShooter{
    public class Manager : MonoBehaviour
    {   
        public string player_prefab;
        public Transform[] spawn_pos;

        // Start is called before the first frame update
        void Start()
        {
            Spawn();    
        }

        public void Spawn(){
            Transform spawn = spawn_pos[Random.Range(0,spawn_pos.Length)];
            PhotonNetwork.Instantiate(player_prefab, spawn.position, spawn.rotation);
        }
    }
}