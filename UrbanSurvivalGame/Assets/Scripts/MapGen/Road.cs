using UnityEngine;
using System.Collections.Generic;

public class Road
{
    public Vector3 Node1 { get; set; }
    public Vector3 Node2 { get; set; }

    public Road(Vector3 node1, Vector3 node2)
    {
        Node1 = node1;
        Node2 = node2;
    }
}




