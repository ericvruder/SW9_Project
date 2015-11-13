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
            Logger.CurrentLogger.NewUser();
            sequences = new List<int>();
            for(int i = 0; i < 8; i++) {
                sequences.Add(i);
            }
        }

        public void StartTest(GestureDirection direction) {
            GestureParser.SetDirectionContext(direction);
            targetSequence = GetNextSequence();
            gestureTypeList = GetRandomGestureList();
            board.CreateTarget(targetSequence.Dequeue());
            ChangeGesture();

        }

        Queue<Target> targetSequence = new Queue<Target>();

        public void TargetHit(bool hit, bool correctShape, Cell target, Point pointer) {
            Logger.CurrentLogger.CurrentTargetHit(hit, target, pointer, correctShape);
            if(targetSequence.Count != 0) {
                board.CreateTarget(targetSequence.Dequeue());
            }
            else if(gestureTypeList.Count != 0) {
                ChangeGesture();
                targetSequence = GetNextSequence();
            }
        }

        private Queue<Target> GetNextSequence() {
            int x = new Random().Next(sequences.Count);
            Queue<Target> targets = Target.GetTargetSequence(x);
            sequences.RemoveAt(x);
            return targets;
        }

        private void ChangeGesture() {
            GestureParser.SetTypeContext(gestureTypeList.Dequeue());
            VideoWindow window = new VideoWindow(GestureParser.GetDirectionContext(), GestureParser.GetTypeContext());
            Logger.CurrentLogger.StartNewgestureTest(GestureParser.GetTypeContext(), GestureParser.GetDirectionContext());
        }

        Queue<GestureType> gestureTypeList;

        private Queue<GestureType> GetRandomGestureList() {

            Array values = Enum.GetValues(typeof(GestureType));
            Random random = new Random();
            Queue<GestureType> list = new Queue<GestureType>();
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
