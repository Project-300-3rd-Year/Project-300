using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility 
{
    public static void RotateTwoObjectsTowardsTarget(GameObject gameObject1, GameObject gameObject2, Quaternion object1TargetRotation, Quaternion object2TargetRotation, float rotationSpeed)
    {
        if (Quaternion.Angle(gameObject1.transform.rotation, object1TargetRotation) > 0
               || Quaternion.Angle(gameObject2.transform.rotation, object2TargetRotation) > 0)
        {
            gameObject1.transform.rotation = Quaternion.RotateTowards(gameObject1.transform.rotation, object1TargetRotation, rotationSpeed * Time.deltaTime);
            gameObject2.transform.rotation = Quaternion.RotateTowards(gameObject2.transform.rotation, object2TargetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public static IEnumerator RotateTwoObjectsToTargetPosition(GameObject gameObject1, GameObject gameObject2, Quaternion object1TargetRotation, Quaternion object2TargetRotation, float rotationSpeed)
    {

        while (Quaternion.Angle(gameObject1.transform.rotation, object1TargetRotation) > 0
               || Quaternion.Angle(gameObject2.transform.rotation, object2TargetRotation) > 0)
        {
            gameObject1.transform.rotation = Quaternion.RotateTowards(gameObject1.transform.rotation, object1TargetRotation, rotationSpeed * Time.deltaTime);
            gameObject2.transform.rotation = Quaternion.RotateTowards(gameObject2.transform.rotation, object2TargetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
