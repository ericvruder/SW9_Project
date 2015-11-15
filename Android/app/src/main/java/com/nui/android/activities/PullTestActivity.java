package com.nui.android.activities;

import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;

import com.nui.android.R;

import java.util.Random;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class PullTestActivity extends BaseActivity {

    private ImageView circleView;
    private ImageView squareView;

    private final Random random = new Random();
    private int TopShapeTop;
    private int TopShapeBottom;
    private int BottomShapeTop;
    private int BottomShapeBottom;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_pull_test;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Drawable drawableCircle = ContextCompat.getDrawable(this, R.drawable.circle);
        Drawable drawableSquare = ContextCompat.getDrawable(this, R.drawable.square);
        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);

        circleView.setImageDrawable(drawableCircle);
        squareView.setImageDrawable(drawableSquare);

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "circle";

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if (random.nextBoolean()) {
                        TopShapeTop = circleView.getTop();
                        TopShapeBottom = circleView.getBottom();
                        BottomShapeTop = squareView.getTop();
                        BottomShapeBottom = squareView.getBottom();

                        circleView.setTop(BottomShapeTop);
                        circleView.setBottom(BottomShapeBottom);
                        squareView.setTop(TopShapeTop);
                        squareView.setBottom(TopShapeBottom);
                    }
                }

                return true;
            }

        });

        squareView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "square";

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if (random.nextBoolean()) {
                        TopShapeTop = circleView.getTop();
                        TopShapeBottom = circleView.getBottom();
                        BottomShapeTop = squareView.getTop();
                        BottomShapeBottom = squareView.getBottom();

                        circleView.setTop(BottomShapeTop);
                        circleView.setBottom(BottomShapeBottom);
                        squareView.setTop(TopShapeTop);
                        squareView.setBottom(TopShapeBottom);
                    }
                }

                return true;
            }

        });

    }

}


