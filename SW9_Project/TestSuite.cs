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
        public bool Done { get { return done; } }

        bool firstDirectionRun = false, done = false;

        public void StartTest(GestureDirection direction) {
            GestureParser.SetDirectionContext(direction);
            gestureTypeList = GetRandomGestureList();
            ChangeGesture();
        }

        Queue<Target> targetSequence = new Queue<Target>();
        Queue<Target> practiceSequence = new Queue<Target>();
        bool practiceDone = false;

        public void TargetHit(bool hit, bool correctShape, Cell target, Point pointer, Cell pointerCell) {
            if(practiceDone) {
                Logger.CurrentLogger.CurrentTargetHit(hit, target, pointer, pointerCell, correctShape);
            }
            if (practiceSequence.Count != 0) {
                board.CreateTarget(practiceSequence.Dequeue());
                return;
            }
            else if (!practiceDone){
                practiceDone = true;
                board.PracticeDone();
            }

            if(targetSequence.Count != 0) {
                board.CreateTarget(targetSequence.Dequeue());
            }
            else {
                if(gestureTypeList.Count == 0) {
                    if(!firstDirectionRun) { firstDirectionRun = true; }
                    else { done = true; }
                }
                board.CurrentGestureDone();
            }
        }
        
        VideoWindow techniquePlayer;
        public bool ChangeGesture() {
            if (gestureTypeList.Count == 0) { return false; }
            practiceDone = false;
            targetSequence = Target.GetNextSequence();
            practiceSequence = Target.GetPracticeTargets();

            board.Clear();
            board.StartNewGesture();
            board.CreateTarget(practiceSequence.Dequeue());
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
            return new Queue<GestureType>(types);
        }
    }
}
