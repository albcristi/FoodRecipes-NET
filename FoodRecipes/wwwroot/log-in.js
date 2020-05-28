
function logInFailed(){
    $("#invalidUserData")
        .css("display","block");
}

$(document).ready( () => {
    $("#invalidUserData")
        .css("display", "none");

    
    $("#log-in-button")
        .click(
            function () {
                let user_name = $("#user-name-input").val();
                let password = $("#usr-password-input").val();
                $.ajax(
                    {
                        url: 'https://localhost:44348/api/log-in',
                        type: 'POST',
                        data: JSON.stringify({ id: 1, user_name: user_name, password: password }),
                        dataType: 'text',
                        contentType: 'application/json; charset=utf-8',
                        statusCode: {
                            200: function (resp) {
                                sessionStorage.setItem("jwt", resp);
                                sessionStorage.setItem("user_name", user_name);
                                window.location = '../main_page/main-page.html'
                            },
                            400: function () {
                                console.log("log-in: failed");
                                logInFailed();
                            }
                        }
                    }
                )
            }
        );
});