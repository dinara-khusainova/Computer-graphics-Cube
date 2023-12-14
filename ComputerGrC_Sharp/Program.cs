using System;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class MyRenderWindow : RenderWindow
{
    public MyRenderWindow(VideoMode mode, string title) : base(mode, title) { }

    public bool TryPollEvent(out Event eventToFill)
    {
        return PollEvent(out eventToFill);
    }
}
class Edge
{
    public int start, end;

    public Edge(int s, int e)
    {
        start = s;
        end = e;
    }

   
}

class Point
{
    public int x, y, z;

    public Point() { }

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
class Polygon {
    public int point11, point22, point33;
    public Polygon(int point1, int point2, int point3)
    {
        point11 = point1;
        point22 = point2;
        point33 = point3;
    }
}
class Vertex2
{
    public int i, j;
    public Point w;
    public Point e;

    public Vertex2(int x, int y, int z)
    {
        w = new Point(x, y, z);
        e = new Point(x, y, z);
    }

    public void SetViewCoord(double theta, double phi, double ro)
    {
        double st = Math.Sin(theta);
        double ct = Math.Cos(theta);
        double sf = Math.Sin(phi);
        double cf = Math.Cos(phi);

        e.x = (int)((-st * w.x) + (ct * w.y));
        e.y = (int)(-cf * ct * w.x - cf * st * w.y + sf * w.z);
        e.z = (int)(-sf * ct * w.x - sf * st * w.y - cf * w.z + ro);
    }

    public void PrintCoords()
    {
        Console.WriteLine($"World coords: {w.x}, {w.y}, {w.z}");
        Console.WriteLine($"View coords: {e.x}, {e.y}, {e.z}");
        
    }

    public void SetScreenCoords(int ro)
    {
        int x = ro * e.x / (2 * e.z);
        int y = ro * e.y / (2 * e.z);

        int xr = 600, xl = -300, yd = -300, yu = 600;
        int ir = 700, il = 0, jd = 700, ju = 0;

        i = il + ((x - xr) * (ir - il) / (xl - xr));
        j = ju + ((y - yu) * (jd - ju) / (yd - yu));
        Console.WriteLine($"Screen coords: {i}, {j}");
    }
    public static bool Determined(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        int[,] deterMatrix = new int[3, 3] { { x1, y1, 1 }, { x2, y2, 1 }, { x3, y3, 1 } };
        int deter = x1 * y2 * 1 + y1 * 1 * x3 + 1 * x2 * y3 - 1 * y2 * x3 - x1 * 1 * y3 - y1 * x2 * 1;
        if (deter>0)
        {
            return true;
        }
        else return false;
    }
    public static void DrawLine(int x1, int y1, int x2, int y2, RenderWindow window)
    {
        Vertex[] line =
        {
            new Vertex(new Vector2f(x1, y1)),
            new Vertex(new Vector2f(x2, y2))
        };

        window.Draw(line, PrimitiveType.Lines);
    }
}
class Program
{
    
    static void Main()
    {
        const int numberOfPoints = 8;
        const int numberOfEdges = 12;
        const int numberOfPolygons = 12;
        Vertex2[] mas = new Vertex2[numberOfPoints];
        Edge[] edgesInt = new Edge[numberOfEdges];
        Polygon[] polygonsInt = new Polygon[numberOfPolygons];
        double theta = Math.PI / 6;
        double phi = Math.PI / 4;

        int ro = 1000;

        string filePath = @"C:\Users\Dinara\Downloads\Pythagorean-Tree-computer-graphics-main\Pythagorean-Tree-computer-graphics-main\ComputerGrC_Sharp\points.txt";
        MyRenderWindow window = new MyRenderWindow(new VideoMode(1000, 1000), "Perfecto");

        
        window.Clear(new Color(0, 0, 0, 0));
        
        using (StreamReader sr = new StreamReader(filePath))
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                string[] coords = sr.ReadLine().Split(',');
                int x = int.Parse(coords[0].Trim());
                int y = int.Parse(coords[1].Trim());
                int z = int.Parse(coords[2].Trim());
                mas[i] = new Vertex2(x, y, z);
            }
            for (int i = 0; i < numberOfEdges; i++)
            {
                string[] edges = sr.ReadLine().Split(' ');
                int s = int.Parse(edges[0].Trim());
                int e = int.Parse(edges[1].Trim());
                edgesInt[i] = new Edge(s,e);
            }
            for (int i = 0; i < numberOfPolygons; i++) {
                string[] polygons = sr.ReadLine().Split(' ');
                int onee = int.Parse(polygons[0].Trim());
                int twoo = int.Parse(polygons[1].Trim());
                int theee = int.Parse(polygons[2].Trim());
                polygonsInt[i] = new Polygon(onee, twoo, theee);
            }
        }
        for (int i = 0; i < numberOfEdges; i++)
        { }


            while (window.IsOpen)
        {
            
            Event myEvent = new Event();
            while (window.TryPollEvent(out myEvent))
            {
                
                if (myEvent.Type == EventType.Closed)
                {
                    window.Close();
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    theta += 0.01;
                    
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    
                    phi += 0.01;
                }
            }


            window.Clear(new Color(0, 0, 0, 0));
            for (int i = 0; i < numberOfPoints; i++)
            {

                mas[i].SetViewCoord(theta, phi, ro);
                mas[i].SetScreenCoords(ro);
               
            }
            for (int i = 0; i < numberOfPolygons; i++)
            {
                int onee = polygonsInt[i].point11;
                int twoo = polygonsInt[i].point22;
                int theee = polygonsInt[i].point33;
                if (Vertex2.Determined(mas[onee].i, mas[onee].j, mas[twoo].i, mas[twoo].j, mas[theee].i, mas[theee].j)) {
                    Vertex2.DrawLine(mas[onee].i, mas[onee].j, mas[twoo].i, mas[twoo].j, window);
                    Vertex2.DrawLine(mas[twoo].i, mas[twoo].j, mas[theee].i, mas[theee].j, window);
                }
            }
            //for (int i = 0; i < numberOfEdges; i++)
            //{
            //    int start = edgesInt[i].start;
            //    int end = edgesInt[i].end;

            //    Vertex2.DrawLine(mas[start].i, mas[start].j, mas[end].i, mas[end].j, window);
                
            //}
            window.Display();
        }
    }
}
