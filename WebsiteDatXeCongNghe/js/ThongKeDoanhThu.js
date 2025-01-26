document.addEventListener("DOMContentLoaded", function () {
// Function to update the chart based on the selected time period
function updateChart() {
    var chart = document.querySelector(".column-chart");
    var selectedPeriod = document.getElementById("time-period").value;
    var selectedDate = document.getElementById("selected-date").value;
    var selectedMonth = document.getElementById("selected-month").value;
    var selectedYear = document.getElementById("selected-year").value;

    // Clear the existing columns
    chart.innerHTML = "";

    // Generate new columns based on the selected period
    var numColumns;
    var columnHeights = []; // Array to store column heights based on revenue data

    if (selectedPeriod === "day") {
        document.getElementById("date-selection").style.display = "block";
        numColumns = 1;
        // Append selected date to the heading
        document.querySelector("h2").textContent = "Thống kê doanh thu ngày " + selectedDate;
        columnHeights.push(getRevenueForDay(selectedMonth, selectedYear));
    } else if (selectedPeriod === "week") {
        document.getElementById("date-selection").style.display = "none";
        numColumns = 4;
        document.querySelector("h2").textContent = "Thống kê doanh thu tuần " + selectedMonth + ", năm " + selectedYear;
        columnHeights = getRevenueForWeeks(selectedMonth, selectedYear);
    } else if (selectedPeriod === "month") {
        document.getElementById("date-selection").style.display = "none";
        numColumns = 12; // Assuming max 31 days in a month
        document.querySelector("h2").textContent = "Thống kê doanh thu tháng " + selectedMonth + ", năm " + selectedYear;
        columnHeights = getRevenueForMonths(selectedYear);
    } else if (selectedPeriod === "year") {
        document.getElementById("date-selection").style.display = "none";
        numColumns = 1;
        document.querySelector("h2").textContent = "Thống kê doanh thu năm " + selectedYear;
        columnHeights.push(getTotalRevenue(selectedYear));
    }

    // Find the maximum revenue to calculate the height percentage
    var maxRevenue = Math.max(...columnHeights);

    for (var i = 0; i < numColumns; i++) {
        var column = document.createElement("div");
        column.classList.add("column");

        // Calculate the height based on revenue data
        var heightPercentage = (columnHeights[i] / maxRevenue) * 100;
        column.style.height = heightPercentage + "%";

        chart.appendChild(column);
    }

    // Update statistics based on the selected period
    updateStatistics(selectedPeriod, selectedMonth, selectedYear);
}
// Function to update the statistics based on the selected period
function updateStatistics(selectedPeriod, selectedMonth, selectedYear) {
    var statistics = document.querySelector(".statistics");
    statistics.innerHTML = ""; // Clear existing statistics

    if (selectedPeriod === "day") {
        // Get revenue data for the selected day (hard-coded)
        var revenue = getRevenueForDay(selectedMonth, selectedYear);
        createStatistic("Ngày " + selectedMonth, revenue);
    } else if (selectedPeriod === "week") {
        // Get revenue data for all weeks of the selected month and year (hard-coded)
        var weeksData = getRevenueForWeeks(selectedMonth, selectedYear);
        for (var i = 0; i < weeksData.length; i++) {
            createStatistic("Tuần " + (i + 1) + ", Tháng " + selectedMonth, weeksData[i]);
        }
    } else if (selectedPeriod === "month") {
        // Get revenue data for all months of the selected year (hard-coded)
        var monthsData = getRevenueForMonths(selectedYear);
        for (var i = 0; i < monthsData.length; i++) {
            createStatistic("Tháng " + (i + 1) + ", Năm " + selectedYear, monthsData[i]);
        }
    } else if (selectedPeriod === "year") {
        // Get total revenue data for the selected year (hard-coded)
        var totalRevenue = getTotalRevenue(selectedYear);
        createStatistic("Năm " + selectedYear, totalRevenue);
    }
}

// Function to create a statistic element
function createStatistic(label, value) {
    var statistic = document.createElement("div");
    statistic.classList.add("round-statistic");

    var heading = document.createElement("h3");
    heading.textContent = label;

    var valueElement = document.createElement("div");
    valueElement.classList.add("value");
    valueElement.textContent = value.toLocaleString("vi-VN");

    var unitElement = document.createElement("div");
    unitElement.classList.add("unit");
    unitElement.textContent = "VND";

    statistic.appendChild(heading);
    statistic.appendChild(valueElement);
    statistic.appendChild(unitElement);

    var statistics = document.querySelector(".statistics");
    statistics.appendChild(statistic);
}

// Hard-coded revenue data for the selected day (replace with your actual data)
function getRevenueForDay(month, year) {
    // Implement your logic here to get revenue for the specified day
    // Replace the return value with your actual data
    return Math.floor(Math.random() * 100000) + 250000;
}

// Hard-coded revenue data for all weeks of the selected month and year (replace with your actual data)
function getRevenueForWeeks(month, year) {
    // Implement your logic here to get revenue for all weeks of the specified month and year
    // Replace the return value with your actual data
    var weeksData = [];
    for (var i = 0; i < 4; i++) {
        weeksData.push(Math.floor(Math.random() * 500000) + 100000);
    }
    return weeksData;
}

// Hard-coded revenue data for all months of the selected year (replace with your actual data)
function getRevenueForMonths(year) {
    // Implement your logic here to get revenue for all months of the specified year
    // Replace the return value with your actual data
    var monthsData = [];
    for (var i = 0; i < 12; i++) {
        monthsData.push(Math.floor(Math.random() * 500000) + 500000);
    }
    return monthsData;
}

// Hard-coded total revenue data for the selected year (replace with your actual data)
function getTotalRevenue(year) {
    // Implement your logic here to get total revenue for the specified year
    // Replace the return value with your actual data
    return Math.floor(Math.random() * 5000000) + 5000000;
}

// Attach event listeners to the dropdown lists
var dropdownTimePeriod = document.getElementById("time-period");
dropdownTimePeriod.addEventListener("change", updateChart);

var dropdownSelectedDate = document.getElementById("selected-date");
dropdownSelectedDate.addEventListener("change", updateChart);

var dropdownSelectedMonth = document.getElementById("selected-month");
dropdownSelectedMonth.addEventListener("change", updateChart);

var dropdownSelectedYear = document.getElementById("selected-year");
dropdownSelectedYear.addEventListener("change", updateChart);

// Initial chart update on page load
updateChart();
});