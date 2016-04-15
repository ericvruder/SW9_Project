using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace DataSetGenerator {
    public class DataVisualizer {
        private static void DrawHitBox(List<Attempt> attempts, string fileName) {
            Bitmap hitbox = DrawHitBox(attempts);
            hitbox.Save(DataGenerator.DataDirectory + fileName);
            hitbox.Dispose();
        }

        public static void DrawHitBox(List<Attempt> attempts, out MemoryStream stream) {
            stream = new MemoryStream();
            Bitmap hitbox = DrawHitBox(attempts);
            hitbox.Save(stream, ImageFormat.Png);
            hitbox.Dispose();
        }

        private static Bitmap DrawHitBox(List<Attempt> attempts) {

            //61 pixel sized squares, makes it better to look at
            int cellSize = 61;
            int bmsize = cellSize * 3;

            Bitmap hitbox = new Bitmap(bmsize, bmsize);
            Graphics hBGraphic = Graphics.FromImage(hitbox);
            hBGraphic.FillRectangle(Brushes.White, 0, 0, bmsize, bmsize);
            hBGraphic.DrawRectangle(new Pen(Brushes.Black, 1.0f), cellSize, cellSize, cellSize, cellSize);

            foreach (var attempt in attempts) {
                Brush brush = Brushes.Red;
                float scale = attempt.Size == GridSize.Large ? 122.0f : 61.0f;
                Point p = new Point(attempt.TargetCell.X, attempt.TargetCell.Y);
                p.X = p.X * scale; p.Y = p.Y * scale;
                p.X = attempt.Pointer.X - p.X;
                p.Y = attempt.Pointer.Y - p.Y;
                if (attempt.Size == GridSize.Large) {
                    p.X /= 2;
                    p.Y /= 2;
                }

                p.X += cellSize;
                p.Y += cellSize;

                if ((p.X > cellSize && p.X < cellSize * 2) && (p.Y > cellSize && p.Y < cellSize * 2)) {
                    brush = Brushes.Green;
                }

                if (!((p.X < 0) && (p.X >= bmsize)) || !((p.Y < 0) && (p.Y >= bmsize))) {
                    hBGraphic.FillRectangle(brush, (float)p.X, (float)p.Y, 2, 2);
                }
            }

            hBGraphic.Save();
            hBGraphic.Dispose();
            return hitbox;

        }

        /*
        
            gridHeight = height;
            gridWidth = width;
            squareHeight = canvas.ActualHeight / height;
            squareWidth = canvas.ActualWidth / width;

            grid = new Cell[width, height];

            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    grid[i, j] = new Cell(ShapeFactory.CreateGridCell(squareWidth, squareHeight, includeBorders));
                    grid[i, j].X = i; grid[i, j].Y = j;
                    canvas.Children.Add(grid[i, j].GridCell);
                    Canvas.SetBottom(grid[i, j].GridCell, j * squareHeight);
                    Canvas.SetLeft(grid[i, j].GridCell, i * squareWidth);
                    Canvas.SetZIndex(grid[i, j].GridCell, 0);
                }
            }

         */

        public static void DrawHeatMap(List<Attempt> attempts, GridSize size, out MemoryStream stream) {
            stream = new MemoryStream();
            Bitmap heatMap = HeatMap(attempts, size);
            heatMap.Save(stream, ImageFormat.Png);
            heatMap.Dispose();
        }

        public static void DrawHeatMap(List<Attempt> attempts, GridSize size, string filename) {
            Bitmap heatMap = HeatMap(attempts, size);
            heatMap.Save(DataGenerator.DataDirectory + filename);
            heatMap.Dispose();
        }


        private static Bitmap HeatMap(List<Attempt> attempts, GridSize size) {
            // Color red to green -> http://stackoverflow.com/questions/6394304/algorithm-how-do-i-fade-from-red-to-green-via-yellow-using-rgb-values

            int width = size == GridSize.Large ? 10 : 20;
            int height = size == GridSize.Large ? 5 : 10;

            int sqSize = size == GridSize.Large ? 40 : 20;

            Bitmap heatMap = new Bitmap(400,200);
            Graphics map = Graphics.FromImage(heatMap);

            map.FillRectangle(Brushes.White, 0, 0, 100, 100);

            for(int i = 0; i < height; i++) {
                for(int j = 0; j < width; j++) {
                    
                    var rectangle = new Rectangle(j * sqSize, i * sqSize, sqSize, sqSize);
                    var cellAttempts = attempts.Where(x => x.TargetCell.X == j && x.TargetCell.Y == i);

                    /*
                    RGB values for the colors:
                        •Red 255, 0, 0
                        •Yellow 255, 255, 0
                        •Green 0, 255, 0

                    Between Red and Yellow, equally space your additions to the green channel until it reaches 255. Between Yellow and Green, equally space your subtractions from the red channel.
                    */

                    float percentage = cellAttempts.Sum(x => x.Hit ? 1 : 0) / cellAttempts.Count();
                    Brush brush = Brushes.White;
                    if(cellAttempts.Count() != 0) {
                        if(percentage < 0.5) {
                            var t = percentage * 2.0f;
                            int value = (int)Math.Round(255.0f / t);
                            brush = new SolidBrush(Color.FromArgb(255, value, 0));
                        }
                    }
                    map.FillRectangle(brush, rectangle);
                }
            }

            return heatMap;
        }

        public static void CreateHitboxes(DataSource source) {
            var tests = DataGenerator.GetTests(source);
            if (tests.Count == 0) return;
            Dictionary<GestureType, List<Attempt>> techAttempts = new Dictionary<GestureType, List<Attempt>>();
            Dictionary<GridSize, List<Attempt>> sizeAttempts = new Dictionary<GridSize, List<Attempt>>();
            sizeAttempts.Add(GridSize.Large, new List<Attempt>());
            sizeAttempts.Add(GridSize.Small, new List<Attempt>());
            foreach (var test in tests) {
                foreach (var gesture in DataGenerator.AllTechniques) {
                    if (!techAttempts.ContainsKey(gesture)) {
                        techAttempts.Add(gesture, new List<Attempt>());
                    }
                    techAttempts[gesture].AddRange(test.Attempts[gesture]);
                    sizeAttempts[GridSize.Small].AddRange(from attempt in test.Attempts[gesture]
                                                          where attempt.Size == GridSize.Small
                                                          select attempt);
                    sizeAttempts[GridSize.Large].AddRange(from attempt in test.Attempts[gesture]
                                                          where attempt.Size == GridSize.Large
                                                          select attempt);
                }
            }

            DrawHitBox(techAttempts[GestureType.Pinch], "pinch.png");
            DrawHitBox(techAttempts[GestureType.Swipe], "swipe.png");
            DrawHitBox(techAttempts[GestureType.Throw], "throw.png");
            DrawHitBox(techAttempts[GestureType.Tilt], "tilt.png");

            DrawHitBox(sizeAttempts[GridSize.Large], "large.png");
            DrawHitBox(sizeAttempts[GridSize.Small], "small.png");

            var lp = from attempt in techAttempts[GestureType.Pinch]
                     where attempt.Size == GridSize.Large
                     select attempt;
            DrawHitBox(lp.ToList(), "pinchlarge.png");
            var sp = from attempt in techAttempts[GestureType.Pinch]
                     where attempt.Size == GridSize.Small
                     select attempt;
            DrawHitBox(sp.ToList(), "pinchsmall.png");

            var ls = from attempt in techAttempts[GestureType.Swipe]
                     where attempt.Size == GridSize.Large
                     select attempt;
            DrawHitBox(ls.ToList(), "swipelarge.png");
            var ss = from attempt in techAttempts[GestureType.Swipe]
                     where attempt.Size == GridSize.Small
                     select attempt;
            DrawHitBox(ss.ToList(), "swipesmall.png");

            var lti = from attempt in techAttempts[GestureType.Tilt]
                      where attempt.Size == GridSize.Large
                      select attempt;
            DrawHitBox(lti.ToList(), "tiltlarge.png");
            var sti = from attempt in techAttempts[GestureType.Tilt]
                      where attempt.Size == GridSize.Small
                      select attempt;
            DrawHitBox(sti.ToList(), "tiltsmall.png");

            var lth = from attempt in techAttempts[GestureType.Throw]
                      where attempt.Size == GridSize.Large
                      select attempt;
            DrawHitBox(lth.ToList(), "throwlarge.png");
            var sth = from attempt in techAttempts[GestureType.Throw]
                      where attempt.Size == GridSize.Small
                      select attempt;
            DrawHitBox(sth.ToList(), "throwsmall.png");

        }

        public static void DrawBellCurve() {

        }

    }
}
