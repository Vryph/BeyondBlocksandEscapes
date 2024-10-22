using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBE
{
    public class Checkpoint : MonoBehaviour
    {
        private BoxCollider2D _collider;

        [SerializeField] private TempLapManager _tempLapManager;

        private void Awake()
        {
           _collider = GetComponent<BoxCollider2D>();
        
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_tempLapManager._checkpointTrigger == false)
            {
                _tempLapManager._checkpointTrigger = true;
            }
        }
    }
}