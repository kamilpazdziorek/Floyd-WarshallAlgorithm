using Floyd_WarshallAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Floyd_WarshallAlgorithm.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region fields

        CanvasState canvasState;
        List<Vertex> vertices;
        Vertex FirstVertex;
        Vertex SecondVertex;
        Vertex draggingVertext;
        Ellipse draggingEllipse;
        List<Line> draggingLines;
        private static double XPositionCliked;
        private static double YPositionCliked;
        private static double XDistanceToCenter = 0;
        private static double YDistanceToCenter = 0;

        #endregion

        #region ICommands

        public ICommand AddVertexCommand { get { return new RelayCommand(o => AddVertex()); } }
        public ICommand AddEdgeCommand { get { return new RelayCommand(o => AddEdge()); } }
        public ICommand ResetCommand { get { return new RelayCommand(o => Reset(o)); } }
        public ICommand AuthorsCommand { get { return new RelayCommand(o => Authors()); } }
        public ICommand CreateMatrixCommand { get { return new RelayCommand(o => CreateMatrix()); } }
        public ICommand CanvasActionCommand { get { return new RelayCommand(o => CanvasAction(o)); } }
        public ICommand MouseMoveCommand { get { return new RelayCommand(o => MouseMove(o)); } }
        public ICommand MouseLeftButtonUpCommand { get { return new RelayCommand(o => MouseLeftButtonUp(o)); } }

        #endregion

        public MainWindowViewModel()
        {
            draggingLines = new List<Line>();
            vertices = new List<Vertex>();
            CityName = "Podaj miasto";
            EdgeWeight = 1;
        }

        #region propertisy

        public string CityName { get; set; }
        public int EdgeWeight { get; set; }
        public Cursor Cursor
        {
            get { return _cursor; }
            set { _cursor = value; OnPropertyChanged("Cursor"); }
        }
        private Cursor _cursor;

        #endregion

        #region methods

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void Authors()
        {
            View.AuthorsView authors = new View.AuthorsView();
            authors.ShowDialog();
        }

        private void AddVertex()
        {
            canvasState = CanvasState.ReadyToAddVertex;
            Cursor = Cursors.Cross;
        }

        private void AddEdge()
        {
            canvasState = CanvasState.ReadyToAddEdge;
            Cursor = Cursors.Cross;
        }

        private void Reset(object o)
        {
            canvasState = CanvasState.StartingValue;
            vertices.Clear();
            FirstVertex = null;
            SecondVertex = null;
            Cursor = Cursors.Arrow;
            (o as Canvas).Children.Clear();

        }

        private void CanvasAction(object obj)
        {
            double XPosition = Mouse.GetPosition(Application.Current.MainWindow).X - 45;
            double YPosition = Mouse.GetPosition(Application.Current.MainWindow).Y - 45;

            XPositionCliked = XPosition;
            YPositionCliked = YPosition;


            //zmiana pozycji wierzchołka
            if (canvasState == CanvasState.StartingValue)
            {
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition - x.CoordinatesX) <= 15 && Math.Abs(YPosition - x.CoordinatesY) <= 15);
                if (vertex == null) return;

                canvasState = CanvasState.DragingVertex;
                draggingVertext = vertex;
            }

            //dodanie wierzchołka
            else if (canvasState == CanvasState.ReadyToAddVertex)
            {

                Ellipse ellipse = new Ellipse()
                {
                    Width = 30,
                    Height = 30,
                    Stroke = new SolidColorBrush(Colors.BurlyWood),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Colors.LightYellow)
                };
                (obj as Canvas).Children.Add(ellipse);
                Canvas.SetLeft(ellipse, XPosition);
                Canvas.SetTop(ellipse, YPosition);
                Cursor = Cursors.Arrow;
                Vertex vertex = new Vertex(XPosition, YPosition, CityName);
                vertices.Add(vertex);
                canvasState = CanvasState.StartingValue;
            }

            //dodanie krawędzi, pierwszy wierzchołek
            else if (canvasState == CanvasState.ReadyToAddEdge)
            {
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition - x.CoordinatesX) <= 15 && Math.Abs(YPosition - x.CoordinatesY) <= 15);
                if (vertex == null) return;

                foreach (var item in (obj as Canvas).Children)
                {
                    var FocusedEllipse = item as Ellipse;
                    if (FocusedEllipse != null && Math.Abs(XPosition - Canvas.GetLeft(FocusedEllipse)) <= 15 && Math.Abs(YPosition - Canvas.GetTop(FocusedEllipse)) <= 15)
                    {
                        FocusedEllipse.Stroke = new SolidColorBrush(Colors.Red);
                    }

                }

                FirstVertex = vertex;
                canvasState = CanvasState.FirstSelected;
            }

            //drugi wierzcholek
            else if (canvasState == CanvasState.FirstSelected)
            {
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition - x.CoordinatesX) <= 15 && Math.Abs(YPosition - x.CoordinatesY) <= 15);
                if (vertex == null) return;
                SecondVertex = vertex;

                foreach (var item in (obj as Canvas).Children)
                {
                    var FocusedEllipse = item as Ellipse;
                    if (FocusedEllipse != null && Canvas.GetTop(FocusedEllipse) == FirstVertex.CoordinatesY && Canvas.GetLeft(FocusedEllipse) == FirstVertex.CoordinatesX)
                    {
                        FocusedEllipse.Stroke = new SolidColorBrush(Colors.BurlyWood);
                    }

                }


                Edge edge = new Edge(EdgeWeight, FirstVertex, SecondVertex);
                if (vertices.FirstOrDefault(x => x == FirstVertex).Edges.FirstOrDefault(x => x.EndVertex == SecondVertex) == null)
                {
                    vertices.FirstOrDefault(x => x == FirstVertex).Edges.Add(edge);
                    vertices.FirstOrDefault(x => x == SecondVertex).Edges.Add(edge);
                    Line line = new Line()
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        X1 = FirstVertex.CoordinatesX + 15,
                        X2 = SecondVertex.CoordinatesX + 15,
                        Y1 = FirstVertex.CoordinatesY + 15,
                        Y2 = SecondVertex.CoordinatesY + 15
                    };
                    Canvas.SetZIndex(line, -1);
                    (obj as Canvas).Children.Add(line);
                }

                Cursor = Cursors.Arrow;
                FirstVertex = null;
                SecondVertex = null;
                canvasState = CanvasState.StartingValue;
            }

        }

        private void CreateMatrix()
        {
            int[,] WeightMatrix = new int[vertices.Count, vertices.Count];
            string[] NamesMatrix = new string[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                NamesMatrix[i] = vertices[i].Name;
                for (int j = 0; j < vertices.Count; j++)
                {
                    WeightMatrix[i, j] = 0;
                }
            }

            //dodanie wag do macierzy
        }

        private void MouseMove(object obj)
        {
            if (canvasState == CanvasState.DragingVertex)
            {
                double CurrentXPosition = Mouse.GetPosition(Application.Current.MainWindow).X - 45;
                double CurrentYPosition = Mouse.GetPosition(Application.Current.MainWindow).Y - 45;

                if (CurrentXPosition > 0 && CurrentXPosition < 700 && CurrentYPosition > 0 && CurrentYPosition < 230)
                {
                    if (draggingEllipse == null)
                    {
                        double XBeforeMove = 0;
                        double YBeforeMove = 0;
                        foreach (var item in (obj as Canvas).Children)
                        {
                            var FocusedEllipse = item as Ellipse;
                            if (FocusedEllipse != null && Math.Abs(CurrentXPosition - Canvas.GetLeft(FocusedEllipse)) <= 15 && Math.Abs(CurrentYPosition - Canvas.GetTop(FocusedEllipse)) <= 15)
                            {
                                FocusedEllipse.Stroke = new SolidColorBrush(Colors.Red);
                                XBeforeMove = Canvas.GetLeft(FocusedEllipse);
                                YBeforeMove = Canvas.GetTop(FocusedEllipse);
                                Canvas.SetZIndex(FocusedEllipse, 1);

                                XDistanceToCenter = XPositionCliked - Canvas.GetLeft(FocusedEllipse);
                                YDistanceToCenter = YPositionCliked - Canvas.GetTop(FocusedEllipse);

                                draggingVertext.CoordinatesX = CurrentXPosition - XDistanceToCenter;
                                draggingVertext.CoordinatesY = CurrentYPosition - YDistanceToCenter;
                                draggingEllipse = FocusedEllipse;

                                Canvas.SetLeft(FocusedEllipse, CurrentXPosition - XDistanceToCenter);
                                Canvas.SetTop(FocusedEllipse, CurrentYPosition - YDistanceToCenter);
                                break;

                            }
                        }
                        foreach (var line in (obj as Canvas).Children)
                        {
                            var FocusedLine = line as Line;
                            if (FocusedLine != null)
                            {
                                if (FocusedLine.X1 == XBeforeMove + 15 && FocusedLine.Y1 == YBeforeMove + 15)
                                {
                                    draggingLines.Add(FocusedLine);
                                    FocusedLine.X1 = draggingVertext.CoordinatesX + 15;
                                    FocusedLine.Y1 = draggingVertext.CoordinatesY + 15;
                                }
                                else if (FocusedLine.X2 == XBeforeMove + 15 && FocusedLine.Y2 == YBeforeMove + 15)
                                {
                                    draggingLines.Add(FocusedLine);
                                    FocusedLine.X2 = draggingVertext.CoordinatesX + 15;
                                    FocusedLine.Y2 = draggingVertext.CoordinatesY + 15;
                                }
                            }
                        }

                    }
                    else
                    {
                        Canvas.SetLeft(draggingEllipse, CurrentXPosition - XDistanceToCenter);
                        Canvas.SetTop(draggingEllipse, CurrentYPosition - YDistanceToCenter);

                        foreach (var line in draggingLines)
                        {
                            if (line.X1 == draggingVertext.CoordinatesX + 15 && line.Y1 == draggingVertext.CoordinatesY + 15)
                            {
                                line.X1 = Canvas.GetLeft(draggingEllipse) + 15;
                                line.Y1 = Canvas.GetTop(draggingEllipse) + 15;
                            }
                            else if (line.X2 == draggingVertext.CoordinatesX + 15 && line.Y2 == draggingVertext.CoordinatesY + 15)
                            {
                                line.X2 = Canvas.GetLeft(draggingEllipse) + 15;
                                line.Y2 = Canvas.GetTop(draggingEllipse) + 15;
                            }
                        }

                        draggingVertext.CoordinatesX = Canvas.GetLeft(draggingEllipse);
                        draggingVertext.CoordinatesY = Canvas.GetTop(draggingEllipse);
                    }
                }
            }
        }

        private void MouseLeftButtonUp(object o)
        {
            if (canvasState == CanvasState.DragingVertex)
            {
                if (draggingEllipse != null)
                {
                    draggingEllipse.Stroke = new SolidColorBrush(Colors.BurlyWood);
                    Canvas.SetZIndex(draggingEllipse, 0);
                }
                draggingLines.Clear();
                draggingVertext = null;
                draggingEllipse = null;
                canvasState = CanvasState.StartingValue;
            }
        }

        private enum CanvasState
        {
            StartingValue,
            DragingVertex,
            ReadyToAddVertex,
            ReadyToAddEdge,
            FirstSelected,
            AddedEdge
        }

        #endregion
    }
}
