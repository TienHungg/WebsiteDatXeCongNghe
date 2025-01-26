"use strict";
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
        e.preventDefault();
        $.ajax({
            url: "GoogleMap",
            type: "post",
            dataType: "html",
            data: {
                distance: $("#distance").text(), duration: $("#duration").text(),
                totalFare: $("#totalFare").text(), location1: $("#txtLocation1").val(), location2: $("#txtLocation2").val(),
                type: $("#type").text(), phone: $("#phone-number").text(), payment: $("#result").text(), DateTime: $("#dated").text()
            },


            success: function (result) {

            }
        });
    })
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
});
function initializeMap() {
    displayInitialLocations();
    var latLng = new google.maps.LatLng(10.803837577744153, 106.71660094469999);
    var mapOptions = { center: latLng, zoom: 8 };
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
        lastLocationTextBox = textBox
    });
    var autoComplete = new google.maps.places.Autocomplete(document.getElementById(textBox));
    autoComplete.addListener("place_changed", function () {
        var place = autoComplete.getPlace();
        googleMap.setCenter(place.geometry.location);
        googleMap.setZoom(15);
        $("#" + textBox).data("lat", place.geometry.location.lat());
        $("#" + textBox).data("lng", place.geometry.location.lng());
        priceCalculated = false;
        if (marker) {
            marker.setMap(null);
            marker = null
        }
        marker = new google.maps.Marker({ position: place.geometry.location, map: googleMap, title: place.formatted_address })
    })
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
    
}
google.maps.event.addDomListener(window, "load", initializeMap);



