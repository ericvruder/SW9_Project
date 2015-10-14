package com.nui.android;

/**
 * Created by ericv on 10/14/2015.
 */
public class RotationData extends MobileData {

    public float Pitch;
    public float Roll;

    public RotationData(float pitch, float roll, long time){
        Time = time;
        Pitch = pitch;
        Roll = roll;
    }
}
