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

        public TestSuite(IDrawingBoard board) {
            this.board = board;
            UserID = Logger.CurrentLogger.NewUser();
        }

        public int UserID { get; }

        public void StartTest(GestureDirection direction) {
            GestureParser.SetDirectionContext(direction);
            gestureTypeList = GetRandomGestureList();
            ChangeGesture();
        }

        Queue<Target> targetSequence = new Queue<Target>();

        public void TargetHit(bool hit, bool correctShape, Cell target, Point pointer, Cell pointerCell) {
            Logger.CurrentLogger.CurrentTargetHit(hit, target, pointer, pointerCell, correctShape);
            if(targetSequence.Count != 0) {
                board.CreateTarget(targetSequence.Dequeue());
                board.SetProgress(totalTargets - targetSequence.Count, totalTargets);
            }
            else
            {
                board.CurrentGestureDone();
            }
        }

        int totalTargets = 0;
        VideoWindow techniquePlayer;
        public bool ChangeGesture() {
            if (gestureTypeList.Count == 0) { return false; }
            targetSequence = Target.GetNextSequence();
            totalTargets = targetSequence.Count;
            board.Clear();
            board.StartNewGesture();
            board.CreateTarget(targetSequence.Dequeue());
            board.SetProgress(totalTargets - targetSequence.Count, totalTargets);
            GestureParser.SetTypeContext(gestureTypeList.Dequeue());
            if(techniquePlayer != null) {
                techniquePlayer.Close();
            }
            techniquePlayer = new VideoWindow(GestureParser.GetDirectionContext(), GestureParser.GetTypeContext());
            Logger.CurrentLogger.StartNewgestureTest(GestureParser.GetTypeContext(), GestureParser.GetDirectionContext());
            return true;
        }

        Queue<GestureType> gestureTypeList;

        private Queue<GestureType> GetRandomGestureList() {
            
            List<GestureType> types = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };
            types.Shuffle();
            Queue<GestureType> list = new Queue<GestureType>(types);
            return list;
        }
    }
}
