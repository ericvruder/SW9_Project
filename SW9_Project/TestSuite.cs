using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class TestSuite {

        List<GestureType> gestureTypes;
        GestureType currentType;
        GestureDirection currentDirection, firstDirection;
        int numberOfHitsPerGesture = 5;
        int currentHits = 0;
        bool nextGesture = false, nextDirection = false, nextSize = false, done = false, bothSizes = false;

        public bool NextGesture {
            get {
                bool t = nextGesture;
                nextGesture = false;
                return t;
            }
        }
        public bool NextDirection {
            get {
                bool t = nextDirection;
                nextDirection = false;
                return t;
            }
        }
        public bool NextSize {
            get {
                bool t = nextSize;
                nextSize = false;
                return t;
            }
        }
        public bool Done { get { return done; } }
        public GestureType CurrrentType { get { return currentType;} }

        public TestSuite(GestureDirection firstDirection) {
            this.firstDirection = firstDirection;
            this.currentDirection = firstDirection;
            gestureTypes = GetRandomGestureList();
            ChangeGesture();
        }

        public static void StartNewTest(GestureDirection direction) {
            CurrentTest = new TestSuite(direction);
        }

        public static TestSuite CurrentTest;

        public void TargetHit() {
            if(++currentHits == numberOfHitsPerGesture) {
                currentHits = 0;
                if(gestureTypes.Count != 0) {
                    ChangeGesture();
                }
                else if(currentDirection == firstDirection) {
                    gestureTypes = GetRandomGestureList();
                    if(firstDirection == GestureDirection.Pull) {
                        currentDirection = GestureDirection.Push;
                    }
                    else {
                        currentDirection = GestureDirection.Pull;
                    }
                    nextDirection = true;
                    ChangeGesture();
                }
                else if (!bothSizes) {
                    gestureTypes = GetRandomGestureList();
                    currentDirection = firstDirection;
                    ChangeGesture();
                    bothSizes = true;
                    nextSize = true;
                    nextDirection = true;
                }
                else {
                    done = true;
                }
            }
        }

        private void ChangeGesture() {
            currentType = gestureTypes[0];
            GestureParser.SetTypeContext(gestureTypes[0]);
            gestureTypes.Remove(0);
            nextGesture = true;
        }

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
