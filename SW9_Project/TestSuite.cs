using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SW9_Project.Logging;
using System.Windows;

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
        List<int> sequences;

        public TestSuite(IDrawingBoard board) {
            this.board = board;
            UserID = Logger.CurrentLogger.NewUser();
        }

        public int UserID { get; }

        public void StartTest(GestureDirection direction) {
            GestureParser.SetDirectionContext(direction);
            gestureTypeList = GetRandomGestureList();
            ChangeGesture();
            board.CreateTarget(targetSequence.Dequeue());

        }

        Queue<Target> targetSequence = new Queue<Target>();

        public void TargetHit(bool hit, bool correctShape, Cell target, Point pointer) {
            Logger.CurrentLogger.CurrentTargetHit(hit, target, pointer, correctShape);
            board.SetProgress(totalTargets - targetSequence.Count, totalTargets);
            if(targetSequence.Count != 0) {
                board.CreateTarget(targetSequence.Dequeue());
            }
            else if(gestureTypeList.Count != 0) {
                ChangeGesture();
            }
            else {
                ThankYou ty = new ThankYou();
            }
        }

        int totalTargets = 0;
        private void ChangeGesture() {
            targetSequence = Target.GetNextSequence();
            totalTargets = targetSequence.Count;
            board.SetProgress(0, totalTargets);
            GestureParser.SetTypeContext(gestureTypeList.Dequeue());
            VideoWindow window = new VideoWindow(GestureParser.GetDirectionContext(), GestureParser.GetTypeContext());
            Logger.CurrentLogger.StartNewgestureTest(GestureParser.GetTypeContext(), GestureParser.GetDirectionContext());
        }

        Queue<GestureType> gestureTypeList;

        private Queue<GestureType> GetRandomGestureList() {

            Queue<GestureType> list = new Queue<GestureType>();
            Array values = Enum.GetValues(typeof(GestureType));
            Random random = new Random();
            while (true) {
                GestureType randomType = (GestureType)values.GetValue(random.Next(values.Length));
                if(!list.Contains(randomType)) {
                    list.Enqueue(randomType);
                }
                else if(list.Count == 4) {
                    break;
                }
            }
            return list;
        }
    }
}
