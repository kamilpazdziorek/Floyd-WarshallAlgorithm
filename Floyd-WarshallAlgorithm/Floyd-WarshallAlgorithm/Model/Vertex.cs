using System.Collections.Generic;

namespace Floyd_WarshallAlgorithm.Model
{
    public class Vertex
    {

        public int Id { get; set; }
        public double CoordinatesX { get; set; }
        public double CoordinatesY { get; set; }
        public string Name { get; set; }
        public List<Edge> Edges { get; set; }

        public Vertex(double coordinatesX, double coordinatesY, string name)
        {
            CoordinatesX = coordinatesX;
            CoordinatesY = coordinatesY;
            Name = name;
            Edges = new List<Edge>();

        }

    }
}
