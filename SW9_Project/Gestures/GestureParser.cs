using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW9_Project.Logging;
using System.Windows.Shapes;

namespace SW9_Project {
    static class GestureParser {

        static private KinectGesture waitingKinectGesture;
        static private MobileGesture waitingMobileGesture;
        static private KinectGesture awaitingGesture;


        private static Connection connection;

        static private GestureDirection directionContext = GestureDirection.Push;
        static private GestureType typeContext = GestureType.Swipe;

        static private IDrawingBoard board;

        static public void Initialize(IDrawingBoard _board) {
            board = _board;
        }
        static bool paused = false;
        static public void Pause(bool pause) {
            paused = pause;
        }

        static public KinectGesture AwaitingGesture {
            get {
                KinectGesture t = awaitingGesture;
                awaitingGesture = null;
                if(t != null) {
                    Console.WriteLine($"{DateTime.Now}: Activating gesture {t.Type} {t.Direction}");
                }
                return t;
            }
            set { awaitingGesture = value; }
        }

        public static void SetDirectionContext(GestureDirection direction) {
            connection?.StartTest(direction);
            directionContext = direction;
        }

        public static GestureDirection GetDirectionContext() {
            return directionContext;
        }

        public static GestureType GetTypeContext() {
            return typeContext;
        }

        public static void SetTypeContext(GestureType type) {
            connection?.SetGesture(type);
            typeContext = type;
        }
        
        static public void AddMobileGesture(MobileGesture receivedGesture) {
            if (paused) return;
            Logger.CurrentLogger.AddNewMobileGesture(receivedGesture);
            if (receivedGesture.Type == typeContext) {
                switch (receivedGesture.Type) {
                    case GestureType.Swipe: case GestureType.Tilt {
                            ClearGestures();
                            AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                        }
                        break;
                    case GestureType.Pinch:
                        break;
                    case GestureType.Throw:
                        if (waitingKinectGesture?.Type == GestureType.Throw) {
                            ClearGestures();
                            AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                        } else {
                            ClearGestures();
                            waitingMobileGesture = receivedGesture;
                        }
                        break;
                }
            }
        }

        static private void ClearGestures() {
            waitingKinectGesture = null;
            waitingMobileGesture = null;
            AwaitingGesture = null;
        }


        static public void SetConnection(Connection conn) {
            connection = conn;
        }


        static public void AddKinectGesture(KinectGesture receivedGesture) {
            if (paused) return;
            Logger.CurrentLogger.AddNewKinectGesture(receivedGesture, board.GetCell(receivedGesture.Pointer));
            if (typeContext == receivedGesture.Type) {
                switch (receivedGesture.Type) {
                    case GestureType.Pinch:
                        {
                            if (directionContext == GestureDirection.Pull && receivedGesture.Direction == GestureDirection.Pull) {
                                board.LockPointer(); 
                                string shape = "";
                                shape = board.GetCell(receivedGesture.Pointer)?.Shape is Ellipse ? "circle" : "square";
                                KinectGesture gesture = new KinectGesture(shape);
                                connection.SendPinch();
                                ClearGestures();
                                waitingKinectGesture = gesture;
                            } else if (waitingMobileGesture?.Type == GestureType.Pinch) {
                                if (receivedGesture.Direction != directionContext) { break; }
                                KinectGesture gesture = new KinectGesture(waitingMobileGesture.Shape);
                                ClearGestures();
                                AwaitingGesture = gesture;
                            } else {
                                ClearGestures();
                            }
                        }
                        break;
                    case GestureType.Throw:
                        {
                            if (directionContext != receivedGesture.Direction) {
                                ClearGestures();
                            } else if (waitingMobileGesture == null) {
                                ClearGestures();
                                waitingKinectGesture = receivedGesture;
                            } else if (waitingMobileGesture?.Type == GestureType.Throw) {
                                KinectGesture gesture = new KinectGesture(waitingMobileGesture.Shape);
                                ClearGestures();
                                AwaitingGesture = gesture;
                            } else {
                                ClearGestures();

                            }
                        }
                        break;
                }
            }
        }
    }
}
