using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationVelocity = 1f;

    void Update()
    {

        transform.Rotate(Vector3.up, Time.deltaTime * rotationVelocity);
    }
}
