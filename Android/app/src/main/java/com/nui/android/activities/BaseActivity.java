package com.nui.android.activities;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GestureDetectorCompat;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

import com.nui.android.AccelerometerMonitor;
import com.nui.android.Network;
import com.nui.android.PinchGestureListener;
import com.nui.android.R;
import com.nui.android.RotationMonitor;
import com.nui.android.Shape;
import com.nui.android.SwipeGestureListener;
import com.nui.android.TouchGestureListener;
import com.nui.android.filter.GyroscopeOrientation;
import com.nui.android.filter.ImuOCfOrientation;
import com.nui.android.filter.ImuOCfQuaternion;
import com.nui.android.filter.ImuOCfRotationMatrix;
import com.nui.android.filter.ImuOKfQuaternion;
import com.nui.android.filter.Orientation;
import com.nui.android.gauge.GaugeBearing;
import com.nui.android.gauge.GaugeRotation;

import java.net.DatagramPacket;
import java.util.Random;

/**
 * Base activity
 */
public class BaseActivity extends Activity {

    //private Network network;
    GestureDetectorCompat swipeDetector;
    ScaleGestureDetector pinchDetector;
    GestureDetectorCompat touchDetector;
    private AccelerometerMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;
    private Orientation orientation;

    public static String shape;
    public static String nextShape;

    private ImageView circleView;
    private ImageView squareView;

    private ImageView pullShape;
    private Button moveCursor;
    private boolean sendGyroData = false;

    private final Random random = new Random();
    private int count;
    private static int MAX_COUNT = 2;
    private boolean imuOCfOrienationEnabled;
    private boolean imuOCfRotationMatrixEnabled;
    private boolean imuOCfQuaternionEnabled;
    private boolean imuOKfQuaternionEnabled;
    private boolean isCalibrated;
    public boolean virtualCalibrated = false;
    // Handler for the UI plots so everything plots smoothly
    protected Handler handler;
    protected Runnable runable;

    private byte[] msg = new byte[100];
    public DatagramPacket dp = new DatagramPacket(msg, msg.length);
    public Thread nt;
    private boolean end_nt;

    private float calibrateZ = 0;
    private float calibrateX = 0;
    private float calibrateY = 0;

    private float virtualX = 0;
    private float virtualY = 0;
    private float virtualZ = 0;

    private float[] vOrientation = new float[3];
    private boolean dataReady = false;

    // The gauge views. Note that these are views and UI hogs since they run in
    // the UI thread, not ideal, but easy to use.
    private GaugeBearing gaugeBearingCalibrated;
    private GaugeRotation gaugeTiltCalibrated;
    private boolean menuActive = true;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_base);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);

        getWindow().getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_IMMERSIVE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);


        initNetwork();
        SwipeGestureListener swipeGestureListener = new SwipeGestureListener(Network.getInstance());
        rotationSensor = new RotationMonitor(Network.getInstance(), this);
        acceloremeterSensor = new AccelerometerMonitor(Network.getInstance(), rotationSensor, this);
        swipeDetector = new GestureDetectorCompat(this, swipeGestureListener);
        pinchDetector = new ScaleGestureDetector(this, new PinchGestureListener(Network.getInstance(), swipeGestureListener));
        touchDetector = new GestureDetectorCompat(this, new TouchGestureListener(this, Network.getInstance()));

        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);
        pullShape = (ImageView) findViewById(R.id.pull_shape);
        pullShape.setVisibility(View.INVISIBLE);
        circleView.setVisibility(View.INVISIBLE);
        squareView.setVisibility(View.INVISIBLE);
        count = 0;
    }

    private boolean pushOrPull;

    public void networkThread(){
        handler = new Handler();

        runable = new Runnable()
        {
            private long time = 0;

            @Override
            public void run() {
                if (time == 0)
                    time = orientation.sensorTimestamp;

                handler.postDelayed(this, 100);

                vOrientation = orientation.getOrientation();

                float x = vOrientation[0];
                float y = vOrientation[1];
                float z = vOrientation[2];

                if (!virtualCalibrated) {
                    calibrateZ = z;
                    calibrateX = x;
                    calibrateY = y;
                    virtualCalibrated = true;
                }

                virtualX = x - calibrateX;
                virtualY = y - calibrateY;
                virtualZ = z - calibrateZ;

                Log.d("Orientation", Float.toString(virtualX) + " " + Float.toString(virtualY) + " " + Float.toString(virtualZ));
                byte[] buf = ("gyrodata:time:" + orientation.sensorTimestamp + ":x:" + virtualX + ":y:" + virtualY + ":z:" + virtualZ).getBytes();
                dp.setData(buf);
                time = orientation.sensorTimestamp;
                dataReady = true;

                //updateGauges();
            }
        };

        // network thread
        nt = new Thread(new Runnable() {
            @Override
            public void run() {
                long lt = 0;

                while (true) {
                    if (end_nt) {
                        Log.d("BaseActivity", "Network thread ends.");
                        break;
                    }
                    //
                    if (sendGyroData && orientation.sensorTimestamp > lt) {
                        try {
                            Network.getInstance().ds.send(dp);
                            lt = orientation.sensorTimestamp;
                        } catch (Exception e) {
                            e.printStackTrace();
                        }
                    }
                }
            }
        }, "UdpThread");

        nt.setPriority(Thread.MIN_PRIORITY);
        if(Network.getInstance().ds != null) {
            nt.start();
        }else {
            Network.getInstance().Reconnect();
        }
    }

    public boolean PushOrPull(){
        //True = push, false = pull
        return pushOrPull;
    }

    protected void initNetwork()
    {
        Network.initInstance(this);
    }

    public void StartPullTest(){
        pushOrPull = false;
        circleView.setVisibility(View.INVISIBLE);
        squareView.setVisibility(View.INVISIBLE);
        pullShape.setVisibility(View.VISIBLE);

        pullShape.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                return true;
            }

        });
    }

    public void SetGesture(String gesture){
        sendGyroData = false;
        switch (gesture){
            case "tilt": case "throw": acceloremeterSensor.SetTiltorThrow(gesture); break;
            case "swipe": sendGyroData = true ; break;
            default: break;
        }
    }

    public void StartPushTest(){

        pushOrPull = true;
        pullShape.setVisibility(View.INVISIBLE);
        circleView.setVisibility(View.VISIBLE);
        squareView.setVisibility(View.VISIBLE);

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if(event.getAction() == MotionEvent.ACTION_DOWN || event.getAction() == MotionEvent.ACTION_MOVE) {
                    circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle_stroke));
                    squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
                }

                return true;
            }

        });

        squareView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Square;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if(event.getAction() == MotionEvent.ACTION_DOWN || event.getAction() == MotionEvent.ACTION_MOVE) {
                    squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square_stroke));
                    circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
                }

                return true;
            }

        });
    }

    boolean pullPinchWaiting = false;
    public void AwaitingPullPinch(boolean waiting){
        pullPinchWaiting = waiting;
    }

    public boolean IsWaitingForPinch(){
        return pullPinchWaiting;
    }

    public void ClearShapes(){
        shape = null;
        squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
        circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
    }

    public void SwitchPosition() {
        ClearShapes();
        if(count > MAX_COUNT || random.nextBoolean()) {
            count = 0;
            int TopShapeTop = circleView.getTop();
            int TopShapeBottom = circleView.getBottom();
            int BottomShapeTop = squareView.getTop();
            int BottomShapeBottom = squareView.getBottom();

            circleView.setTop(BottomShapeTop);
            circleView.setBottom(BottomShapeBottom);
            squareView.setTop(TopShapeTop);
            squareView.setBottom(TopShapeBottom);
        } else {
            count++;
        }
    }

    public void SetShape(String shape) {
        ClearShapes();
        shape = shape;

        if(shape.equals("circle")) {
            pullShape.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
        }
        else {
            pullShape.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
        }
    }

    public boolean ReadyToStart(){
        return true;
    }

    public static String GetSelectedShape(){
        return shape;
    }

    public void CloseApp(){
        this.finish();
        Intent intent = new Intent(Intent.ACTION_MAIN);
        intent.addCategory(Intent.CATEGORY_HOME);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        startActivity(intent);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {

        menu.add(Menu.NONE, R.id.network_discovery, Menu.NONE, R.string.network_discovery);
        menu.add(Menu.NONE, R.id.reconnect_action, Menu.NONE, R.string.reconnect_action);
        menu.add(Menu.NONE, R.id.reset_orientation, Menu.NONE, R.string.reset_orientation);
        menu.add(Menu.NONE, R.id.gyro_config, Menu.NONE, R.string.gyro_config);
        menu.add(Menu.NONE, R.id.close_app_action, Menu.NONE, R.string.close_app_action);

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.network_discovery:
                Network.getInstance().FindServer(true);
                return true;
            case R.id.reconnect_action:
                Network.getInstance().Reconnect();
                return true;
            case R.id.close_app_action:
                //android.os.Process.killProcess(android.os.Process.myPid());
                CloseApp();
                return true;
            case R.id.reset_orientation:
                orientation.reset();
                virtualCalibrated = false;
                return true;
            case R.id.gyro_config:
                Intent intent = new Intent();
                intent.setClass(this, ConfigActivity.class);
                startActivity(intent);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    @Override
    protected void onPause(){
        super.onPause();
        acceloremeterSensor.Pause();
        orientation.onPause();
        handler.removeCallbacks(runable);
        Network.getInstance().Pause();
    }

    @Override
    protected void onResume(){
        super.onResume();
        Network.getInstance().Resume();
        readPrefs();
        reset();
        acceloremeterSensor.Resume();
        orientation.onResume();
        networkThread();
        handler.post(runable);
    }

    private void readPrefs()
    {
        imuOCfOrienationEnabled = getPrefImuOCfOrientationEnabled();
        imuOCfRotationMatrixEnabled = getPrefImuOCfRotationMatrixEnabled();
        imuOCfQuaternionEnabled = getPrefImuOCfQuaternionEnabled();
        imuOKfQuaternionEnabled = getPrefImuOKfQuaternionEnabled();
    }

    private boolean getPrefCalibratedGyroscopeEnabled()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return prefs.getBoolean(
                ConfigActivity.CALIBRATED_GYROSCOPE_ENABLED_KEY, true);
    }

    private boolean getPrefImuOCfOrientationEnabled()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return prefs.getBoolean(ConfigActivity.IMUOCF_ORIENTATION_ENABLED_KEY, false);
    }

    private boolean getPrefImuOCfRotationMatrixEnabled()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return prefs.getBoolean(
                ConfigActivity.IMUOCF_ROTATION_MATRIX_ENABLED_KEY, false);
    }

    private boolean getPrefImuOCfQuaternionEnabled()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return prefs.getBoolean(ConfigActivity.IMUOCF_QUATERNION_ENABLED_KEY, false);
    }

    private boolean getPrefImuOKfQuaternionEnabled()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return prefs.getBoolean(ConfigActivity.IMUOKF_QUATERNION_ENABLED_KEY, false);
    }

    private float getPrefImuOCfOrienationCoeff()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return Float.valueOf(prefs.getString(
                ConfigActivity.IMUOCF_ORIENTATION_COEFF_KEY, "0.5"));
    }

    private float getPrefImuOCfRotationMatrixCoeff()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return Float.valueOf(prefs.getString(
                ConfigActivity.IMUOCF_ROTATION_MATRIX_COEFF_KEY, "0.5"));
    }

    private float getPrefImuOCfQuaternionCoeff()
    {
        SharedPreferences prefs = PreferenceManager
                .getDefaultSharedPreferences(getApplicationContext());

        return Float.valueOf(prefs.getString(
                ConfigActivity.IMUOCF_QUATERNION_COEFF_KEY, "0.5"));
    }

    private boolean gyroscopeAvailable()
    {
        return getPackageManager().hasSystemFeature(PackageManager.FEATURE_SENSOR_GYROSCOPE);
    }

    private void reset() {
        isCalibrated = getPrefCalibratedGyroscopeEnabled();
        orientation = new GyroscopeOrientation(this);
        if (isCalibrated)
        {
            Toast.makeText(this, "Sensor Calibrated", Toast.LENGTH_SHORT);
        }
        else
        {
            Toast.makeText(this, "Sensor Uncalibrated", Toast.LENGTH_SHORT);
        }

        if (imuOCfOrienationEnabled)
        {
            orientation = new ImuOCfOrientation(this);
            orientation.setFilterCoefficient(getPrefImuOCfOrienationCoeff());

            if (isCalibrated)
            {
                Toast.makeText(this, "ImuOCfOrientation Calibrated", Toast.LENGTH_SHORT);
            }
            else
            {
                Toast.makeText(this, "ImuOCfOrientation Uncalibrated", Toast.LENGTH_SHORT);
            }

        }
        if (imuOCfRotationMatrixEnabled)
        {
            orientation = new ImuOCfRotationMatrix(this);
            orientation.setFilterCoefficient(getPrefImuOCfRotationMatrixCoeff());

            if (isCalibrated)
            {
                Toast.makeText(this, "ImuOCfRm Calibrated", Toast.LENGTH_SHORT);
            }
            else
            {
                Toast.makeText(this, "ImuOCfRm Uncalibrated", Toast.LENGTH_SHORT);
            }
        }
        if (imuOCfQuaternionEnabled)
        {
            orientation = new ImuOCfQuaternion(this);
            orientation.setFilterCoefficient(getPrefImuOCfQuaternionCoeff());

            if (isCalibrated)
            {
                Toast.makeText(this, "ImuOCfRm Uncalibrated", Toast.LENGTH_SHORT);
            }
            else
            {
                Toast.makeText(this, "ImuOCfQuaternion Uncalibrated", Toast.LENGTH_SHORT);
            }
        }
        if (imuOKfQuaternionEnabled)
        {
            orientation = new ImuOKfQuaternion(this);

            if (isCalibrated)
            {
                Toast.makeText(this, "ImuOKfQuaternion Calibrated", Toast.LENGTH_SHORT);
            }
            else
            {
                Toast.makeText(this, "ImuOKfQuaternion Uncalibrated", Toast.LENGTH_SHORT);
            }
        }


    }

    private void updateGauges()
    {
        gaugeBearingCalibrated.updateBearing(vOrientation[0]);
        gaugeTiltCalibrated.updateRotation(vOrientation);
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (nt.isAlive()) {
            end_nt = true;
        }
    }

    @Override
    protected void onStop(){
        super.onStop();
    }

    @Override
    public void onBackPressed() {
        if(menuActive)
            super.onBackPressed();
    }
    @Override
    public boolean onPrepareOptionsMenu (Menu menu) {
        return menuActive;
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        int action = event.getAction();
        int keyCode = event.getKeyCode();
        switch (keyCode) {
            case KeyEvent.KEYCODE_VOLUME_UP:
                if (action == KeyEvent.ACTION_DOWN) {
                    menuActive = !menuActive;
                }
                return true;
            default:
                return super.dispatchKeyEvent(event);
        }
    }

}
