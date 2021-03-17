using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreakDownDoor : MonoBehaviour
{
    //Components.
    //[SerializeField] private DoorHandle doorHandle;
    Rigidbody rigidbody;

    [Header("Breaking Open Door Sequence By Enemy")]
    [SerializeField] private float doorAffectSpeed;
    public bool SequenceIsActive;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void StartBreakingDownSequence()
    {
        if(!SequenceIsActive)
            StartCoroutine(EnemyHittingDoorSequence());
    }

    //Leaving this here for now, when enemy ai reaches a closed door this is code that could be called to force it open.
    IEnumerator EnemyHittingDoorSequence()
    {
        //Variables that would be needed for this -
        //public AudioClip[] possibleSmashDoorSounds;
        //public AudioClip[] possibleHitDoorSounds;
        //AudioSource audioSource;

        SequenceIsActive = true;
        print("start of sequence");

        System.Random rng = new System.Random();
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 15f, 0)), doorAffectSpeed));
        //AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0,possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -doorAffectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 15f, 0)), doorAffectSpeed));
        //AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0, possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -doorAffectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        //AudioSource.PlayClipAtPoint(possibleSmashDoorSounds[rng.Next(0, possibleSmashDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 90f, 0)), doorAffectSpeed));

        SequenceIsActive = false;
        rigidbody.isKinematic = false;
        print("end of sequence");

    }

    //Make sure to apply a negative or positive force depending on which way you want the door to move.
    IEnumerator ApplyForceToDoorUntilItReachesAngle(Quaternion targetRotation, float forceToApply)
    {
        rigidbody.isKinematic = false;

        print(Quaternion.Angle(gameObject.transform.localRotation, targetRotation));
        while (Quaternion.Angle(gameObject.transform.localRotation, targetRotation) > 4f) //Anything less tends to overshoot if the open speed is fast.
        {
            print("rotating");
            //print(Quaternion.Angle(gameObject.transform.localRotation, targetRotation));
            rigidbody.AddRelativeTorque(gameObject.transform.up * forceToApply * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        rigidbody.isKinematic = true;
    }
}
