using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SW9_Project.Logging;

namespace SW9_Project {
    class TestSuite {

        private static int sgHeight, sgWidth, lgHeight, lgWidth;
        private static double canvasHeight, canvasWidth;

        public static void Intialize(int sHeight, int sWidth, int lHeight, int lWidth, double cnvasHeight, double cnvasWidth) {
            sgHeight = sHeight;
            sgWidth = sWidth;
            lgHeight = lHeight;
            lgWidth = lWidth;
            canvasHeight = cnvasHeight;
            canvasWidth = cnvasWidth;
        }

        IDrawingBoard board;
        GridSize currentSize;
        bool runningTest = true;

        public TestSuite(GestureDirection direction, IDrawingBoard board) {
            this.board = board;
            Random r = new Random();
            currentSize = r.Next(2) == 0 ? GridSize.Small : GridSize.Large;
            Logger.CurrentLogger.NewUser();
            ChangeSize();
            GestureParser.SetDirectionContext(direction);
            board.CreateTarget(GestureParser.GetDirectionContext());
            ChangeGesture();
        }

        private int currentHits = 0;
        private bool doneFirstDirection = false, doneFirstSize = false;
        public void TargetHit() {
            if (runningTest) {
                Logger.CurrentLogger.CurrentTargetHit();
                if (++currentHits >= 5) {
                    currentHits = 0;
                    Logger.CurrentLogger.EndCurrentGestureTest();
                    if (gestureTypeList.Count != 0) {
                        ChangeGesture();
                    } else if (!doneFirstSize) {
                        doneFirstSize = true;
                        ChangeSize();
                        ChangeGesture();
                    } else if (!doneFirstDirection) {
                        doneFirstSize = false;
                        doneFirstDirection = true;
                        GestureDirection direction = GestureParser.GetDirectionContext() == GestureDirection.Pull ? GestureDirection.Push : GestureDirection.Pull;
                        GestureParser.SetDirectionContext(direction);
                        ChangeSize();
                        ChangeGesture();
                    } else {
                        runningTest = false;
                        Logger.CurrentLogger.EndUser();
                        ThankYou ty = new ThankYou();
                        return;
                    }

                }
                Cell target = board.CreateTarget(GestureParser.GetDirectionContext());
                Logger.CurrentLogger.AddNewTarget("circle", target.X, target.Y);
            }
        }


        private void ChangeSize() {
            currentSize = currentSize == GridSize.Large ? GridSize.Small : GridSize.Large;
            board.CreateGrid(currentSize);
            gestureTypeList = GetRandomGestureList();
            if (currentSize == GridSize.Large) {
                Logger.CurrentLogger.ChangeSize(lgHeight, lgWidth, canvasHeight / lgHeight, canvasWidth / lgWidth);
            }
            else {
                Logger.CurrentLogger.ChangeSize(sgHeight, sgWidth, canvasHeight / sgHeight, canvasWidth / sgWidth);
            }

        }
        private void ChangeGesture() {
            GestureParser.SetTypeContext(gestureTypeList[0]);
            VideoWindow window = new VideoWindow(GestureParser.GetDirectionContext(), GestureParser.GetTypeContext());
            Logger.CurrentLogger.StartNewgestureTest(GestureParser.GetTypeContext(), GestureParser.GetDirectionContext());
            gestureTypeList.RemoveAt(0);
        }

        List<GestureType> gestureTypeList;

        private List<GestureType> GetRandomGestureList() {

            Array values = Enum.GetValues(typeof(GestureType));
            Random random = new Random();
            List<GestureType> list = new List<GestureType>();
            while (true) {
                GestureType randomType = (GestureType)values.GetValue(random.Next(values.Length));
                if(!list.Contains(randomType)) {
                    list.Add(randomType);
                }
                else if(list.Count == 4) {
                    break;
                }
            }
            return list;
        }
    }
}
