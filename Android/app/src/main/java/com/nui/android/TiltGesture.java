package com.nui.android;

/**
 * Created by ericv on 11/2/2015.
 */
public class TiltGesture extends MobileGesture {
    public TiltGesture(String shape, String direction){
        this.Shape = shape;
        this.Direction = direction;
        this.Type = "TiltGesture";
    }
}
