const btnHam = document.querySelector('.ham-btn'),
    btnTimes = document.querySelector('.times-btn'),
    navBar = document.getElementById('nav-bar'),
    arrow = document.querySelector('.arrowUP-none'),
    arrowUP = document.querySelector('.arrowUP'),
    btn = document.querySelector('.regs'),
    header = document.getElementById('header');


btnHam.addEventListener('click', function () {
    if (btnHam.className !== "") {
        btnHam.style.display = "none";
        btnTimes.style.display = "block";
        navBar.classList.add("show-nav");
    }
})

btnTimes.addEventListener('click', function () {
    if (btnHam.className !== "") {
        this.style.display = "none";
        btnHam.style.display = "block";
        navBar.classList.remove("show-nav");
    }
})



// ==================================================================

function scroll() {

    window.onscroll = function showHeader() {
        if (window.pageYOffset > 200) {
            arrow.classList.add('arrowUP');
        } else {
            arrow.classList.remove('arrowUP');
        }
    };
}

scroll();

// ==============================Регистр===========================================