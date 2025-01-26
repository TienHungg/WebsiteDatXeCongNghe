// Variable to store the chart instance
var revenueChart;

// Function to fetch revenue data based on the selected period, month, and year
function fetchRevenueData() {
    var period = document.getElementById("period").value;
    var month = document.getElementById("selected-month").value;
    var year = document.getElementById("selected-year").value;
    var date = null;

    if (period === "day") {
        date = document.getElementById("selected-date").value;
    }

    $.ajax({
        url: "/ThongKe/GetRevenueData2",
        type: "GET",
        data: { period: period, selectedDate: date, month: month, year: year },
        success: function (data) {
            // Destroy the previous chart if it exists
            if (revenueChart) {
                revenueChart.destroy();
            }

            // Generate the chart based on the revenue data
            generateChart(data, period);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log(textStatus + ": " + errorThrown);
        }
    });
}

// Function to generate the chart based on the revenue data and selected period
function generateChart(revenueData, period) {
    var chartData = {
        labels: revenueData.map(function (data) {
            return data.Label;
        }),
        datasets: [{
            data: revenueData.map(function (data) { return data.TotalRevenue; }),
            backgroundColor: "rgba(75, 192, 192, 0.6)",
            borderColor: "rgba(75, 192, 192, 1)",
            borderWidth: 1
        }]
    };

    var chartOptions = {
        responsive: true,
        plugins: {
            datalabels: {
                anchor: "end",
                align: "top",
                color: "black",
                font: {
                    family: "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif",
                    weight: "bold"
                },
                formatter: function (value) {
                    return value.toLocaleString("en-US", {
                        style: "currency",
                        currency: "VND"
                    });
                }
            }
        },
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    callback: function (value) {
                        return value.toLocaleString("en-US", {
                            style: "currency",
                            currency: "VND"
                        });
                    }
                }
            }
        }
    };

    // Create the chart
    var ctx = document.getElementById("revenueChart").getContext("2d");
    revenueChart = new Chart(ctx, {
        type: "bar",
        data: chartData,
        options: chartOptions
    });
}

// Event listener to show/hide the date selection based on the period
document.getElementById("period").addEventListener("change", function () {
    var period = this.value;
    var dateSelection = document.getElementById("date-selection");

    if (period === "day") {
        dateSelection.style.display = "block";
    } else {
        dateSelection.style.display = "none";
    }
});
