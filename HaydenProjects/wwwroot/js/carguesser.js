
// triggers when the document is ready
$("document").ready(function () {

    // submit easy mode button click
    $("#submitEasy").click(function () {
        isEasyMode = true;
        startGame()
    });

    // submit hard mode button click
    $("#submitHard").click(function () {
        isEasyMode = false;
        startGame()
    });

    // minimize button click
    $("#minimize").click(function () {
        toggleForm();
    });

    // click for body radio buttons
    $(".body input:radio").click(function () {
        $("#submitEasy").html("Guess " + this.value + " Easy Mode");
        $("#submitHard").html("Guess " + this.value + " Hard Mode");
    });

    // click for company radio buttons
    $(".company input:radio").click(function () {
        $("#submitEasy").html("Guess " + this.value + " Easy Mode");
        $("#submitHard").html("Guess " + this.value + " Hard Mode");
    });

    // img loaded event
    $("#carImg").bind('load', function () {
        if ($("#guesser").css("display") == "none")
            $("#guesser").slideToggle();

        if (isEasyMode) {
            $("#title").html($("#submitEasy").html() + ":");
            this.style.transform = "";
        }
        else {
            $("#title").html($("#submitHard").html() + ":");
            this.style.transform = "scale(4)";
        }
    });

    // click one of the choices
    $("#0, #1, #2, #3").click(function () {

        if (canClick) {
            canClick = false;
            $("#guesser").slideToggle();

            // check if answer is correct (don't cheat pls)
            if (ans == parseInt(this.id)) {
                $("#title").html("Correct! Loading...");
                score += 1;
                $("#scoreNum").html(score);
                guessCar();
            }
            else {
                $("#title").html("Incorrect, Answer was " + $("#" + ans).html());
                $("#scoreText").html("Final score: ");
                toggleForm();
                canClick = true;
            }
        }
    });

    // click skip
    $("#skip").click(function () {

        if (canClick) {

            canClick = false;
            $("#guesser").slideToggle();
            $("#title").html("Loading...");
            guessCar();
        }

    });
});

var score = 0;
var ans = 0;

var isEasyMode = true;
var canClick = true;

function toggleForm() {

    if ($("#minimize").html() == "-") {
        $("#minimize").html("+");
    }
    else {
        $("#minimize").html("-");
    }
    $("#formBox").slideToggle();
}

function startGame() {

    if (canClick) {
        canClick = false;
        score = 0;
        $("#scoreText").html("Score: ");
        $("#scoreNum").html(0);
        toggleForm();
        $("#title").html("Loading...");
        $("#title").show();
        $("#scoreBoard").show();
        guessCar();
    }
}

function guessCar() {

    let body = $(".body input:radio:checked").length != 0 ? $(".body input:radio:checked")[0].id : "";
    let company = $(".company input:radio:checked").length != 0 ? $(".company input:radio:checked")[0].id : "";

    // make post request to get the car the user can guess
    $.ajax({
        url: "/api/CarGuesserApi?body=" + body + "&company=" + company + "&isEasyMode=" + isEasyMode,
        type: "POST",
        success: function (response) {

            canClick = true;
            $("#carImg").attr("src", response.imgUrl);
            ans = response.answer;

            $("#0").html(response.choices[0]);
            $("#1").html(response.choices[1]);
            $("#2").html(response.choices[2]);
            $("#3").html(response.choices[3]);
        },
        error: function () {
            canClick = true;
            console.log("error!");
        },
    });
}
