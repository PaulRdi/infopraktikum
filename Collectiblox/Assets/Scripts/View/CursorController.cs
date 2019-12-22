using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox.View
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] Camera mainCam;
        [SerializeField] Collider interactionPlane;
        private void Awake()
        {
            if (mainCam == null)
                mainCam = FindObjectOfType<Camera>();
        }
        private void Update()
        {
            UpdateCursorPosition();
        }

        public void UpdateCursorPosition()
        {
            Ray ray = mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f));
            if (interactionPlane.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                transform.position = GameManager.QuantizeToGrid(hit.point);
            }
        }
    }
}
