using UnityEngine;
using System.Collections;

public class AstronautCollisionBehaviour : MonoBehaviour {

    public AstronautPlay Astronaut { private get; set; }
    void OnCollisionEnter(Collision collision)
    {
        Astronaut.ASTROCOLLISION(collision.relativeVelocity,collision.contacts[0].point);
    }
}
