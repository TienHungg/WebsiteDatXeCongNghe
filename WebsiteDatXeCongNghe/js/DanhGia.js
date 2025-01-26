let menu = document.querySelector('#menu-btn');
let navbar = document.querySelector('.navbar');

menu.onclick = () => {
    menu.classList.toggle('fa-times');
    navbar.classList.toggle('active');
}

document.querySelector('#login-btn').onclick = () => {
    document.querySelector('.login-form-container').classList.toggle('active');
}



window.onscroll = () => {

    menu.classList.remove('fa-times');
    navbar.classList.remove('active');

    if (window.scrollY > 0) {
        document.querySelector('.header').classList.add('active');
    } else {
        document.querySelector('.header').classList.remove('active');
    };

};



var swiper = new Swiper(".vehicles-slider", {
    grabCursor: true,
    centeredSlides: true,
    spaceBetween: 20,
    loop: true,
    autoplay: {
        delay: 9500,
        disableOnInteraction: false,
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});

var swiper = new Swiper(".featured-slider", {
    grabCursor: true,
    centeredSlides: true,
    spaceBetween: 20,
    loop: true,
    autoplay: {
        delay: 9500,
        disableOnInteraction: false,
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});

var swiper = new Swiper(".review-slider", {
    grabCursor: true,
    centeredSlides: true,
    spaceBetween: 20,
   /* loop: true,*/
    autoplay: {
        delay: 9500,
        disableOnInteraction: false,
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});





//$(document).ready(function () {
//    $("#text").keyup(function (event) {
//        if (event.keyCode == 13) {
//            var selectedOption = $("input:radio[name=rate]:checked").val();
//            var ele = document.getElementsByName('rate');
//            for (i = 0; i < ele.length; i++) {
//                if (ele[i].checked)
//                    document.getElementById("value_rating").innerHTML = "" + ele[i].value;
//            }
//            if ($.trim($("#text").val()) != '') {
//                $.ajax({
//                    url: "ThemDanhGiaUngDung",
//                    type: "post",
//                    dataType: "json",
//                    data: {
//                        phone: $("#phone").text(),
//                        date: new Date().toISOString(),
//                        value_rate: selectedOption,
//                        content: $("#text").val()
//                    },
//                    success: function (data) {
//                        var newReview = '<div class="swiper-slide box">' +
//                            '<img src="/image/KhachHang/' + data.HinhAnh + '" alt="" class="picture">' +
//                            '<div class="content">' +
//                            '<p class="date">' + data.NgayDanhGia + '</p>' +
//                            '<h3 class="name">' + data.Ten + '</h3>' +
//                            '<p class="content">' + data.NoiDung + '</p>' +
//                            '<div class="stars">';
//                        for (var i = 0; i < data.MucDo; i++) {
//                            newReview += '<i class="fas fa-star"></i>';
//                        }
//                        newReview += '</div></div></div>';

//                        var $swiperWrapper = $(".swiper-wrapper");
//                        $swiperWrapper.append(newReview);

//                        // Initialize Swiper after appending new review
//                        var swiper = new Swiper('.review-slider', {
//                            grabCursor: true,
//                            centeredSlides: true,
//                            spaceBetween: 20,
//                            autoplay: {
//                                delay: 9500,
//                                disableOnInteraction: false,
//                            },
//                            speed: 500, // Adjust the speed of the sliding animation (in milliseconds)
//                            slidesPerView: 'auto', // Adjust the number of slides per view for smoother dragging
//                            pagination: {
//                                el: ".swiper-pagination",
//                                clickable: true,
//                            },
//                            breakpoints: {
//                                0: {
//                                    slidesPerView: 1,
//                                },
//                                768: {
//                                    slidesPerView: 2,
//                                },
//                                1024: {
//                                    slidesPerView: 3,
//                                },
//                            },
//                        });

//                        // Get the index of the newly added slide
//                        var newSlideIndex = $swiperWrapper.children().length - 1;

//                        // Slide to the newly added slide
//                        swiper.slideTo(newSlideIndex, 500); // Adjust the duration of the sliding animation (in milliseconds)
//                        swiper.update(); // Update swiper after sliding to the new slide
//                    }
//                });
//            }
//        }
//    });
//});


//$(document).ready(function () {
//    $("#btnPost").click(function () {
//        var selectedOption = $("input:radio[name=rate]:checked").val();
//        var content = $("#text").val();

//        if ($.trim(content) != '') {
//            $.ajax({
//                url: "ThemDanhGiaUngDung",
//                type: "post",
//                dataType: "json",
//                data: {
//                    phone: $("#phone").text(),
//                    date: new Date().toISOString(),
//                    value_rate: selectedOption,
//                    content: content
//                },
//                success: function (data) {
//                    var newReview = '<div class="swiper-slide box">' +
//                        '<img src="/image/KhachHang/' + data.HinhAnh + '" alt="" class="picture">' +
//                        '<div class="content">' +
//                        '<p class="date">' + data.NgayDanhGia + '</p>' +
//                        '<h3 class="name">' + data.Ten + '</h3>' +
//                        '<p class="content">' + data.NoiDung + '</p>' +
//                        '<div class="stars">';
//                    for (var i = 0; i < data.MucDo; i++) {
//                        newReview += '<i class="fas fa-star"></i>';
//                    }
//                    newReview += '</div></div></div>';

//                    var $swiperWrapper = $(".swiper-wrapper");
//                    $swiperWrapper.append(newReview);

//                    // Initialize Swiper after appending new review
//                    var swiper = new Swiper('.review-slider', {
//                        grabCursor: true,
//                        centeredSlides: true,
//                        spaceBetween: 20,
//                        /* loop: true,*/
//                        autoplay: {
//                            delay: 9500,
//                            disableOnInteraction: false,
//                        },
//                        speed: 500, // Adjust the speed of the sliding animation (in milliseconds)
//                        slidesPerView: 'auto', // Adjust the number of slides per view for smoother dragging
//                        pagination: {
//                            el: ".swiper-pagination",
//                            clickable: true,
//                        },
//                        breakpoints: {
//                            0: {
//                                slidesPerView: 1,
//                            },
//                            768: {
//                                slidesPerView: 2,
//                            },
//                            1024: {
//                                slidesPerView: 3,
//                            },
//                        },
//                    });

//                    // Get the index of the newly added slide
//                    var newSlideIndex = $swiperWrapper.children().length - 1;

//                    // Slide to the newly added slide
//                    swiper.slideTo(newSlideIndex, 500); // Adjust the duration of the sliding animation (in milliseconds)
//                    swiper.update(); // Update swiper after sliding to the new slide
//                }
//            });
//        }
//    });
//});






$(document).ready(function () {
    var currentDate = new Date();
    var day = currentDate.getDate();
    var month = currentDate.getMonth() + 1;
    var year = currentDate.getFullYear();
    var formattedDate = month + '/' + day + '/' + year;
    $("#date").text(formattedDate);
});




    
 