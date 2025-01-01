using UnityEngine;

namespace IdleStrategyKit
{
    public class CameraExtended : MonoBehaviour
    {
        public Camera _camera;

        [Header("Zoom")]
        public float zoomMin = 1;
        public float zoomMax = 10;

        [Header("Limits")]
        public float leftLimit;
        public float rightLimit;
        public float topLimit;
        public float bottomLimit;

        Vector3 touch;

        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    touch = _camera.ScreenToWorldPoint(Input.mousePosition);
                }
                if (Input.GetMouseButton(0) && !Utils.IsCursorOverUserInterface())
                {
                    Vector3 direction = touch - _camera.ScreenToWorldPoint(Input.mousePosition);
                    _camera.transform.position += direction;
                }

                //zoom
                if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                    float distTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
                    float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

                    float difference = currentDistTouch - distTouch;

                    Zoom(difference * 0.01f);
                }
                else Zoom(Input.GetAxis("Mouse ScrollWheel"));

                float camWidth = _camera.orthographicSize * _camera.aspect;
                float leftLimitCurrent = leftLimit + camWidth;
                float rightLimitCurrent = rightLimit - camWidth;

                float bottomLimitCurrent = bottomLimit + _camera.orthographicSize;
                float topLimitCurrent = topLimit - _camera.orthographicSize;

                //limits
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, leftLimitCurrent, rightLimitCurrent),
                    Mathf.Clamp(transform.position.y, bottomLimitCurrent, topLimitCurrent),
                    transform.position.z);

                SelectionHandling(player);
            }
        }

        void Zoom(float increment)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(leftLimit, topLimit), new Vector2(rightLimit, topLimit));
            Gizmos.DrawLine(new Vector2(leftLimit, bottomLimit), new Vector2(rightLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(leftLimit, topLimit), new Vector2(leftLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(rightLimit, topLimit), new Vector2(rightLimit, bottomLimit));
        }

        private void SelectionHandling(Player player)
        {
            // click raycasting if not over a UI element & not pinching on mobile
            // note: this only works if the UI's CanvasGroup blocks Raycasts
            if (Input.GetMouseButtonDown(0) && !Utils.IsCursorOverUserInterface() && Input.touchCount <= 1)
            {
                // cast a 3D ray from the camera towards the 2D scene.
                // Physics2D.Raycast isn't made for that, we use GetRayIntersection.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // raycast with local player ignore option
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                // valid target?
                Entity entity = hit.transform != null ? hit.transform.GetComponentInParent<Entity>() : null;
                if (entity != null &&
                    entity is BuildingOnMap building &&
                    building.building != null &&
                    building.indexInGlobalList != -1 &&
                    player.buildings.buildings[building.indexInGlobalList].level > 0)
                {
                    //if entity is Camp
                    if (building.building.openTownInfo) UITown.singleton.panel.SetActive(true);
                    else if (building.building.openResearches) UIResearches.singleton.panel.SetActive(true);
                    else if (building.building.IsManaged()) UIBuild.singleton.OpenBuildingManager(building.building);
                }
                else if (entity && entity is Battlefield battlefield)
                {
                    //UIEnemyCamps.selectedEnemyCamp = player.battles.enemyCamps[player.battles.GetEnemyCampByHash(battlefield.hash)];
                    if (player.camps.enemyCamps[battlefield.index].state == CampState.none)
                    {
                        //_audio.PlaySoundButtonClick();
                        player.camps.selectedEnemyCamp = player.camps.enemyCamps[battlefield.index];
                        UIEnemyCamps.singleton.Show();
                    }
                }
                // if we hit nothing then we want to move somewhere
                //else
                //{
                //    UIMenu.showMenuType = MenuType.Close;
                //}
            }
        }
    }
}


