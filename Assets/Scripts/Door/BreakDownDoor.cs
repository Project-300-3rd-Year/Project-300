using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartBreakingDownSequence()
    {
        if(!SequenceIsActive)
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

        print(Quaternion.Angle(gameObject.transform.localRotation, targetRotation));
        while (Quaternion.Angle(gameObject.transform.localRotation, targetRotation) > 4f) //Anything less tends to overshoot if the open speed is fast.
        {
            print("rotating");
            //print(Quaternion.Angle(gameObject.transform.localRotation, targetRotation));
            rb.AddRelativeTorque(gameObject.transform.up * forceToApply * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        rb.isKinematic = true;
    }
}
