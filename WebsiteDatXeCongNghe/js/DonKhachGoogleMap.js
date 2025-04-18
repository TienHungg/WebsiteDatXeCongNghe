﻿"use strict";
var METERS_IN_KM = 1e3;
var SECONDS_IN_MINUTE = 60;
var MINUTES_IN_HOUR = 60;
var STARTING_PRICE = 10;
var PRICE_PER_KM = 4;
var directionsRenderer;
var googleMap;
var marker;
var locationCounter = 0;
var lastLocationTextBox = null;
var distanceInKM = 0;
var totalFare = 0;
var travelPrice = 0;
var mins = 0;
var hrs = 0;
var priceCalculated = false;
var directionsRenderer1;
var markerA;
var markerB;
var durationInMillis = 0;
var durationValue = 0;
var infoWindow;
var countdown;
var speedValue = 0; // Initial speed in km/h
var polyline;
var hiddenPolylines = [];
var map;

$(document).ready(function () {
    $("#fareDetails").hide();
    $("#btnClearDirections").click(function () {
        if (confirm("Bạn có muốn xoá hết tất cả các vị trị tìm kiếm?")) {
            $("#fareDetails").hide();
            priceCalculated = false;
            distanceInKM = 0;
            totalFare = 0;
            travelPrice = 0;
            mins = 0;
            hrs = 0;
            if (directionsRenderer) {
                directionsRenderer.setMap(null)
            }
            if (directionsRenderer1) {
                directionsRenderer1.setMap(null)
            }
            $("#frmLocation").get(0).reset();
            $("#locations").empty();
            locationCounter = 0;
            displayInitialLocations();
            if (marker) {
                marker.setMap(null);
                marker = null
            }
        }
    });
    $("#btnAddLocation").click(function () {
        locationCounter++;
        var locationTextBoxId = "txtLocation" + locationCounter;
        addLocationBox(locationTextBoxId);
        addAutoComplete(locationTextBoxId);
        priceCalculated = false
    });
    $("#btnGetJson").click(function () {
        if (!priceCalculated) {
            alert("Vui lòng tính giá tiền trước.");
            return
        }
        var locations = [];
        $('#locations input[type="text"]').each(function (index, locationBox) {
            var location = { address: $(locationBox).val(), lat: $(locationBox).data("lat"), lng: $(locationBox).data("lng") };
            locations.push(location)
        });
        var data = {
            locations: locations, distanceInKM: distanceInKM, startingPrice: STARTING_PRICE, travelPrice: travelPrice, totalFare: totalFare, mins: mins, hrs: hrs
        };
        alert(JSON.stringify(data))
    });
    $("#btnCalculateFare").click(function () {
        if (directionsRenderer) {
            directionsRenderer.setMap(null)
        }
        if (marker) {
            marker.setMap(null);
            marker = null
        }
        $("#fareDetails").hide();
        var directionsService = new google.maps.DirectionsService;
        directionsRenderer = new google.maps.DirectionsRenderer
            ({ map: googleMap, draggable: false });
        var source = null;
        var destination = null;
        var location = null;
        var wayPoints = [];
        var valid = true;
        var count = 0;
        $('#locations input[type="text"]').each(function (index, locationBox) {
            count++;
            if ($(locationBox).val().trim() === "") {
                alert("Vui lòng chọn vị trí.");
                $(locationBox).focus();
                valid = false;
                return false
            }
            if (index === 0) {
                source = $(locationBox).data("lat") + " " + $(locationBox).data("lng")
            } else {
                if (location !== null) {
                    var wayPoint = { location: location, stopover: true };
                    wayPoints.push(wayPoint)
                }
                location = $(locationBox).data("lat") + " " + $(locationBox).data("lng")
            }
        });
        destination = location;
        if (!valid) {
            return
        } else {
            if (count === 0) {
                alert("Please add source and destination locations.");
                return
            } else if (count === 1) {
                alert("Please add destination location.");
                return
            }
        }
        var directionsRequest =
        {
            origin: source, destination: destination, travelMode: google.maps.TravelMode["DRIVING"], waypoints: wayPoints, optimizeWaypoints:
                $("#chkOptimizePath").is(":checked")
        };
        directionsService.route(directionsRequest,
            function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsRenderer.setDirections(response);
                    var distanceInMeters = 0;
                    var timeInSeconds = 0;
                    for (var i = 0; i < response.routes[0].legs.length; i++) {
                        var leg = response.routes[0].legs[i];
                        distanceInMeters += leg.distance.value;
                        timeInSeconds += leg.duration.value
                    }
                    distanceInKM = distanceInMeters / METERS_IN_KM;
                    var durationInMins = timeInSeconds / SECONDS_IN_MINUTE;
                    var durationInHours = durationInMins / MINUTES_IN_HOUR;
                    travelPrice = PRICE_PER_KM * distanceInKM;
                    totalFare = STARTING_PRICE + travelPrice;
                    var hoursMins = "";
                    mins = 0;
                    hrs = 0;
                    if (durationInHours < 1) {
                        mins = durationInMins.toFixed(0);
                        hoursMins = mins + " phút" + (mins > 1 ? "" : "")
                    } else {
                        mins = (durationInMins % MINUTES_IN_HOUR).toFixed(0);
                        hrs = Math.floor(durationInHours);
                        hoursMins = hrs + " giờ" + (hrs > 1 ? "" : "") + ", " + mins + " phút" + (mins > 1 ? "" : "")
                    }
                    $("#distance").text(distanceInKM.toFixed(2) + " KM");
                    $("#duration").text(hoursMins);
                    $("#startingPrice").text(STARTING_PRICE.toFixed(3) + " VND ");
                    $("#travelPrice").text(travelPrice.toFixed(3) + " VND ");
                    $("#fare").text(totalFare.toFixed(3) + " VND ");
                    $("#totalFare").text(totalFare.toFixed(3) + " VND ");
                    $("#type").text("Xe máy")
                    $("#fareDetails").show();
                    priceCalculated = true
                } else {
                    alert("Không tìm thấy kết quả.")
                }
            })
    })
    $("#btnBooking").click(function (e) {
        if (!priceCalculated) {
            alert("Vui lòng tính giá tiền trước.");
            return
        }
        var passengerImage = 1; // Default value
        var Status = null;



        e.preventDefault();
        $.ajax({
            url: "GoogleMap",
            type: "post",
            dataType: "html",
            data: {
                distance: $("#distance").text(), duration: $("#duration").text(),
                totalFare: $("#totalFare").text(), location1: $("#txtLocation1").val(), location2: $("#txtLocation2").val(),
                type: $("#type").text(), phone: $("#phone-number").text(), payment: $("#result").text(), DateTime: $("#dated").text(), Status: Status, PassengerImage: passengerImage, PassengerName: $("#PassengerName").text()
            },
            success: function (result) {

            }
        });
    });

    $("#btnGetCurrentLocation").click(function () {
        // Get current location using HTML5 Geolocation API
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var currentLatLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                // Create a marker for the current location
                if (marker) {
                    marker.setMap(null);
                }
                marker = new google.maps.Marker({
                    position: currentLatLng,
                    map: googleMap,
                    title: "Current location",
                });
                // Center the map on the current location
                googleMap.panTo(currentLatLng);
                // Zoom the map to the current location
                googleMap.setZoom(15);
                // Reverse geocode the current location to get the address
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'location': currentLatLng }, function (results, status) {
                    if (status === 'OK') {
                        // Display the address in the txtLocation1 element
                        var address = results[0].formatted_address;
                        $("#txtLocation1").val(address);
                        // Set the data attributes for the txtLocation1 element
                        $("#txtLocation1").data("lat", position.coords.latitude);
                        $("#txtLocation1").data("lng", position.coords.longitude);
                        // Calculate the fare if there are at least two locations
                        if ($("#locations input[type='text']").length >= 2) {
                            calculateFare();
                        }
                    } else {
                        alert('Trình mã hóa địa lý không thành công do: ' + status);
                    }
                });
            }, function () {
                alert("Không thể truy xuất vị trí của bạn.");
            });
        } else {
            alert("Định vị địa lý không được trình duyệt này hỗ trợ.");
        }
    });
    $("#btnDirection").click(function (e) {
        if (!priceCalculated) {
            alert("Vui lòng tính giá tiền trước.");
            return
        }
        console.log("Direction clicked!!!");
        simulateMovement(googleMap);
    });

    // Hide the popup, clear the interval, and remove markers when clicking the exit button
    var btnExit = document.getElementById('btnExit');
    btnExit.addEventListener('click', function () {
        var popup = document.getElementById('popup');
        var countdownElement = document.getElementById('countdown');
        popup.style.display = 'none';
        clearInterval(countdown);
        directionsRenderer.setMap(null); // Remove the route from the map

        // Clear markerA and markerB if they exist
        if (markerA) {
            markerA.setMap(null);
            markerA = null;
        }
        if (markerB) {
            markerB.setMap(null);
            markerB = null;
        }
    });
    // Stop button click handler
    $("#stopButton").click(function (event) {
        event.preventDefault();
        stopMoving();
    });

    // Next button click handler
    $("#nextButton").click(function (event) {
        event.preventDefault();
        nextButtonClicked();
    });

    // Accelerate button click handler
    $("#accelerateButton").click(function (event) {
        event.preventDefault();
        accelerate();
    });

    // Decelerate button click handler
    $("#decelerateButton").click(function (event) {
        event.preventDefault();
        decelerate();
    });

    // Accident button click handler
    $("#accidentButton").click(function (event) {
        event.preventDefault();
        accident();
    });
    checkMarkerPosition();
});
function initializeMap() {
    displayInitialLocations();
    var latLng = new google.maps.LatLng(10.803837577744153, 106.71660094469999);
    var mapOptions = { center: latLng, zoom: 15 };
    googleMap = new google.maps.Map(document.getElementById("googleMap"), mapOptions);
    google.maps.event.addListener(googleMap, "click", function (e) {
        if (lastLocationTextBox === null) {
            return
        }
        if (marker) {
            marker.setMap(null);
            marker = null
        }
        var geocoder = new google.maps.Geocoder;
        geocoder.geocode({ latLng: e.latLng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    var address = results[0].formatted_address;
                    marker = new google.maps.Marker({ position: e.latLng, map: googleMap, title: address });
                    $("#" + lastLocationTextBox).val(address);
                    $("#" + lastLocationTextBox).data("lat", e.latLng.lat());
                    $("#" + lastLocationTextBox).data("lng", e.latLng.lng());
                    priceCalculated = false
                }
            }
        }
        )
    }
    )
}
function addAutoComplete(textBox) {
    $("#" + textBox).focus(function () {
        lastLocationTextBox = textBox;
    });

    var autoComplete = new google.maps.places.Autocomplete(document.getElementById(textBox));
    autoComplete.addListener("place_changed", function () {
        var place = autoComplete.getPlace();
        displayPlaceOnMap(place, textBox);
    });

    // Manually handle pasted data
    $("#" + textBox).on('input', function () {
        var inputLocation = $(this).val();
        if (inputLocation) {
            geocodeAddress(inputLocation, function (place) {
                displayPlaceOnMap(place, textBox);
            });
        }
    });
}

function geocodeAddress(address, callback) {
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            var place = results[0];
            callback(place);
        } else {

        }
    });
}

function displayPlaceOnMap(place, textBox) {
    if (place.geometry) {
        googleMap.setCenter(place.geometry.location);
        googleMap.setZoom(15);

        $("#" + textBox).data("lat", place.geometry.location.lat());
        $("#" + textBox).data("lng", place.geometry.location.lng());
        priceCalculated = false;

        if (marker) {
            marker.setMap(null);
            marker = null;
        }

        marker = new google.maps.Marker({
            position: place.geometry.location,
            map: googleMap,
            title: place.formatted_address
        });
    }
}




function addLocationBox(locationTextBoxId) {
    $("#locations").append('<div class="form-group" id="' + locationTextBoxId + 'Fields">' + '<label for="' + locationTextBoxId + '" \n\
        class="sr-only">Location</label>' + '<input id="' + locationTextBoxId + '" type="text" placeholder="Địa điểm" \n\
        class="form-control location-box" />' + "<a href=\"javascript:removeLocation('" + locationTextBoxId + "Fields');\">" + '<span \n\
        class="glyphicon glyphicon-remove remove-icon"></span>' + "</a>" + "</div>")
}
function removeLocation(locationFields) {
    $("#" + locationFields).remove();
    priceCalculated = false;
    locationCounter--;
    // renumber remaining location text boxes
    $("#locations .form-group").each(function (index) {
        var locationTextBoxId = "txtLocation" + (index + 1);
        $(this).attr("id", locationTextBoxId + "Fields");
        $(this).find("label").attr("for", locationTextBoxId);
        $(this).find("input").attr("id", locationTextBoxId);
    });
}

function displayInitialLocations() {
    locationCounter++;
    var locationTextBoxId = "txtLocation" + locationCounter;
    addLocationBox(locationTextBoxId);
    addAutoComplete(locationTextBoxId);
    locationCounter++;
    var locationTextBoxId = "txtLocation" + locationCounter;
    addLocationBox(locationTextBoxId);
    addAutoComplete(locationTextBoxId);
    var locationBox = document.createElement("input");
    locationBox.type = "text";
    locationBox.id = locationTextBoxId;
}
google.maps.event.addDomListener(window, "load", initializeMap);

function simulateMovement() {
    setTimeout(function () {
        var directionsService = new google.maps.DirectionsService();
        if (!directionsRenderer) {
            directionsRenderer = new google.maps.DirectionsRenderer({
                map: googleMap,
            });
        } else {
            directionsRenderer.setMap(googleMap);
        }

        if (!markerA) {
            markerA = new google.maps.Marker({
                map: googleMap,
                icon: {
                    url: '/image/TaiXe/scooter navigator3.png',
                    scaledSize: new google.maps.Size(48, 48),
                    anchor: new google.maps.Point(24, 24),
                    rotation: 0, // Initial rotation angle
                },
            });
        }

        if (!markerB) {
            markerB = new google.maps.Marker({
                map: googleMap,
                icon: {
                    path: google.maps.SymbolPath.CIRCLE,
                    fillColor: '#f00',
                    fillOpacity: 1,
                    strokeColor: '#f00',
                    strokeOpacity: 1,
                    strokeWeight: 1,
                    scale: 6,
                },
            });
        }
        infoWindow = new google.maps.InfoWindow();
        var pickupAddress = document.getElementById('txtLocation1').value;
        var dropoffAddress = document.getElementById('txtLocation2').value;

        var request = {
            origin: pickupAddress,
            destination: dropoffAddress,
            travelMode: google.maps.TravelMode.DRIVING,
        };

        directionsService.route(request, function (result, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsRenderer.setDirections(result);

                var leg = result.routes[0].legs[0];
                var distance = leg.distance.text;
                var duration = leg.duration.text;

                markerA.setPosition(leg.start_location);
                markerB.setPosition(leg.end_location);

                // After the line: markerA.setPosition(leg.start_location);
                // Add the following code to zoom to marker A
                var bounds = new google.maps.LatLngBounds();
                bounds.extend(markerA.getPosition());
                bounds.extend(markerB.getPosition());

                // Set maximum zoom level to ensure markers are visible
                var maxZoomLevel = 16; // Adjust the maximum zoom level as needed
                googleMap.fitBounds(bounds);

                google.maps.event.addListenerOnce(googleMap, 'idle', function () {
                    setTimeout(function () {
                        if (googleMap.getZoom() < maxZoomLevel) {
                            googleMap.setZoom(maxZoomLevel);
                        }
                    }, 1000); // Adjust the delay (in milliseconds) as needed
                });

                google.maps.event.addListener(markerA, 'click', function () {
                    infoWindow.setContent('Start Location');
                    infoWindow.open(googleMap, markerA);
                });

                google.maps.event.addListener(markerB, 'click', function () {
                    infoWindow.setContent('End Location');
                    infoWindow.open(googleMap, markerB);
                });

                document.getElementById('distance').textContent = distance;
                document.getElementById('duration').textContent = duration;

                // Show the popup
                var popup = document.getElementById('popup');
                popup.style.display = 'block';

                // Calculate and display remaining distance
                var distanceLeft = document.getElementById('distanceLeft');
                distanceLeft.textContent = distance;

                // Calculate and display remaining time
                var timeLeft = document.getElementById('timeLeft');
                timeLeft.textContent = duration;

                // Calculate and display speed
                var speed = document.getElementById('speed');
                var distanceValue = parseFloat(distance.replace(',', ''));
                durationValue = parseFloat(duration.split(' ')[0]);
                var speedValue = distanceValue / durationValue;
                speed.textContent = speedValue.toFixed(2) + ' km/h';

                // Start counting down
                var startTime = new Date().getTime();
                durationInMillis = durationValue * 60 * 1000; // Convert duration to milliseconds

                countdown = setInterval(function () {
                    var currentTime = new Date().getTime();
                    var elapsedTime = currentTime - startTime;
                    var remainingTime = durationInMillis - elapsedTime;

                    if (remainingTime <= 0) {
                        clearInterval(countdown);
                        popup.style.display = 'none';
                    } else {
                        // Calculate and display remaining time
                        var hours = Math.floor(remainingTime / (1000 * 60 * 60));
                        var minutes = Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60));
                        var seconds = Math.floor((remainingTime % (1000 * 60)) / 1000);

                        var timeLeftText =
                            (hours < 10 ? '0' + hours : hours) +
                            ':' +
                            (minutes < 10 ? '0' + minutes : minutes) +
                            ':' +
                            (seconds < 10 ? '0' + seconds : seconds);
                        timeLeft.textContent = timeLeftText;

                        // Calculate and display remaining distance
                        var remainingDistance = (remainingTime / durationInMillis) * distanceValue;
                        distanceLeft.textContent = remainingDistance.toFixed(2) + ' km';

                        // Calculate and display speed
                        var currentSpeed = remainingDistance / (remainingTime / (50 * 160 * 10));
                        speed.textContent = currentSpeed.toFixed(2) + ' km/h';

                        // Move marker along the route
                        var route = result.routes[0];
                        var totalDistance = google.maps.geometry.spherical.computeLength(route.overview_path);
                        var targetDistance = (elapsedTime / durationInMillis) * totalDistance;

                        var interpolatedPoint = getInterpolatedPoint(targetDistance, route.overview_path);

                        // Smoothly move marker to the interpolated point
                        smoothMoveMarker(markerA, interpolatedPoint);


                        // Calculate the closest route segment to the interpolated point
                        var closestSegmentIndex = getClosestSegmentIndex(interpolatedPoint, route.overview_path);

                        // Calculate the heading based on the closest segment
                        var closestSegment = [route.overview_path[closestSegmentIndex], route.overview_path[closestSegmentIndex + 1]];
                        var heading = google.maps.geometry.spherical.computeHeading(closestSegment[0], closestSegment[1]);

                        // Adjust the arrow rotation
                        var arrowRotation = (heading !== undefined) ? heading : markerA.getRotation();
                        markerA.setIcon({
                            url: '/image/TaiXe/scooter navigator3.png',
                            scaledSize: new google.maps.Size(48, 48),
                            anchor: new google.maps.Point(24, 24),
                            rotation: arrowRotation,
                        });


                        // Helper function to smoothly move the marker to the destination point
                        function smoothMoveMarker(marker, destination) {
                            var startPosition = marker.getPosition();
                            var startTime = new Date().getTime();
                            var duration = 400; // Adjust the duration (in milliseconds) as desired

                            function animateMarker() {
                                var currentTime = new Date().getTime();
                                var elapsedTime = currentTime - startTime;
                                var fraction = elapsedTime / duration;

                                if (fraction >= 1) {
                                    marker.setPosition(destination);
                                } else {
                                    var newPosition = google.maps.geometry.spherical.interpolate(startPosition, destination, fraction);
                                    marker.setPosition(newPosition);
                                    startPosition = newPosition; // Update the start position for the next frame

                                    // Pan the map only when the marker is outside the visible bounds
                                    var bounds = googleMap.getBounds();
                                    if (!bounds.contains(newPosition)) {
                                        googleMap.panTo(newPosition);
                                    }

                                    requestAnimationFrame(animateMarker);
                                }
                            }

                            animateMarker();
                            //// Pan the map only when the marker is outside the visible bounds
                            //googleMap.panTo(interpolatedPoint);
                        }

                    }
                }, 50);
            }
        });
    }, 500);
}

// Helper function to get the interpolated point on the route
function getInterpolatedPoint(targetDistance, path) {
    for (var i = 0; i < path.length - 1; i++) {
        var segmentDistance = google.maps.geometry.spherical.computeDistanceBetween(path[i], path[i + 1]);
        if (segmentDistance >= targetDistance) {
            var fraction = targetDistance / segmentDistance;
            return google.maps.geometry.spherical.interpolate(path[i], path[i + 1], fraction);
        }
        targetDistance -= segmentDistance;
    }
    return path[path.length - 1]; // Fallback to the last point on the route
}

// Helper function to get the index of the closest segment to a given point
function getClosestSegmentIndex(point, path) {
    var closestSegmentIndex = 0;
    var closestDistance = google.maps.geometry.spherical.computeDistanceBetween(point, path[0]);

    for (var i = 1; i < path.length - 1; i++) {
        var distance = google.maps.geometry.spherical.computeDistanceBetween(point, path[i]);
        if (distance < closestDistance) {
            closestDistance = distance;
            closestSegmentIndex = i;
        }
    }

    return closestSegmentIndex;
}

// In your googleMap.min.js file



// Function to check the marker positions and control the button display
function checkMarkerPosition() {
    // Define a variable to keep track of the "Pickup-passenger" button
    var pickupButton = document.getElementById('Dropoff-passenger');
    var btnDirection = document.getElementById('btnDirection');
    var DonKhachInfors = document.getElementById('TraKhachInfors');
    var DonKhachInfors2 = document.getElementById('TraKhachInfors2');
    var NhanChuyenXeInfors = document.getElementById('NhanChuyenXeInfors');
    var NhanChuyenXeInfors2 = document.getElementById('NhanChuyenXeInfors2');
    var NhanChuyenXeInfors3 = document.getElementById('NhanChuyenXeInfors3');
    var markerAPosition = markerA.getPosition();
    var markerBPosition = markerB.getPosition();
    // Check if markerA and markerB are defined
    if (markerA && markerB) {
        // Calculate the distance between marker A and marker B
        var distance = google.maps.geometry.spherical.computeDistanceBetween(markerAPosition, markerBPosition);

        // Set a threshold distance (in meters) for considering that marker A has reached marker B
        var thresholdDistance = 150; // You can adjust this value

        if (distance <= thresholdDistance) {
            // Marker A has reached marker B, display the button
            pickupButton.style.display = 'block';
            btnDirection.style.display = 'none';
            DonKhachInfors.style.display = 'none';
            DonKhachInfors2.style.display = 'block';
            NhanChuyenXeInfors.style.display = 'none';
            NhanChuyenXeInfors3.style.display = 'block';

        } else {
            // Marker A is not near marker B, hide the button
            pickupButton.style.display = 'none';
            btnDirection.style.display = 'block';

            // Set the label text to "Đang Di Chuyển..." when marker A is moving
            DonKhachInfors.innerHTML = 'Đang Di Chuyển...';
            NhanChuyenXeInfors2.style.display = 'block';
        }
    }
}

// Call checkMarkerPosition periodically to check the marker positions
setInterval(checkMarkerPosition, 1000); // Adjust the interval (in milliseconds) as needed

// Stop button click handler
function stopMoving() {
    speedValue += 0; // Increase speed by 10 km/h

    // Calculate the new durationInMillis based on the updated speedValue
    // You can adjust this calculation as needed
    durationInMillis = durationValue * 60 * 1000;

    swal("Speed has stop in " + speedValue + " km/h", {
        icon: "success",
    });
}

// Next button click handler
function nextButtonClicked() {
    speedValue += 0; // Increase speed by 10 km/h

    // Calculate the new durationInMillis based on the updated speedValue
    // You can adjust this calculation as needed
    durationInMillis = durationValue * 60 * 1000;

    swal("Speed has resume in " + speedValue + " km/h", {
        icon: "success",
    });
}

// Function to handle the "Accelerate" button click
function accelerate() {
    speedValue += 100; // Increase speed by 10 km/h

    // Calculate the new durationInMillis based on the updated speedValue
    // You can adjust this calculation as needed
    durationInMillis = durationValue * 60 * 1000 / (speedValue / 60);


}

// Decelerate button click handler
function decelerate() {
    speedValue -= 20; // Increase speed by 10 km/h

    // Calculate the new durationInMillis based on the updated speedValue
    // You can adjust this calculation as needed
    durationInMillis = durationValue * 60 * 1000 / (speedValue / 20);


}

// Accident button click handler
function accident() {
    speedValue += 0; // Increase speed by 10 km/h

    // Calculate the new durationInMillis based on the updated speedValue
    // You can adjust this calculation as needed
    durationInMillis = durationValue * 60 * 1000;

    swal("Driver have some trouble in " + speedValue + " km/h", {
        icon: "success",
    });
}




