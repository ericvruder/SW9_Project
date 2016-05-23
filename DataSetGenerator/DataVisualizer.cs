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
        public static void DrawHitBox(List<Attempt> attempts, string fileName) {
            Bitmap hitbox = DrawHitBox(attempts);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(DataGenerator.DataDirectory + fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    hitbox.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            
            //hitbox.Save(DataGenerator.DataDirectory + fileName);
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

        public static void DrawHeatMap(List<Attempt> attempts, GridSize size, out MemoryStream stream) {
            stream = new MemoryStream();
            Bitmap heatMap = HeatMap(attempts, size);
            heatMap.Save(stream, ImageFormat.Png);
            heatMap.Dispose();
        }

        public static void DrawHeatMap(List<Attempt> attempts, GridSize size, string filename) {
            Bitmap heatMap = HeatMap(attempts, size);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(DataGenerator.DataDirectory + filename, FileMode.Create, FileAccess.ReadWrite))
                {
                    heatMap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            //heatMap.Save(DataGenerator.DataDirectory + filename);
            heatMap.Dispose();
        }


        private static Bitmap HeatMap(List<Attempt> attempts, GridSize size) {
            // Color red to green -> http://stackoverflow.com/questions/6394304/algorithm-how-do-i-fade-from-red-to-green-via-yellow-using-rgb-values

            int width = size == GridSize.Large ? 10 : 20;
            int height = size == GridSize.Large ? 5 : 10;

            int sqSize = size == GridSize.Large ? 40 : 20;

            Bitmap heatMap = new Bitmap(400,200);
            Graphics map = Graphics.FromImage(heatMap);

            var list = new List<Tuple<float, int, int>>();
            map.FillRectangle(Brushes.White, 0, 0, 400, 200);
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    var cellAttempts = attempts.Where(x => x.TargetCell.X == j && x.TargetCell.Y == i).ToList();
                    float sum = cellAttempts.Sum(x => x.Hit ? 1 : 0);
                    float count = cellAttempts.Count();
                    float percent = sum / count;
                    if(count != 0) {
                        list.Add(new Tuple<float, int, int>(percent, j, i));
                    }
                }
            }
            list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            
            int colorDegree = (int)Math.Round(255f / (list.Count() * 0.5)) - 1;
            int half = (int)Math.Round((list.Count() * 0.5));
            foreach(var cell in list) {
                var rectangle = new Rectangle(cell.Item2 * sqSize, cell.Item3 * sqSize, sqSize, sqSize);

                int value = (int)Math.Round(cell.Item1 * 255f);
                SolidBrush brush = new SolidBrush(Color.FromArgb(255, value, 0));
                map.FillRectangle(brush, rectangle);

            }
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    var rectangle = new Rectangle(j * sqSize, i * sqSize, sqSize, sqSize);
                    map.DrawRectangle(Pens.Black, rectangle);
                }
            }
            map.Save();
            /*
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

                    float sum = (float)cellAttempts.Sum(x => x.Hit ? 1 : 0);
                    float count = (float)cellAttempts.Count();
                    float percentage =  sum / count;
                    Brush brush = Brushes.White;
                    if(cellAttempts.Count() != 0) {

                        float adjust = percentage < 0.75 ? 0.5f : 0.75f;

                        int value = (int)Math.Round(255f / 0.25f * (percentage - adjust));
                        brush = percentage < 0.75f ? new SolidBrush(Color.FromArgb(255, value, 0)) : new SolidBrush(Color.FromArgb(255, 255, 255 - value));
                    }
                    map.FillRectangle(brush, rectangle);
                    map.DrawRectangle(Pens.Black, rectangle);
                }
            } */

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

            foreach(var t in DataGenerator.AllTechniques) {
                DrawHitBox(techAttempts[t], t.ToString() + ".png");
            }

            foreach (var s in DataGenerator.AllSizes) {
                DrawHitBox(sizeAttempts[s], s.ToString() + ".png");
            }

            foreach(var s in DataGenerator.AllSizes) {
                foreach(var t in DataGenerator.AllTechniques) {
                    var attempts = from attempt in techAttempts[t]
                                   where attempt.Size == s
                                   select attempt;
                    DrawHitBox(attempts.ToList(), t.ToString() + s.ToString() + ".png");
                }
            }

        }

        public static void DrawBellCurve() {

        }

    }
}
