// Wait for the HTML document to finish loading before running any JavaScript code
document.addEventListener('DOMContentLoaded', function () {
    window.onload = function () {
        const starsContainer = document.querySelector('.stars');
        const ratingInfo = document.querySelector('.rating-info'); // Target element to display the star value
        const currentDateElement = document.querySelector('.current-date'); // Target element to display the current date

        // Create 5 stars dynamically
        for (let i = 1; i <= 5; i++) {
            const star = document.createElement('span');
            star.classList.add('star');
            star.innerText = '★';
            starsContainer.appendChild(star);
        }

        const stars = document.querySelectorAll('.star');

        // Attach click event listeners to stars
        stars.forEach(function (star) {
            star.addEventListener('click', function () {
                // Remove selected class from all stars
                stars.forEach(function (s) {
                    s.classList.remove('selected-stars');
                });

                // Add selected class to the clicked star and all previous stars
                var clickedStarIndex = Array.prototype.indexOf.call(stars, star);
                for (var i = 0; i <= clickedStarIndex; i++) {
                    stars[i].classList.add('selected-stars');
                }

                // Get the value of the selected star
                const selectedStarValue = clickedStarIndex + 1;

                // Display the star value in the ratingInfo element
                ratingInfo.innerText = selectedStarValue;

                // Get the current date
                const currentDate = new Date();
                const formattedDate = currentDate.toLocaleDateString('en-US');

                // Display the current date in the currentDateElement
                currentDateElement.innerText = formattedDate;
            });
        });
    };
});