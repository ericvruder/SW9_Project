package com.nui.android;

/**
 * Created by ericv on 10/13/2015.
 */
public class ThrowGesture extends MobileGesture {

    public ThrowGesture(String shape){
        Shape = shape;
        Type = "ThrowGesture";
    }

    public ThrowGesture(String shape, String direction){
        Shape = shape;
        Direction = direction;
        Type = "ThrowGesture";
    }
}
