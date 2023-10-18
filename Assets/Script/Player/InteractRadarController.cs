using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractRadarController : MonoBehaviour {
    //  [SerializeField] private PlayerControllerBeta playerScript;
    [SerializeField] private PlayerBehaviour playerBehaviour;
    [SerializeField] private PlayerDimensionController playerDimensionController;
    [SerializeField] private GameObject Player3D;
    [SerializeField] private  MovementController_2D movement;
    [SerializeField]private List<Collider> potentialProjectionSurfaces = new();
    private Collider currentProjectionSurface;

    private Vector3 gizmoDrawLoc;

    private void Update() {
        if (!playerBehaviour.IsIn3D() || !playerDimensionController.DOGEnabled) {
            playerDimensionController.DisableProjections();
            return;
        }
        CheckForPotentialSurfaces();
    }
    private void HandleOneSurfaceNearby() {
       // print("handleonesurface is being called");
        //if the only surface found is not transferable disable project and quit out
        if (!potentialProjectionSurfaces[0].GetComponent<WallBehaviour>().AllowsDimensionTransition ) {
            playerDimensionController.DisableProjections();
            return;
        }

        currentProjectionSurface = potentialProjectionSurfaces[0];
        if (playerDimensionController.IsProjecting) {
            //update the position if currently projecting
            playerDimensionController.UpdateProjectionPosition(
                currentProjectionSurface,
                currentProjectionSurface.ClosestPointOnBounds(Player3D.transform.position));
        }
        //found a surface and wasnt projecting before
        else {
            //so enable the projection at the closest point
            playerDimensionController.EnableProjection(currentProjectionSurface,
                currentProjectionSurface.ClosestPointOnBounds(Player3D.transform.position));
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
            var closePoint = c.ClosestPointOnBounds(Player3D.transform.position);
            var distToCollider = Vector3.Distance(closePoint, Player3D.transform.position);
           
            //looking for the closest one to the player
            if (distToCollider < distance) {
                gizmoDrawLoc = Player3D.transform.position;
                distance = distToCollider;
                closest = c;
                closestPointOnBounds = closePoint;
            }
        }
        //enable the projection on the closest wall
        closest.TryGetComponent(out WallBehaviour wallB);
        //print(closest);
        if (closest != null && wallB.AllowsDimensionTransition)
        {
           // print("projecting");
            currentProjectionSurface = closest;
            movement.currentWall = wallB;
            playerDimensionController.EnableProjection(currentProjectionSurface,
                closestPointOnBounds);
            playerDimensionController.UpdateProjectionPosition(
                currentProjectionSurface,
                closestPointOnBounds);
        }
        else
        {
            playerDimensionController.DisableProjections();
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
            playerDimensionController.DisableProjections();
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                playerBehaviour.AddObjectToInRangeList(tObject);
            }
        }
        //tell projection to enable
        else if (other.gameObject.layer == LayerInfo.WALL) {
           // if (other.gameObject.GetComponent<WallBehaviour>().AllowsDimensionTransition){
                //print("adding " + other);

                if (potentialProjectionSurfaces.Contains(other)) return;
                potentialProjectionSurfaces.Add(other);
            }
            
            //potentialProjectionSurfaces = potentialProjectionSurfaces.Distinct().ToList();

        //}
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                playerBehaviour.RemoveObjectFromRangeList(tObject);
            }
        }
        //tell projection to disasble
        else if (other.gameObject.layer == LayerInfo.WALL) {
         //   print("its exiting");
            if (other.gameObject.GetComponent<WallBehaviour>().AllowsDimensionTransition)
            {
                //print("exiting " + other.gameObject.GetComponent<WallBehaviour>());
                
                if (movement.currentWall == other.gameObject.GetComponent<WallBehaviour>())
                {
                   movement.currentWall = null;
               }
                potentialProjectionSurfaces.Remove(other);
            }
        }
    }
    public void clearsurfaces()
    {
        potentialProjectionSurfaces.Clear();
    }
}
