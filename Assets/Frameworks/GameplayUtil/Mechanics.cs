using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuriousPlay
{
    public class Mechanics
    {
        public static void MoveDirection(Transform target,Vector3 direction,float speed)
        {
            target.position += direction.normalized * speed * Time.deltaTime;
        }

        public static void MovePosition(Transform target, Vector3 position, float speed)
        {
            target.position = Vector3.Lerp(target.position, position,speed * Time.deltaTime);
        }

        public static void SpeedFixedDeltaTime(Transform target,Vector3 direction, float speed)
        {
            target.position += direction.normalized * speed * Time.deltaTime;
        }

    }
}
