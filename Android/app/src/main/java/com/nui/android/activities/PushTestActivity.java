package com.nui.android.activities;

import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;

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

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if(count > MAX_COUNT || random.nextBoolean()) {
                        count = 0;
                        SwitchPosition(circleView, squareView);
                    } else {
                        count++;
                    }
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

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if(count > MAX_COUNT || random.nextBoolean()) {
                        count = 0;
                        SwitchPosition(circleView, squareView);
                    } else {
                        count++;
                    }
                }
                return true;
            }

        });

    }

    public void SwitchPosition(ImageView imgView1, ImageView imgView2) {
        int TopShapeTop = imgView1.getTop();
        int TopShapeBottom = imgView1.getBottom();
        int BottomShapeTop = imgView2.getTop();
        int BottomShapeBottom = imgView2.getBottom();

        imgView1.setTop(BottomShapeTop);
        imgView1.setBottom(BottomShapeBottom);
        imgView2.setTop(TopShapeTop);
        imgView2.setBottom(TopShapeBottom);
    }

}


