using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DataSetGenerator {

    class inputstream
    {

        public void addTests(inputstream s)
        {
            int choice = -1;
            int tchoice = -1;
            bool ben = true;
            Console.WriteLine("Select a range that has to be added from and to are included");
            while (ben)
            {
                Console.WriteLine("From:");
                s.s = Console.ReadLine();
                int.TryParse(s.s, out choice);
                if (choice != -1 && tchoice != 0)
                {
                    while (ben)
                    {
                        Console.WriteLine("From = " + choice);
                        Console.WriteLine("To:");
                        s.s = Console.ReadLine();
                        int.TryParse(s.s, out tchoice);

                        if (tchoice != -1 && tchoice != 0)
                        {
                            ben = false;
                            bool bob = true;
                            while (bob)
                            {
                                Console.WriteLine("Proceeding wiil add items " + choice + " to " + tchoice + "both including, on repo: " + s.sds);
                                Console.WriteLine("Do you want to continue? (y) / (n)");
                                s.s = Console.ReadLine();
                                switch (s.s)
                                {
                                    case "y":
                                        bob = false;
                                        List<Test> tests = new List<Test>();
                                        for (int i = choice; i <= tchoice; i++)
                                        {
                                            tests.Add(new Test(i, s.ds));
                                        }
                                        AttemptRepository.SaveTestsToDatabase(tests);
                                        break;
                                    case "n":
                                        bob = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                    }
                }
            }
        }

        public void remTests(inputstream s)
        {
            int choice = -1;
            int tchoice = -1;
            bool ben = true;
            Console.WriteLine("Select a range that has to be removed, from and to are included");
            while (ben)
            {
                Console.WriteLine("From:");
                s.s = Console.ReadLine();
                int.TryParse(s.s, out choice);
                if (choice != -1 && tchoice != 0)
                {
                    while (ben)
                    {
                        Console.WriteLine("From = " + choice);
                        Console.WriteLine("To:");
                        s.s = Console.ReadLine();
                        int.TryParse(s.s, out tchoice);

                        if (tchoice != -1 && tchoice != 0)
                        {
                            ben = false;
                            bool bob = true;
                            while (bob)
                            {
                                Console.WriteLine("Proceeding wiil delete items " + choice + " to " + tchoice + "both including, on repo: " + s.sds);
                                Console.WriteLine("Do you want to continue? (y) / (n)");
                                s.s = Console.ReadLine();
                                switch (s.s)
                                {
                                    case "y":
                                        AttemptRepository.RemoveTests(s.ds, choice, tchoice); bob = false;
                                        break;
                                    case "n":
                                        bob = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                    }
                }
            }
        }

        public void makeHitbox()
        {
            Console.WriteLine("fetching data for hitboxes");
            string fileName = $"{ds}data";
            var attempts = AttemptRepository.GetAttempts(ds);
            if (attempts == null)
            {
                return;
            }
            var pinch = attempts.Where(x => x.Type == GestureType.Pinch).ToList();
            var tilt = attempts.Where(x => x.Type == GestureType.Tilt).ToList();
            var _throw = attempts.Where(x => x.Type == GestureType.Throw).ToList();
            var swipe = attempts.Where(x => x.Type == GestureType.Swipe).ToList();

            Console.WriteLine("Making hitboxes");
            Console.WriteLine("pinch");
            DataVisualizer.DrawHitBox(pinch, fileName + "/pinch.png");
            Console.WriteLine("tilt");
            DataVisualizer.DrawHitBox(tilt, fileName + "/tilt.png");
            Console.WriteLine("throw");
            DataVisualizer.DrawHitBox(_throw, fileName + "/throw.png");
            Console.WriteLine("swipe");
            DataVisualizer.DrawHitBox(swipe, fileName + "/swipe.png");
            Console.WriteLine("Making heatmaps");
            Console.WriteLine("pinch");
            DataVisualizer.DrawHeatMap(pinch, GridSize.Large, fileName + "/heatmap/pinch.png");
            Console.WriteLine("tilt");
            DataVisualizer.DrawHeatMap(tilt, GridSize.Large, fileName + "/heatmap/tilt.png");
            Console.WriteLine("throw");
            DataVisualizer.DrawHeatMap(_throw, GridSize.Large, fileName + "/heatmap/throw.png");
            Console.WriteLine("swipe");
            DataVisualizer.DrawHeatMap(swipe, GridSize.Large, fileName + "/heatmap/swipe.png");
            Console.WriteLine("all");
            DataVisualizer.DrawHeatMap(attempts, GridSize.Large, fileName + "/heatmap/all.png");
            Console.WriteLine("Done");
            Console.WriteLine("");
        }

        private string ss;
        public string sds;
        public DataSource ds;
        public string s
        {
            get { return this.ss; }
            set
            {
                if (value == "q" || value =="Q")
                {
                    Environment.Exit(0);
                }
                else
                {
                    ss = value;
                }
            }
        }
    }

    class Program {



        static void Main(string[] args)
        {
            inputstream s = new inputstream();

            bool bds = false;
            int choice = 0;
            
            Console.WriteLine("To quit at anytime, press q");
            //Console.WriteLine("Please enter the data source you want to use: Target or Field ");
            //sds = Console.ReadLine();
            while (bds == false)
            {
                switch (s.s)
                {
                    case "Field":
                        s.ds = DataSource.Field; bds = true;
                        break;
                    case "Target":
                        s.ds = DataSource.Target; bds = true;
                        break;
                    
                    default:
                        Console.WriteLine("Please enter the data source you want to use: Target or Field ");
                        s.s = Console.ReadLine();
                        break;
                }
            }
            s.sds = s.s;
            //Menu

            while (true)
            {
                Console.WriteLine("You are on datasource " + s.sds + " - what do you want to do with it?");
                Console.WriteLine("1) Add tests");
                Console.WriteLine("2) Remove tests");
                Console.WriteLine("3) Make Hitboxes");
                Console.WriteLine("Q) quit");

                s.s = Console.ReadLine();

                switch (s.s)
                {
                    case "1": s.addTests(s);
                        break;
                    case "2": s.remTests(s);
                        break;
                    case "3": s.makeHitbox();
                        break;
                    case "4":
                        break;
                    default:
                        break;
                }

            }



            //AttemptRepository.RemoveTests(ds);

            //List<Test> tests = new List<Test>();

            //for (int i = 1; i < 25; i++)
            //{
            //    tests.Add(new Test(i,ds));
            //}
            //AttemptRepository.SaveTestsToDatabase(tests);
        }
    }
}
 