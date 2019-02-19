namespace Floyd_WarshallAlgorithm.Model
{
    public class Edge
    {

        public int Id { get; set; }
        public int Weight { get; set; }
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }

        public Edge(int weight, Vertex startVertex, Vertex endVertex)
        {
            Weight = weight;
            StartVertex = startVertex;
            EndVertex = endVertex;
        }
    }
}
