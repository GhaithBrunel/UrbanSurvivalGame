using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public Vector3 position;
    public List<Road> connectedRoads;

    public Intersection(Vector3 position)
    {
        this.position = position;
        connectedRoads = new List<Road>();
    }
}
