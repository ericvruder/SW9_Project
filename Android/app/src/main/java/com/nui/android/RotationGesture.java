package com.nui.android;

/**
 * Created by ericv on 10/14/2015.
 */
public class RotationGesture extends MobileGesture {

    public float Pitch;
    public float Roll;

    public RotationGesture(float pitch, float roll, long time){
        Time = time;
        Pitch = pitch;
        Roll = roll;
    }
}
