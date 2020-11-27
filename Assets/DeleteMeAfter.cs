using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMeAfter : MonoBehaviour
{
    Rigidbody rb;

    public float gg;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddRelativeTorque(gameObject.transform.up * gg * Time.deltaTime, ForceMode.VelocityChange);
        transform.Rotate(gameObject.transform.up, gg);
    }
}
