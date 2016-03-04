package com.nui.android;

/**
 * Created by ericv on 10/13/2015.
 */
public class MobileGesture {

    public MobileGesture(String shape, String type, String direction){
        Type = type;
        Shape = shape;
        Direction = direction;
    }

    public MobileGesture(String shape, String type){
        this(shape, type, "");
    }

    public String Type;
    public String Shape;
    public String Direction;

}
