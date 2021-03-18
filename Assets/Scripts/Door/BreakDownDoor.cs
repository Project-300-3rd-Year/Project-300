using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is optionally attached to certain doors. 
 * It's resposible for playing a "sequence" where the enemy is attempting to force the door open and this gives the player time to hide in the room that they are in.
 * When the enemy bumps into a door with this script attached, it activates the sequence if the angle of the door is "closed" and the player is in the room.
 * ISSUE - Is door closed method not working correctly for door with "startingYAngle" = 0. Wasn't able to fix in time.
 */

public class BreakDownDoor : MonoBehaviour
{
    //Components.
    Rigidbody rb;

    [Header("Breaking Open Door Sequence By Enemy")]
    [SerializeField] private float doorAffectSpeed;
    public bool SequenceIsActive;

    [Header("Audio To Play When Door Is Hit")]
    [SerializeField] private AudioClip[] possibleHitDoorSounds;
    [SerializeField] private AudioClip[] possibleSmashDoorSounds;

    [Header("Player inside room check")]
    [SerializeField] private Collider roomTriggerCheck;
    private GameObject player;

    //Determining if closed. Done in a bad way.
    private float startingYAngle;
    private float positiveClosedAngle;
    private float negativeClosedAngle;


    //Start.
    private void Awake() => rb = GetComponent<Rigidbody>();
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        startingYAngle = transform.rotation.eulerAngles.y;

        positiveClosedAngle = startingYAngle + 10f;
        negativeClosedAngle = startingYAngle - 10f;
    }

    private bool IsClosed() => gameObject.transform.rotation.eulerAngles.y > negativeClosedAngle && gameObject.transform.rotation.eulerAngles.y < positiveClosedAngle;
    private bool PlayerInRoom()=> roomTriggerCheck.bounds.Contains(player.transform.position);

    public void StartBreakingDownSequence()
    {
        if(!SequenceIsActive && IsClosed() && PlayerInRoom())
            StartCoroutine(EnemyHittingDoorSequence());
    }

    //Leaving this here for now, when enemy ai reaches a closed door this is code that could be called to force it open.
    IEnumerator EnemyHittingDoorSequence()
    {
        SequenceIsActive = true;

        System.Random rng = new System.Random();
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 15f, 0)), doorAffectSpeed));
        AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0,possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -doorAffectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 15f, 0)), doorAffectSpeed));
        AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0, possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -doorAffectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        AudioSource.PlayClipAtPoint(possibleSmashDoorSounds[rng.Next(0, possibleSmashDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 90f, 0)), doorAffectSpeed));

        SequenceIsActive = false;
        rb.isKinematic = false;
    }

    //Make sure to apply a negative or positive force depending on which way you want the door to move.
    IEnumerator ApplyForceToDoorUntilItReachesAngle(Quaternion targetRotation, float forceToApply)
    {
        rb.isKinematic = false;

        while (Quaternion.Angle(gameObject.transform.localRotation, targetRotation) > 4f) //Anything less tends to overshoot if the open speed is fast.
        {
            rb.AddRelativeTorque(gameObject.transform.up * forceToApply * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        rb.isKinematic = true;
    }
}
