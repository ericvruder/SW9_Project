
function loadGraphs(source) {
    $.ajax({

        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        url: '/Home/GetTechniqueData/',
        data: { "source": source },
        error: function (asd, errorText) {
            alert("An error occurred: " + errorText);
        },
        complete: function (data) {

            var data = JSON.parse(data.responseText);
            console.log(data["PushHitRate"]);

            var options = {
                legend: { position: "sw", show: true }, series: { lines: { show: false } },
                xaxis: { min: 0.5, max: 4.5, ticks: [[1, 'Pinch'], [2, 'Swipe'], [3, 'Throw'], [4, 'Tilt']] }
            };

            var std_bars = {
                //do not show points
                radius: 0,
                errorbars: "y",
                yerr: { show: true, upperCap: "-", lowerCap: "-", radius: 5 }
            };

            var hitData = [{ color: "green", lines: { show: true }, points: std_bars, data: data["PushHitRate"], label: "Push" },
                            { color: "red", lines: { show: true }, points: std_bars, data: data["PullHitRate"], label: "Pull" }];

            var timeData = [{ color: "green", lines: { show: true }, points: std_bars, data: data["PushTime"], label: "Push" },
                            { color: "red", lines: { show: true }, points: std_bars, data: data["PullTime"], label: "Pull" }];

            var accData = [{ color: "green", lines: { show: true }, points: std_bars, data: data["PushAccuracy"], label: "Push" },
                            { color: "red", lines: { show: true }, points: std_bars, data: data["PullAccuracy"], label: "Pull" }];



            options.yaxis = { min: 0, max: 1.5 }
            $.plot("#HitPercentage", hitData, options);

            options.yaxis = { min: 0, max: 16 }
            $.plot("#TimeTaken", timeData, options);

            options.yaxis = { min: -150, max: 300 }
            $.plot("#Accuracy", accData, options);

            $('#TotalUsers').append(data["TotalUsers"]);
            $('#TotalAttempts').append(data["TotalAttempts"]);
            $('#AttemptsPerTechnique').append(data["AttemptsPerTechnique"]);
        }

    });
}
