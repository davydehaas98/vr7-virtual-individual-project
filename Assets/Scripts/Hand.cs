using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean GrabAction = null;

    private SteamVR_Behaviour_Pose Pose = null;
    private FixedJoint Joint = null;

    private Interactable CurrentInteractable = null;
    private List<Interactable> ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
        Joint = GetComponent<FixedJoint>();
    }
    private void FixedUpdate()
    {
        // Down
        if (GrabAction.GetStateDown(Pose.inputSource))
        {
            print(Pose.inputSource + " Trigger Down");
            Pickup();
        }

        // Up
        if (GrabAction.GetStateUp(Pose.inputSource))
        {
            print(Pose.inputSource + " Trigger Up");
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Interactable>())
            return;

        ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Interactable>())
            return;

        ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void Pickup()
    {
        // Get nearest
        CurrentInteractable = GetNearestInteractable();

        // Null check
        if (!CurrentInteractable)
            return;

        // Already held check
        if (CurrentInteractable.ActiveHand)
            CurrentInteractable.ActiveHand.Drop();

        // Position
        CurrentInteractable.transform.position = transform.position;

        // Attach
        Rigidbody targetBody = CurrentInteractable.GetComponent<Rigidbody>();
        Joint.connectedBody = targetBody;

        // Set active hand
        CurrentInteractable.ActiveHand = this;
    }

    public void Drop()
    {
        // Null check
        if (!CurrentInteractable)
            return;

        // Apply velocity
        Rigidbody targetBody = CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = Pose.GetVelocity();
        targetBody.angularVelocity = Pose.GetAngularVelocity();

        // Detach
        Joint.connectedBody = null;

        // Clear
        CurrentInteractable.ActiveHand = null;
        CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach (Interactable interactable in ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
