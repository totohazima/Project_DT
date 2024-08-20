
using UnityEngine;

public class SM_rotateThis : MonoBehaviour
{
    public new Transform transform;
    public float rotationSpeedX = 90F;
    public float rotationSpeedY = 0F;
    public float rotationSpeedZ = 0F;
    public bool local =true;
    //Vector3 rotationVector = new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ);

    private void Start()
    {
        transform = gameObject.transform;
    }
    void Update()
    {
        if (local == true)
            transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime);
        if (local == false)
            transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime, Space.World);
    }
}