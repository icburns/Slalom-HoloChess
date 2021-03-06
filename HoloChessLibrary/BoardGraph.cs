﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HoloChessLibrary
{
	public class BoardGraph
	{
		public const double CenterSpaceBoundingRadius = .1;
		public const double InnerRingBoundingRadius = .3;
		public const double OuterRingBoundingRadius = .45;

        // node positions need to adjust for the table scale.
        // if the table scale changes, we'll have to apply that scale to each node coordinate
	    private const float TableScale = 0.1332398f;


        public List<Node> Nodes { get; set; }
		public Dictionary<NodeMapKey, List<NodePath>> NodeMap { get; set; }

		public BoardGraph()
		{
            List<Node> excludedNodes = new List<Node>();

            Nodes = GenerateNodes();

            BuildGraph(Nodes);

            NodeMap = BuildNodeMap(Nodes, excludedNodes);

        }

        public BoardGraph(List<Node> excludedNodes)
	    {

            Nodes = GenerateNodes();

			BuildGraph(Nodes);

			NodeMap = BuildNodeMap(Nodes, excludedNodes);
	        
	    }

		private static List<Node> GenerateNodes()
		{
			List<Node> nodes = new List<Node>();

			for (int i = 0; i < 25; i++)
			{
				nodes.Insert(i, new Node(i));
				//assign x and y coordinate for each node
			}

			return nodes;
		}

		private void BuildGraph(IList<Node> nodes)
		{
			nodes[0].XPosition = 0;
			nodes[0].YPosition = 0;

			//inner spaces
			for (int i = 1; i < 13; i++)
			{
				const int innerNode = 0;
				int ccwNode = (i + 10) % 12 + 1;
				int cwNode = i % 12 + 1;
				int outerNode = i + 12;

				AddNodeCoordinates(nodes[i], CenterSpaceBoundingRadius, InnerRingBoundingRadius);

				//add inner circle to center node
				nodes[0].AdjacentNodes.Add(nodes[i]);

				nodes[i].AdjacentNodes.Add(nodes[innerNode]);
				nodes[i].AdjacentNodes.Add(nodes[ccwNode]);
				nodes[i].AdjacentNodes.Add(nodes[cwNode]);
				nodes[i].AdjacentNodes.Add(nodes[outerNode]);
			}

			//outer circle spaces
			for (int i = 13; i < 25; i++)
			{
				int innerNode = i - 12;
				int ccwNode = (i + 10) % 12 + 13;
				int cwNode = i % 12 + 13;

				AddNodeCoordinates(nodes[i], InnerRingBoundingRadius, OuterRingBoundingRadius);

				nodes[i].AdjacentNodes.Add(nodes[innerNode]);
				nodes[i].AdjacentNodes.Add(nodes[ccwNode]);
				nodes[i].AdjacentNodes.Add(nodes[cwNode]);
			}
		}

		private static void AddNodeCoordinates(Node node, double innerBoundingRadius, double outerBoundingRadius)
		{
			//75,45,15,-15....
			double angle = Math.PI / 12 * (7 - 2 * (node.Id % 12));

			double x = Math.Cos(angle) * ((innerBoundingRadius + outerBoundingRadius) / 2.0);
			double y = Math.Sin(angle) * ((innerBoundingRadius + outerBoundingRadius) / 2.0);

			node.XPosition = -(float)x/TableScale;
			node.YPosition = -(float)y/TableScale;
		}

		private Dictionary<NodeMapKey, List<NodePath>> BuildNodeMap(List<Node> nodes, List<Node> excludedNodes)
		{
			Dictionary<NodeMapKey, List<NodePath>> nodeMap = new Dictionary<NodeMapKey, List<NodePath>>();

		    foreach (Node node in nodes)
		    {
		        node.AdjacentNodes.RemoveAll(n => excludedNodes.Select(e => e.Id).Contains(n.Id));
		    }

		    foreach (Node node in nodes)
			{
				nodeMap = nodeMap.Concat(BuildMapForNode(nodes.Where(n => !excludedNodes.Select(e => e.Id).Contains(n.Id)).ToList(), node)).ToDictionary(x => x.Key, x => x.Value);
			}

			return nodeMap;
		}


		// Well, it certainly ain't pretty, but it's past my bed time - ianb 20160831
		private Dictionary<NodeMapKey, List<NodePath>> BuildMapForNode(List<Node> nodes, Node sourceNode)
		{
			List<Node> unvisitedNodes = new List<Node>();
			Dictionary<Node, int> shortestDistanceToNode = new Dictionary<Node, int>();
			Dictionary<Node, Node> previousNodeAlongShortestPath = new Dictionary<Node, Node>();

		    if (!nodes.Contains(sourceNode))
		    {
                shortestDistanceToNode.Add(sourceNode, 0);
                previousNodeAlongShortestPath.Add(sourceNode, null);
                unvisitedNodes.Add(sourceNode);
            }

            foreach (Node node in nodes)
			{
				shortestDistanceToNode.Add(node, int.MaxValue);
				previousNodeAlongShortestPath.Add(node, null);
				unvisitedNodes.Add(node);
			}

			while (unvisitedNodes.Any())
			{
				unvisitedNodes.Sort((x, y) => shortestDistanceToNode[x] - shortestDistanceToNode[y]);
				Node currentNode = unvisitedNodes[0];
				unvisitedNodes.Remove(currentNode);

				foreach (Node adjacentNode in currentNode.AdjacentNodes)
				{
					int currentPathDistance = shortestDistanceToNode[currentNode] + 1;
					if (currentPathDistance < shortestDistanceToNode[adjacentNode] && currentPathDistance > 0)
					{
						shortestDistanceToNode[adjacentNode] = currentPathDistance;
						previousNodeAlongShortestPath[adjacentNode] = currentNode;
					}
                }
            }

			Dictionary<NodeMapKey, List<NodePath>> shortestPathMap = new Dictionary<NodeMapKey, List<NodePath>>();

			foreach (Node node in nodes)
			{
				int distance = 0;
			    NodePath shortestPath = new NodePath
			    {
			        DestinationNode = node
			    };

                Node currentPathNode = node;
				while (previousNodeAlongShortestPath[currentPathNode] != null)
				{
					distance++;
					shortestPath.PathToDestination.Insert(0, currentPathNode);
					currentPathNode = previousNodeAlongShortestPath[currentPathNode];
				}

				NodeMapKey currentTupleKey = new NodeMapKey(sourceNode.Id, distance);

				if (!shortestPathMap.Keys.Any(k => k.Equals(currentTupleKey)))
				{
					shortestPathMap.Add(currentTupleKey, new List<NodePath>());
				}

				shortestPathMap[currentTupleKey].Add(shortestPath);

			}

			return shortestPathMap;
		}
	}
}
