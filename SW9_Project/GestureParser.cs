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

        static private GestureDirection directionContext = GestureDirection.Push;
        static private GestureType typeContext = GestureType.Swipe;

        static private IDrawingBoard board;

        static public void Initialize(IDrawingBoard _board) {
            board = _board;
        }

        static public KinectGesture AwaitingGesture {
            get {
                KinectGesture t = awaitingGesture;
                awaitingGesture = null;
                return t;
            }
            set { awaitingGesture = value; }
        }

        public static void SetDirectionContext(GestureDirection direction) {
            directionContext = direction;
        }

        public static GestureDirection GetDirectionContext() {
            return directionContext;
        }

        public static GestureType GetTypeContext() {
            return typeContext;
        }

        public static void SetTypeContext(GestureType type) {
            typeContext = type;
        }
        
        static public void AddMobileGesture(MobileGesture receivedGesture) {
            Logger.CurrentLogger.AddNewMobileGesture(receivedGesture);
            switch (receivedGesture.Type) {
                case GestureType.Swipe:
                    {
                        if (typeContext == GestureType.Swipe) {
                            ClearGestures();
                            AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                        } else {
                            ClearGestures();
                        }
                    }
                    break;
                case GestureType.Pinch:
                    {
                        if (typeContext == GestureType.Pinch) {
                            if (waitingKinectGesture?.Type == GestureType.Pinch) {
                                ClearGestures();
                                AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                            } else {
                                ClearGestures();
                                waitingMobileGesture = receivedGesture;
                            }
                        } else {
                            ClearGestures();
                        }
                    }
                    break;
                case GestureType.Throw:
                    {
                        if(typeContext == GestureType.Pinch) { break; }
                        if (typeContext == GestureType.Tilt) {
                            ClearGestures();
                            AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                        } else if (typeContext == GestureType.Throw) {
                            if (waitingKinectGesture?.Type == GestureType.Throw) {
                                ClearGestures();
                                AwaitingGesture = new KinectGesture(receivedGesture.Shape);
                            } else {
                                ClearGestures();
                                waitingMobileGesture = receivedGesture;
                            }
                        } else {
                            ClearGestures();
                        }
                    }
                    break;
            }
        }

        static private void ClearGestures() {
            waitingKinectGesture = null;
            waitingMobileGesture = null;
            AwaitingGesture = null;
        }

        static public void AddKinectGesture(KinectGesture receivedGesture) {
            Logger.CurrentLogger.AddNewKinectGesture(receivedGesture, board.GetCell(receivedGesture.Pointer));
            if (typeContext == receivedGesture.Type) {
                if (waitingMobileGesture != null) {
                    lock (waitingMobileGesture) {
                        switch (receivedGesture.Type) {
                            case GestureType.Pinch:
                                {
                                    if (directionContext == GestureDirection.Pull && receivedGesture.Direction == GestureDirection.Pull) {
                                        ClearGestures();
                                        string shape = "";
                                        shape = board.GetCell(receivedGesture.Pointer)?.Shape is Ellipse ? "circle" : "square";
                                        AwaitingGesture = new KinectGesture(shape);
                                    } else if (waitingMobileGesture?.Type == GestureType.Pinch) {
                                        if (receivedGesture.Direction != directionContext) { break; }
                                        ClearGestures();
                                        AwaitingGesture = new KinectGesture(waitingMobileGesture?.Shape);
                                    } else {
                                        ClearGestures();
                                    }
                                }
                                break;
                            case GestureType.Throw:
                                {
                                    if (directionContext != receivedGesture.Direction) {
                                        ClearGestures();
                                    } else if (waitingMobileGesture?.Type == GestureType.Throw) {
                                        ClearGestures();
                                        AwaitingGesture = new KinectGesture(waitingMobileGesture?.Shape);
                                    } else if (waitingMobileGesture == null) {
                                        ClearGestures();
                                        waitingKinectGesture = receivedGesture;
                                    } else {
                                        ClearGestures();

                                    }
                                }
                                break;
                        }
                    }
                }
            } else {
                ClearGestures();
            }
        }
    }
}
