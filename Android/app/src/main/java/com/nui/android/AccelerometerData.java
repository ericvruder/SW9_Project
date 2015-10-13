package com.nui.android;

import java.util.Date;

/**
 * Created by ericv on 10/13/2015.
 */
public class AccelerometerData extends MobileData{

    public float X;
    public float Y;
    public float Z;

    public long Time;

    public AccelerometerData(float x, float y, float z, long time){
        X = x; Y = y; Z = z;
        Time = time;
        Type = "AccelerometerData";
    }
}
