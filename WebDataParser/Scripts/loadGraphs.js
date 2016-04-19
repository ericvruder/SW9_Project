
function loadGraphs(source) {

    $.ajax({
        
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        url: '/Home/GetAttemptData/',
        data: { "source": source },
        error: function (asd, errorText) {
            alert("An error occurred: " + errorText);
        },
        complete: function (data) {
            var data = JSON.parse(data.responseText);
            console.log(data["Push"]["HitPercentage"]["Pinch"][0]);

            for (var direction in data) {
                for (var ditem in data[direction]) {
                    for (var technique in data[direction][ditem]) {
                        var count = 0;
                        for (var number in data[direction][ditem][technique]) {
                            data[direction][ditem][technique][number] = [1 + count++, data[direction][ditem][technique][number]];
                        }
                    }
                }
            }

            var options = {
                legend: { position: "sw", show: true }, series: { lines: { show: false } },
                xaxis: { min: 1, max: 18, tickSize: 1 }
            };

            var dataitems = ["HitPercentage", "TimeTaken", "Accuracy"];
            var directions = ["Push", "Pull"];

            for (var direction in directions) {
                for (var ditem in dataitems) {
                    console.log(dataitems[ditem]);

                    var graphData = [{ color: "green", lines: { show: true }, data: data[directions[direction]][dataitems[ditem]]["Pinch"], label: "Pinch" },
                                     { color: "red", lines: { show: true }, data: data[directions[direction]][dataitems[ditem]]["Swipe"], label: "Swipe" },
                                     { color: "blue", lines: { show: true }, data: data[directions[direction]][dataitems[ditem]]["Throw"], label: "Throw" },
                                     { color: "yellow", lines: { show: true }, data: data[directions[direction]][dataitems[ditem]]["Tilt"], label: "Tilt" }];


                    $.plot("#" + directions[direction] + "Attempt" + dataitems[ditem], graphData, options);

                }
            }
        }

    });

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
