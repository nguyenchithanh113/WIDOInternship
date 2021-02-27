using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBreak : MonoBehaviour
{
    public float force = 100f;
    float plusVar = 2.5f;
    public float Torque = 50f;
    public void Breaking(Vector2 expodePoint)
    {
        foreach(Transform bodyPart in transform)
        {
            Vector2 direction = ((Vector2)bodyPart.position - (Vector2)expodePoint).normalized;

            bodyPart.GetComponent<Rigidbody2D>().AddForce(direction * force * PlusForce(bodyPart.position, expodePoint));
            bodyPart.GetComponent<Rigidbody2D>().AddTorque(Torque);
        }
    }
    float PlusForce(Vector2 from, Vector2 to)
    {
        float dis = (from - to).sqrMagnitude;
        dis = plusVar * plusVar - dis;
        if(dis < 0)
        {
            dis = 1;
        }
        return dis;
        
    }
}
