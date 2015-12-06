$(document).ready(function(){

	var data = [
		{
		    label: "Tilt",
		    data: TiltData 
		}, 
		{
		    label: "Throw",
		    data: ThrowData 
		}, 
		{
		    label: "Swipe",
		    data: SwipeData 
		}, 
		{
		    label: "Pinch",
		    data: PinchData 
		}
	];

    var timeData = [
		{
		    label: "Tilt",
		    data: TimeTiltData 
		}, 
		{
		    label: "Throw",
		    data: TimeThrowData 
		}, 
		{
		    label: "Swipe",
		    data: TimeSwipeData 
		}, 
		{
		    label: "Pinch",
		    data: TimePinchData 
		}
    ];
	
    $.plot("#hitpercentage", data,
		{
		    yaxis: {
		        ticks: 10,
		        axisLabel: "Percent (%)",
		        axisLabelFontFamily: 'Times New Roman',
		        max: 100,
		        tickDecimals: 0
		    }, 
		    xaxis: {
		        ticks: 18,
		        min: 1,
		        axisLabel: "Target",
		        axisLabelFontFamily: 'Times New Roman',
		        max: 18,
		        tickDecimals: 0
		    },
		    grid: {
		        borderWidth: 1
		    },
		    legend: { 
		        position: "se",
		        backgroundOpacity: 0.5,
		        container: "#HitPercentlegend"
		    }	
		}
	);

    $.plot("#timepertarget", timeData, {
        series: {
            bars: {
                show: true,
                barWidth: 0.2,
                lineWidth: 0.01,
                order: 1,
                fillColor: {
                    colors: [{ opacity: 1 }, { opacity: 1 },
	                		{ opacity: 1 }, { opacity: 1 }]
                }
            }
        },
        xaxis: {
            mode: "categories",
            tickLength: 0
        },
        grid: {
            borderWidth: 1
        },
        legend: {
            position: "se",
            backgroundOpacity: 0.5,
            container: "#TimeLegend"
        }
    });
});	