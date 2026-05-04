using System.Collections.Generic;
using NGDtuanh.MonsterLegendsLite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MonsterLegendsLite {
    public class TestClickTarget : MonoBehaviourExt {
        void Update() {
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                var pointer = new PointerEventData(EventSystem.current) {
                    position = Mouse.current.position.ReadValue()
                };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, results);
        
                foreach (var r in results)
                    Debug.Log($"Hit: {r.gameObject.name}");
            }
        }
    }
}