using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractRadarController : MonoBehaviour {
    [SerializeField] private List<Collider> potentialProjectionSurfaces = new();
    private Collider currentProjectionSurface;

    private Vector3 gizmoDrawLoc;

    private void Update() {
        if (!PlayerBehaviour.Instance.IsIn3D() || !PlayerBehaviour.Instance.playerDimensionController.DOGEnabled) {
            PlayerBehaviour.Instance.playerDimensionController.DisableProjections();
            return;
        }
        CheckForPotentialSurfaces();
    }
    private void HandleOneSurfaceNearby() {
        //if the only surface found is not transferable disable project and quit out
        if (!potentialProjectionSurfaces[0].GetComponent<WallBehaviour>().AllowsDimensionTransition) {
            PlayerBehaviour.Instance.playerDimensionController.DisableProjections();
            return;
        }

        currentProjectionSurface = potentialProjectionSurfaces[0];
        if (PlayerBehaviour.Instance.playerDimensionController.IsProjecting) {
            //update the position if currently projecting\
            PlayerBehaviour.Instance.playerDimensionController.EnableProjection(currentProjectionSurface,
                currentProjectionSurface.ClosestPointOnBounds(PlayerBehaviour.Instance.player3D.transform.position));
            PlayerBehaviour.Instance.playerDimensionController.UpdateProjectionPosition(
                currentProjectionSurface,
                currentProjectionSurface.ClosestPointOnBounds(PlayerBehaviour.Instance.player3D.transform.position));
        }
        //found a surface and wasnt projecting before
        else {
            //so enable the projection at the closest point
            PlayerBehaviour.Instance.playerDimensionController.EnableProjection(currentProjectionSurface,
                currentProjectionSurface.ClosestPointOnBounds(PlayerBehaviour.Instance.player3D.transform.position));
        }
    }
    private void HandleMultipleSurfacesNearby() {
        float distance = float.MaxValue;
        Collider closest = null;
        Vector3 closestPointOnBounds = Vector3.zero;

        var transferableSurfaces = potentialProjectionSurfaces.FindAll(collider => {
            if (collider.TryGetComponent(out WallBehaviour wallB)) {
                //return wallB.AllowsDimensionTransition;
                return wallB;
            }
            return false;
        });
        //iterate colliders that are currently in range of the player's interaction range
        foreach (Collider c in transferableSurfaces) {
            var closePoint = c.ClosestPointOnBounds(PlayerBehaviour.Instance.player3D.transform.position);
            var distToCollider = Vector3.Distance(closePoint, PlayerBehaviour.Instance.player3D.transform.position);

            //looking for the closest one to the player
            if (distToCollider < distance) {
                gizmoDrawLoc = PlayerBehaviour.Instance.player3D.transform.position;
                distance = distToCollider;
                closest = c;
                closestPointOnBounds = closePoint;
            }
        }
        //enable the projection on the closest wall
        closest.TryGetComponent(out WallBehaviour wallB);
        if (closest != null && wallB.AllowsDimensionTransition) {
            currentProjectionSurface = closest;
            PlayerBehaviour.Instance.player2DMovementController.currentWall = wallB;
            PlayerBehaviour.Instance.playerDimensionController.EnableProjection(currentProjectionSurface,
                closestPointOnBounds);
            PlayerBehaviour.Instance.playerDimensionController.UpdateProjectionPosition(
                currentProjectionSurface,
                closestPointOnBounds);
        }
        else {
            PlayerBehaviour.Instance.playerDimensionController.DisableProjections();
        }
    }
    //Checks through potentialSurfaces list to see if there are any to project onto
    //calls helper methods to handle one of multiple surfaces nearby
    //disables projections if there are no surfaces found
    private void CheckForPotentialSurfaces() {
        //theres at least 1 wall to project to
        if (potentialProjectionSurfaces.Any()) {
            if (potentialProjectionSurfaces.Count == 1) {
                HandleOneSurfaceNearby();
            }
            //more than 1 potential surface need to find the closest one to the player
            else {
                HandleMultipleSurfacesNearby();
            }
        }
        //no surface was found so disable all projections
        else {
            PlayerBehaviour.Instance.playerDimensionController.DisableProjections();
        }
    }

    private void OnTriggerEnter(Collider other) {
        //found an object the player can interact with so add it to the pickupController list
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject == null) {
                tGObject = other.transform;
            }
            if (tGObject.TryGetComponent(out GrabbableObject grabbableObject)) {
                PlayerBehaviour.Instance.pickupController.AddObjectToInRangeList(grabbableObject);
            }
        }
        //tell projection to enable
        else if (other.gameObject.layer == LayerInfo.WALL) {

            if (potentialProjectionSurfaces.Contains(other)) return;
            potentialProjectionSurfaces.Add(other);
        }
    }
    private void OnTriggerExit(Collider other) {
        //found an object the player can interact with so remove it from pickupController list
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject == null) {
                tGObject = other.transform;
            }
            Debug.Log(tGObject.name);
            if (tGObject.TryGetComponent(out GrabbableObject tObject)) {
                PlayerBehaviour.Instance.pickupController.RemoveObjectFromRangeList(tObject);
            }
        }
        //tell projection to disasble
        else if (other.gameObject.layer == LayerInfo.WALL) {
            if (other.gameObject.GetComponent<WallBehaviour>().AllowsDimensionTransition) {
                potentialProjectionSurfaces.Remove(other);
            }
        }
    }
    public void clearsurfaces() {
        potentialProjectionSurfaces.Clear();
    }

}
