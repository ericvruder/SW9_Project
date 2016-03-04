package com.nui.android;

/**
 * Created by ericv on 11/2/2015.
 */
public class TiltGesture extends MobileGesture {
    public TiltGesture(String shape){
        this.Shape = shape;
        this.Type = "TiltGesture";
    }

    public TiltGesture(String shape, String direction){
        Shape = shape;
        Direction = direction;
        Type = "TiltGesture";
    }
}
