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


    //public static IEnumerator RotateObjectToTargetPosition(GameObject gameObject, Quaternion objectTargetRotation, float rotationSpeed)
    //{
    //    //Rigidbody rb = gameObject.GetComponent<Rigidbody>();


    //    //while (Quaternion.Angle(gameObject.transform.rotation, objectTargetRotation) > 0)
    //    //{
    //    //    //gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, objectTargetRotation, rotationSpeed * Time.deltaTime);
    //    //    Debug.Log("rotating");
    //    //    rb.AddRelativeTorque(Vector3.up * rotationSpeed, ForceMode.VelocityChange);
    //    //    //doorRigidbody.AddRelativeTorque(gameObjectToAffect.transform.up * affectSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime, ForceMode.VelocityChange);



    //    //    //gameObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    //    //    yield return null;
    //    //}
    //}
}
