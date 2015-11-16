package com.nui.android.activities;

import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;

import com.nui.android.Network;
import com.nui.android.R;
import com.nui.android.Shape;

import java.util.Random;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class PushTestActivity extends BaseActivity {

    private ImageView circleView;
    private ImageView squareView;

    private final Random random = new Random();
    private int count;
    private static int MAX_COUNT = 2;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_push_test;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);
        count = 0;
        Network.SetActivity(this);

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

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

                return true;
            }

        });

    }

    public void SwitchPosition() {
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

}


