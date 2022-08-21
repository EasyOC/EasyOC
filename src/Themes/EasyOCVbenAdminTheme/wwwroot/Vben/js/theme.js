var antCheckboxWrapper = document.getElementsByClassName("ant-checkbox-wrapper");
if (antCheckboxWrapper) {
    for (var i = 0; i < antCheckboxWrapper.length; i++) {
        antCheckboxWrapper[i].addEventListener('change', function (e) {
            var wapper0 = this;
            var wapper1 = this.querySelector('.ant-checkbox');
            var ckbox = this.querySelector('.ant-checkbox-input')
            wapper0.classList.toggle('ant-checkbox-wrapper-checked', ckbox.checked);
            wapper1.classList.toggle('ant-checkbox-checked', ckbox.checked);
        });
    }
}

var antPassowrds = document.getElementsByClassName('ant-input-password');
if (antPassowrds) {
    for (var i = 0; i < antPassowrds.length; i++) {
        var wapper0 = antPassowrds[i];
        var wapper = wapper0.querySelector(".ant-input-password-icon")

        wapper.addEventListener('click', function (e) {

            var password = this.parentElement.parentElement.querySelector('.ant-input');
            type = password.getAttribute('type') === 'password' ? 'text' : 'password';
            password.setAttribute('type', type);
            this.classList.toggle('anticon-eye', type === 'password');
            this.classList.toggle('anticon-eye-invisible', type !== 'password');

            this.querySelector('svg').innerHTML = ''

            if (type === "password") {
                this.querySelector('svg').innerHTML =
                    '<path d="M942.2 486.2Q889.47 375.11 816.7 305l-50.88 50.88C807.31 395.53 843.45 447.4 874.7 512 791.5 684.2 673.4 766 512 766q-72.67 0-133.87-22.38L323 798.75Q408 838 512 838q288.3 0 430.2-300.3a60.29 60.29 0 000-51.5zm-63.57-320.64L836 122.88a8 8 0 00-11.32 0L715.31 232.2Q624.86 186 512 186q-288.3 0-430.2 300.3a60.3 60.3 0 000 51.5q56.69 119.4 136.5 191.41L112.48 835a8 8 0 000 11.31L155.17 889a8 8 0 0011.31 0l712.15-712.12a8 8 0 000-11.32zM149.3 512C232.6 339.8 350.7 258 512 258c54.54 0 104.13 9.36 149.12 28.39l-70.3 70.3a176 176 0 00-238.13 238.13l-83.42 83.42C223.1 637.49 183.3 582.28 149.3 512zm246.7 0a112.11 112.11 0 01146.2-106.69L401.31 546.2A112 112 0 01396 512z"></path>' +
                    '<path d="M508 624c-3.46 0-6.87-.16-10.25-.47l-52.82 52.82a176.09 176.09 0 00227.42-227.42l-52.82 52.82c.31 3.38.47 6.79.47 10.25a111.94 111.94 0 01-112 112z"></path>'
            } else {
                this.querySelector('svg').innerHTML = ('<path d="M942.2 486.2C847.4 286.5 704.1 186 512 186c-192.2 0-335.4 100.5-430.2 300.3a60.3 60.3 0 000 51.5C176.6 737.5 319.9 838 512 838c192.2 0 335.4-100.5 430.2-300.3 7.7-16.2 7.7-35 0-51.5zM512 766c-161.3 0-279.4-81.8-362.7-254C232.6 339.8 350.7 258 512 258c161.3 0 279.4 81.8 362.7 254C791.5 684.2 673.4 766 512 766zm-4-430c-97.2 0-176 78.8-176 176s78.8 176 176 176 176-78.8 176-176-78.8-176-176-176zm0 288c-61.9 0-112-50.1-112-112s50.1-112 112-112 112 50.1 112 112-50.1 112-112 112z"></path>')
            }

        });
    }


}

function strengthVben(options) {

    var settings = Object.assign(
        {
            minLength: 8,
            upperCase: false,
            lowerCase: false,
            numbers: false,
            specialchars: false,
            target: '',
            passwordSelector: '',
            passwordInvaild: '',
        }, options);
    var passwordCtrl = document.querySelector(settings.passwordSelector);
    checkStrength(passwordCtrl.value);

    var capitalletters = 0;
    var lowerletters = 0;
    var numbers = 0;
    var specialchars = 0;

    var upperCase = new RegExp('[A-Z]');
    var lowerCase = new RegExp('[a-z]');
    var number = new RegExp('[0-9]');
    var specialchar = new RegExp('[^A-Za-z0-9]');

    var valid = false;

    createProgressBar(0);

    function checkStrength(value) {

        minLength = value.length >= settings.minLength ? 1 : 0;
        capitalletters = !settings.upperCase || value.match(upperCase) ? 1 : 0;
        lowerletters = !settings.lowerCase || value.match(lowerCase) ? 1 : 0;
        numbers = !settings.numbers || value.match(number) ? 1 : 0;
        specialchars = !settings.specialchars || value.match(specialchar) ? 1 : 0;

        var score = minLength + capitalletters + lowerletters + numbers + specialchars;

        valid = score >= 5;
        document.querySelector(settings.passwordInvaild).style.display = valid ?  'none':'block';
        createProgressBar(score);
    }

    function createProgressBar(score) {
        var target = document.querySelector(settings.target);
        console.log(score)
        target.setAttribute("data-score", score - 1);
    }

    passwordCtrl.addEventListener("keypress", function (event) {
        checkStrength(this.value);
    });
    passwordCtrl.addEventListener("change", function (event) {
        checkStrength(this.value);
    });
    passwordCtrl.addEventListener("drop", function (event) {
        checkStrength(event.originalEvent.dataTransfer.getData("text"));
    });


    document.querySelector('form').addEventListener('submit', function (e) {
        if (!valid) {
            e.preventDefault()
        }
    });
}
