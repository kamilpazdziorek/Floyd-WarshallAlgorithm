using Floyd_WarshallAlgorithm.Helpers;
using Floyd_WarshallAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Floyd_WarshallAlgorithm.ViewModel
{
    public class MetroWindowViewModel : ViewModelBase
    {
        #region fields
        CanvasState canvasState;
        List<Vertex> vertices;
        Vertex FirstVertex;
        Vertex SecondVertex;
        Vertex draggingVertext;
        Ellipse draggingEllipse;
        List<Line> draggingLines;
        private Visibility _authorsGroupBoxVisibility;
        private Cursor _cursor;
        private SolidColorBrush _canvasBorder;
        private double _minWeight;
        private double _minHeight;
        private static double XPositionCliked;
        private static double YPositionCliked;
        private static double XDistanceToCenter = 0;
        private static double YDistanceToCenter = 0;
        private static double WindowXPosition;
        private static double WindowYPosition;
        #endregion

        #region ICommands
        public ICommand ChangeAuthorsVisibilityCommand { get { return new RelayCommand(o => ChangeAuthorsVisibility()); }}
        public ICommand ExitAppCommand { get { return new RelayCommand(o => ExitApp(o)); }}
        public ICommand AddVertexCommand { get { return new RelayCommand(o => AddVertex()); } }
        public ICommand CanvasActionCommand { get { return new RelayCommand(o => CanvasAction(o)); } }
        public ICommand AddEdgeCommand { get { return new RelayCommand(o => AddEdge()); } }
        public ICommand ResetCommand { get { return new RelayCommand(o => Reset(o)); } }
        public ICommand LocationChangedCommand { get { return new RelayCommand(o => LocationChanged(o)); } }
        public ICommand MouseLeftButtonUpCommand { get { return new RelayCommand(o => MouseLeftButtonUp()); } }
        public ICommand MouseMoveCommand { get { return new RelayCommand(o => MouseMove(o)); } }
        #endregion
        //public ICommand CreateMatrixCommand { get { return new RelayCommand(o => CreateMatrix()); } }

        #region properise
        public int WindowWidth { get; set; }
        public string FirstColumnWidth { get; set; }
        public ObservableCollection<int> NumbersExample { get; set; }
        public ObservableCollection<string> CitiesExample { get; set; }
        public Visibility AuthorsGroupBoxVisibility
        {
            get { return _authorsGroupBoxVisibility; }
            set { _authorsGroupBoxVisibility = value; OnPropertyChanged("AuthorsGroupBoxVisibility"); }
        }
        public Cursor Cursor
        {
            get { return _cursor; }
            set { _cursor = value; OnPropertyChanged("Cursor"); }
        }

        public SolidColorBrush CanvasBorder
        {
            get { return _canvasBorder; }
            set { _canvasBorder = value; OnPropertyChanged("CanvasBorder"); }
        }

        public string CityName { get; set; }
        public int EdgeWeight { get; set; }
        public double MinWeight
        {
            get { return _minWeight; }
            set { _minWeight = value; OnPropertyChanged("MinWeight"); }
        }
        public double MinHeight
        {
            get { return _minHeight; }
            set { _minHeight = value; OnPropertyChanged("MinHeight"); }
        }
        #endregion

        public MetroWindowViewModel()
        {
            WindowXPosition = 0;
            WindowYPosition = 0;
            FirstColumnWidth = "0,15*";
            NumbersExample = Resources.NumbersExamples.numbers;
            CitiesExample = Resources.CitiesExamples.Cities;
            AuthorsGroupBoxVisibility = Visibility.Collapsed;
            draggingLines = new List<Line>();
            vertices = new List<Vertex>();
            MinHeight = 770;
            MinWeight = 1000;
        }

        #region methods

        private void AddVertex()
        {
            canvasState = CanvasState.ReadyToAddVertex;
            Cursor = Cursors.Cross;
            CanvasBorder = Brushes.DeepSkyBlue;
        }

        private void AddEdge()
        {
            canvasState = CanvasState.ReadyToAddEdge;
            Cursor = Cursors.Cross;
            CanvasBorder = Brushes.DeepSkyBlue;
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
            Point point = (obj as Canvas).PointToScreen(new Point(0, 0));

            double XPosition = Mouse.GetPosition(Application.Current.MainWindow).X  - 15;
            double YPosition = Mouse.GetPosition(Application.Current.MainWindow).Y  - 15;

            XPositionCliked = XPosition;
            YPositionCliked = YPosition;


            //zmiana pozycji wierzchołka
            if (canvasState == CanvasState.StartingValue)
            {
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition - point.X - x.CoordinatesX + WindowXPosition) <= 15 && Math.Abs(YPosition - point.Y - x.CoordinatesY + WindowYPosition) <= 15);
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
                Canvas.SetLeft(ellipse, XPosition -point.X + WindowXPosition);
                Canvas.SetTop(ellipse, YPosition -point.Y + WindowYPosition);
                Cursor = Cursors.Arrow;
                Vertex vertex = new Vertex(XPosition - point.X + WindowXPosition, YPosition - point.Y + WindowYPosition, CityName);
                vertices.Add(vertex);
                canvasState = CanvasState.StartingValue;
                CanvasBorder = Brushes.White;

                if (vertex.CoordinatesX + point.X - WindowXPosition> MinWeight)
                {
                    MinWeight = vertex.CoordinatesX + point.X + 40;
                }
                if (vertex.CoordinatesY + point.Y -WindowYPosition >MinHeight)
                {
                    MinHeight = vertex.CoordinatesY + point.Y + 50;
                }
            }

            //dodanie krawędzi, pierwszy wierzchołek
            else if (canvasState == CanvasState.ReadyToAddEdge)
            {
                
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition -point.X - x.CoordinatesX + WindowXPosition) <= 15 && Math.Abs(YPosition -point.Y - x.CoordinatesY + WindowYPosition) <= 15);
                if (vertex == null) return;

                foreach (var item in (obj as Canvas).Children)
                {
                    var FocusedEllipse = item as Ellipse;
                    if (FocusedEllipse != null)
                    {
                        var a = Canvas.GetLeft(FocusedEllipse);
                        if (FocusedEllipse != null && Math.Abs(XPosition - point.X - Canvas.GetLeft(FocusedEllipse) + WindowXPosition) <= 15 && Math.Abs(YPosition - point.Y - Canvas.GetTop(FocusedEllipse) + WindowYPosition) <= 15)
                        {

                            FocusedEllipse.Stroke = new SolidColorBrush(Colors.DeepSkyBlue);
                        }
                    }

                }

                FirstVertex = vertex;
                canvasState = CanvasState.FirstSelected;
            }

            //drugi wierzcholek
            else if (canvasState == CanvasState.FirstSelected)
            {
                Vertex vertex = vertices.FirstOrDefault(x => Math.Abs(XPosition - point.X - x.CoordinatesX + WindowXPosition) <= 15 && Math.Abs(YPosition - point.Y - x.CoordinatesY + WindowYPosition) <= 15);
                if (vertex == null) return;
                SecondVertex = vertex;

                foreach (var item in (obj as Canvas).Children)
                {
                    var FocusedEllipse = item as Ellipse;
                    if (FocusedEllipse != null)
                    {
                        if (FocusedEllipse != null && Canvas.GetTop(FocusedEllipse) == FirstVertex.CoordinatesY && Canvas.GetLeft(FocusedEllipse) == FirstVertex.CoordinatesX)
                        {
                            FocusedEllipse.Stroke = new SolidColorBrush(Colors.BurlyWood);
                            break;
                        }
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
                        X2 = SecondVertex.CoordinatesX  + 15,
                        Y1 = FirstVertex.CoordinatesY  +15 ,
                        Y2 = SecondVertex.CoordinatesY + 15
                    };
                    Canvas.SetZIndex(line, -1);
                    (obj as Canvas).Children.Add(line);
                }

                Cursor = Cursors.Arrow;
                FirstVertex = null;
                SecondVertex = null;
                CanvasBorder = Brushes.White;
                canvasState = CanvasState.StartingValue;
            }
        }

        private void LocationChanged(object obj)
        {
            WindowXPosition = (obj as Window).Left;
            WindowYPosition = (obj as Window).Top;
        }

        private void ChangeAuthorsVisibility()
        {
            if (AuthorsGroupBoxVisibility == Visibility.Visible)
                AuthorsGroupBoxVisibility = Visibility.Collapsed;
            else
                 AuthorsGroupBoxVisibility = Visibility.Visible;

        }

        private void ExitApp(object obj)
        {
            (obj as Window).Close();
        }

        private void MouseLeftButtonUp()
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

        private void MouseMove(object obj)
        {
            if (canvasState == CanvasState.DragingVertex)
            {
                double CurrentXPosition = Mouse.GetPosition(Application.Current.MainWindow).X - 15;
                double CurrentYPosition = Mouse.GetPosition(Application.Current.MainWindow).Y - 15;
                Point point = (obj as Canvas).PointToScreen(new Point(0, 0));

                if (CurrentXPosition > point.X -WindowXPosition && CurrentXPosition < point.X + (obj as Canvas).ActualWidth -WindowXPosition && CurrentYPosition > point.Y -WindowYPosition && CurrentYPosition < point.Y + (obj as Canvas).ActualHeight -WindowYPosition)
                {
                    if (draggingEllipse == null)
                    {
                        double XBeforeMove = 0;
                        double YBeforeMove = 0;

                        foreach (var item in (obj as Canvas).Children)
                        {
                            var FocusedEllipse = item as Ellipse;
                            //TODO
                            if (FocusedEllipse != null && Math.Abs(CurrentXPosition - Canvas.GetLeft(FocusedEllipse) - point.X + WindowXPosition) <= 15 && Math.Abs(CurrentYPosition - Canvas.GetTop(FocusedEllipse) - point.Y + WindowYPosition) <= 15)
                            {
                                FocusedEllipse.Stroke = new SolidColorBrush(Colors.DeepSkyBlue);
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
