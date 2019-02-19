using System.Collections.Generic;

namespace Floyd_WarshallAlgorithm.Model
{
    public class Graph
    {

        public int Id { get; set; }
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }


        public Graph(List<Vertex> vertices, List<Edge> edges)
        {
            Vertices = vertices;
            Edges = edges;
        }
    }
}
