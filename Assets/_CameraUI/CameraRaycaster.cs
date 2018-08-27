using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int CLICK_TO_WALK_LAYER = 8;
        float maxRaycasterDepth = 100f;

        Rect screenRectOnConstruction = new Rect(0, 0, Screen.width, Screen.height); 

        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        public delegate void OnMouseOverPotentiallyWalkable(Vector3 destination);
        public event OnMouseOverPotentiallyWalkable onMouseOverPotentiallyWalkable;

        // Update is called once per frame
        void Update()
        {
            screenRectOnConstruction = new Rect(0, 0, Screen.width, Screen.height);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //implement UI interaction
            }
            else
            {
                PerformRaycast();
            }
        }

        void PerformRaycast()
        {
            if (screenRectOnConstruction.Contains(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (RaycastForEnemy(ray))
                {
                    return;
                }
                if (RaycastForPotentiallyWalkable(ray))
                {
                    return;
                }
            }
        }

        bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycasterDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<EnemyAI>();
            if (enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }
            return false;
        }

        bool RaycastForPotentiallyWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask clickToWalkLayer = 1 << CLICK_TO_WALK_LAYER;
            bool clickToWalkHit = Physics.Raycast(ray, out hitInfo, maxRaycasterDepth, clickToWalkLayer);
            if (clickToWalkHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}