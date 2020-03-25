﻿using MonoGame.Entity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Tiled;
//using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Graph
{
    public class Graph
    {
        public static readonly double INFINITY = System.Double.MaxValue;
        private Dictionary<Vector2, Node> nodeMap;

        public Graph(TiledMap map, List<StaticGameEntity> staticGameEntities) // need extended
        {
            nodeMap = new Dictionary<Vector2, Node>();

            // Construct the nodes based on the map
            for (int i = 0, y = 0; map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++, i++)
                {
                    if (map.TileLayers[0].Tiles[i].GlobalIdentifies == 8 && map.TileLayers[1].Tiles[i].GlobalIdentifier == 0)
                    {
                        GetNode(new Vector2(x * 16 + 8, y * 16 + 8));
                    }
                }
            } // Left here

            // Remove nodes near entities
            foreach (StaticGameEntity e in staticGameEntities) // StaticGameEntity
            {
                Vector2 vector = GetNearestNode(e.Pos);

                int x = (int)vector.X;
                int y = (int)vector.Y;

                int height = e.TextureHeight;
                int width = e.TextureWidth;

                for (int i = x; i < (x + height); i += 16)
                {
                    for (int j = y; j < (y + width); j += 16)
                    {
                        Vector2D v = new Vector2D(i, j);

                        if (nodeMap.ContainsKey(v))
                        {
                            nodeMap.Remove(v);
                        }
                    }
                }
            }

            foreach (Node node in nodeMap.Values)
            {
                Vector2 coordinate = node.coordinate;
                Vector2 right = new Vector2(coordinate.X + 16F, coordinate.Y);
                Vector2 rightUp = new Vector2(coordinate.X + 16F, coordinate.Y - 16F);
                Vector2 rightDown = new Vector2(coordinate.X + 16F, coordinate.Y + 16F);
                Vector2 down = new Vector2(coordinate.X, coordinate.Y + 16F);

                if (nodeMap.ContainsKey(right))
                    AddEdge(coordinate, right, 1); // AddEdge
                if (nodeMap.ContainsKey(rightUp))
                    AddEdge(coordinate, rightUp, 2);
                if (nodeMap.ContainsKey(rightDown))
                    AddEdge(coordinate, rightDown, 2);
                if (nodeMap.ContainsKey(down))
                    AddEdge(coordinate, down, 1);
            }
        }
        public Node GetNode(Vector2 coordinate)
        {
            Node node;

            if (nodeMap.TryGetValue(coordinate, out node)) // TryGetValue
            {
                return node;
            }
            else
            {
                node = new Node(coordinate);
                nodeMap.Add(coordinate, node);

                return node;
            }
        }

        public Vector2D GetNearestNode(Vector2 position)
        {
            return new Vector2(((int)position.X) / 16 * 16 + 8, ((int)position.Y) / 16 * 16 + 8);
        }

        public void AddEdge(Vector2 sourceNode, Vector2 destinationNode, double cost)
        {
            Node lhsNode = GetNode(sourceNode);
            Node rhsNode = GetNode(destinationNode);

            lhsNode.edges.AddLast(new Edge(rhsNode, cost, 0));
            rhsNode.edges.AddLast(new Edge(lhsNode, cost, 0));
        }

        public void ClearAll()
        {
            foreach (Node node in nodeMap.Values)
            {
                node.Reset();
            }
        }

        public LinkedList<Node> AStar(Vector2D startPoint, Vector2D destinationPoint)
        {
            ClearAll();

            Node startNode;

            // Create a starting point
            if (!nodeMap.TryGetValue(GetNearestNode(startPoint), out startNode))
            {
                return new LinkedList<Node>();
            }

            Node destinationNode;

            // Create a ending point
            if (!nodeMap.TryGetValue(GetNearestNode(destinationPoint), out destinationNode))
            {
                destinationNode = startNode;
            }

            // Create priorityqueue of edges
            PriorityQueue<Edge> pq = new PriorityQueue<Edge>();
            pq.Add(new Edge(startNode, 0, 0));
            startNode.dist = 0;

            int visitedNodes = 0;

            while (pq.Size() > 0 && visitedNodes < nodeMap.Count)
            {
                Edge path = pq.Remove();

                Node pathNode = path.toNode;

                if (pathNode.scratch != 0)
                {
                    continue;
                }

                pathNode.scratch = 1;
                visitedNodes++;


                if (pathNode.coordinate == destinationNode.coordinate)
                {
                    LinkedList<Node> nodes = new LinkedList<Node>();
                    Node newNode = pathNode;
                    nodes.AddLast(newNode);

                    while (newNode.prev != null)
                    {
                        newNode.drawable = true;
                        nodes.AddLast(newNode.prev);
                        newNode = newNode.prev;
                    }
                    return nodes;
                }

                foreach (Edge edge in pathNode.edges)
                {
                    Node destNode = edge.toNode;

                    double edgeCost = pathNode.dist + edge.gCost;

                    if (destinationNode.dist > edgeCost)
                    {
                        double heuresticX = Math.Abs(destinationNode.coordinate.X - destNode.coordinate.X);
                        double heuresticY = Math.Abs(destinationNode.coordinate.Y - destNode.coordinate.Y);
                        double edgeHeuresticCost = heuresticX + heuresticY;

                        destinationNode.dist = edgeCost;
                        destinationNode.prev = pathNode;

                        pq.Add(new Edge(destNode, destNode.dist, edgeHeuresticCost));
                    }
                }
            }
            return default;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Node node in nodeMap.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    if (node.drawable == true && edge.toNode.drawable == true)
                        sb.DrawLine(node.coordinate, edge.toNode.coordinate, Color.Red);
                    else if (edge.toNode.scratch != 0)
                        sb.DrawLine(node.coordinate, edge.toNode.coordinate, Color.Yellow);
                    else
                        sb.DrawLine(node.coordinate, edge.toNode.coordinate, Color.Green);
                }
            }

            foreach (Node node in nodeMap.Values)
            {
                if (node.drawable == true)
                    sb.DrawCircle(node.coordinate, 3F, 12, Color.Red, 3f);
                else if (node.scratch != 0)
                    sb.DrawCircle(node.coordinate, 2F, 12, Color.Yellow, 3F);
                else
                    sb.DrawCircle(node.coordinate, 2F, 12, Color.Green, 3F);
            }

            sb.End();
        }
    }
}
