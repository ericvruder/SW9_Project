package com.nui.android;

/**
 * Created by ericv on 11/2/2015.
 */
public class SwipeGesture extends MobileGesture {
    public SwipeGesture(String direction, String shape){
        Shape = shape;
        Direction = direction;
        Type = "SwipeGesture";
    }
}
