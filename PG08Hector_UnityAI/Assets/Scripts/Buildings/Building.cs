using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Building : BaseObject {

    private Bounds globalBounds {
        get {
            Bounds b = nodeBounds;
            b.center += transform.position;
            return b;
        }
    }
    private List<GraphNode> nodes;
    private uint penalty;
    private const int penaltyMultiplier = 100;

    public Bounds nodeBounds;

    void Start() {
        penalty = (uint)health * penaltyMultiplier;
        //graphs is an array of all the Navgraphs in our game, we only have one though and we know its a GridGraph
        GridGraph graph = (GridGraph)AstarPath.active.graphs[0];
        //We get the nodes within the bounds of this object
        nodes = graph.GetNodesInArea(globalBounds);
        foreach (GraphNode node in nodes) {
            //We add penalty (multiple buildings could be placed on top of each other)
            node.Penalty += penalty;
            GameManager.instance.nodeToBuilding[node] = this;
        }
    }

    void OnDestroy() {
        foreach (GraphNode node in nodes) {
            //We deduct penalty (multiple buildings could be placed on top of each other)
            node.Penalty -= penalty;
        }
    }

    //This will draw these gizmos when this object is selected
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(globalBounds.center, globalBounds.extents);
        //Gizmos.DrawWireCube(globalBounds.center, globalBounds.size);
    }

}
