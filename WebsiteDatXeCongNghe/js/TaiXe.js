// Get ride details elements
const pickupLocation = document.getElementById('pickup-location').querySelector('span');
const destination = document.getElementById('destination').querySelector('span');
const distance = document.getElementById('distance').querySelector('span');
const fare = document.getElementById('fare').querySelector('span');

// Get buttons
const acceptRideButton = document.getElementById('accept-ride');
const cancelRideButton = document.getElementById('cancel-ride');

// Set sample ride details
const samplePickupLocation = '123 Main St';
const sampleDestination = '456 Elm St';
const sampleDistance = '5 miles';
const sampleFare = '$10.00';

// Update ride details with sample data
pickupLocation.textContent = samplePickupLocation;
destination.textContent = sampleDestination;
distance.textContent = sampleDistance;
fare.textContent = sampleFare;

// Add event listener to accept ride button
acceptRideButton.addEventListener('click', () => {
    alert('Ride accepted!');
});

// Add event listener to cancel ride button
cancelRideButton.addEventListener('click', () => {
    alert('Ride cancelled.');
});
