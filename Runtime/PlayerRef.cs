using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MayoOps.Runtime {
    public struct PlayerRef {
        public Transform Instance;
        public Vector3 netPosition;
        public Quaternion netRotation;
        public PlayerRef(Transform instance) {
            Instance = instance.transform.GetChild(0);
            Instance.parent = null;
            GameObject.Destroy(instance.gameObject);
        }

        public void UpdateData(Vector3 pos, Quaternion rot) {
            netPosition = pos;
            netRotation = rot;
            Instance.position = Vector3.Lerp(Instance.position, netPosition, moveSpeed * Time.deltaTime);
            Instance.rotation = Quaternion.Lerp(Instance.rotation, netRotation, moveSpeed * Time.deltaTime);
        }
        public const float moveSpeed = 10;
        public void OnUpdate() {
            
        }
    }
}
