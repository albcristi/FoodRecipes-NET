
function redirectFirstPage() {
    alert("Something went wrong..");
    doLogOut();
}


//sends the token
function sendSessionToken(xhr) {
    xhr.setRequestHeader("Authorization", sessionStorage.getItem("jwt"));
}

// does logout 
function doLogOut() {
    $.ajax({
        url: `https://localhost:44348/api/log-out/${sessionStorage.getItem("user_name")}`,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        statusCode: {
            200: function succ() {
                try {
                    sessionStorage.removeItem("jwt");
                    sessionStorage.removeItem("user_name");
                    window.location = '../index.html';
                }
                catch (e) {
                    window.location = '../index.html';
                }
            },
            400: function fail() {
                window.location = '../index.html';
            }
        }

    });
   
}

// retrieve all recipes and apply a callback function
function getRecipes(callBackFct = (resp) => console.log(typeof resp)) {
    $.ajax({
        url: `https://localhost:44348/api/recipe`,
        type: 'GET',
        dataType: 'text',
        beforeSend: sendSessionToken,
        contentType: 'application/json; charset=utf-8',
        statusCode: {
            200: function suc(res) {
                callBackFct(JSON.parse(res));
            },
            400: function fail() {
                redirectFirstPage();
            }
        }
    })
}


// create recipe div
function createRecipeDiv(recipeJSON) {
    let recDiv = document.createElement("div");
    recDiv.setAttribute("id", recipeJSON.id);
    let htmlCont = `<h2> Recipe ${recipeJSON.name}</h2><br/><br/>` +
        `<h3>Description</h3><br/>` +
        `<p>${recipeJSON.description}</p><br/><br/>` +
        `<h3>Steps</h3><br/>` +
        `<p>${recipeJSON.steps}</p><br/><br/>` +
        `<h3>Type</h3><br/><p>${recipeJSON.type}</p><br/><br/>` +
        `<h3>Created By ${recipeJSON.chef_name}</h3><br/><br/>` +
        `<div class=buttons>`+
        `<button id=d${recipeJSON.id}>Remove</button>` +
        `<button id=u${recipeJSON.id}>Update</button></div>`
  
    recDiv.innerHTML = htmlCont;
    document.getElementById("rec-wrap").appendChild(recDiv);
    $(`#d${recipeJSON.id}`)
        .click(removeRecipe);
    $(`#u${recipeJSON.id}`)
        .click(updateRecipe);
}

function callBackGetAllRecs(recipes) {
    $("#rec-wrap").remove();
    let div = document.createElement("div");
    div.setAttribute("id", "rec-wrap");
    document.getElementById("recipesListContainer").appendChild(div);
    recipes.forEach(rec => createRecipeDiv(rec));
}

// search by recipes by type
function retrieveByRecipesByType(recType, callBackFunction = (resp) => { console.log(res) }) {
    $.ajax({
        url: `https://localhost:44348/api/recipe/${recType}`,
        type: 'GET',
        beforeSend: sendSessionToken,
        contentType: 'application/json; charset=utf-8',
        statusCode: {
            200: function suc(res) {
                console.log(res);
                if (res.length === 0) {
                    alert("No results!");
                    return;

                }
                callBackFunction(res);
            },
            400: function fail() {
                redirectFirstPage();
            }
        }
    }); 
}

// Handler for click 'searchButton'
function doSearchClicked() {
    if (window.prevSearch !== null && window.prevSearch !== undefined) {
        $("#lst-searched").text("Previous search: '"+window.prevSearch+"'");
    }

    window.prevSearch = $("#search-area").val();
    let val = window.prevSearch;
    if (val === undefined || val === "") {
        getRecipes(callBackGetAllRecs);
        return;
    }
    retrieveByRecipesByType(val,callBackGetAllRecs);
}


// Handler for click on: 'addRecipe'
function addRecipe(){
    let recName = $("#getNameAdd").val();
    let description = $("#getDescrAdd").val();
    let steps = $("#getStepsAdd").val();
    let typeRec = $("#getTypeAdd").val();

    if (typeRec === undefined
        || recName === undefined || description === undefined || steps === undefined) {
        alert("Fill in all fields");
        return;
    }

    $.ajax({
        url: `https://localhost:44348/api/recipe`,
        type: "PUT",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            id: 0,
            chef_name: sessionStorage.getItem("user_name"),
            name: recName,
            description: description,
            typeRec: typeRec,
            steps: steps
        }),
        beforeSend: sendSessionToken,
        statusCode: {
            200: function (resp) {
                console.log(resp);
                getRecipes(callBackGetAllRecs);
                window.location.href = "#search-bar-cont";
                $("#addContainer").css("display", "none");
            },
            400: function () {
                redirectFirstPage();
            }
        }
    });
}

//Handler for remove button
function removeRecipe() {
    let recId = this.id.substring(1);
    console.log(recId);
    $.ajax({
        url: `https://localhost:44348/api/recipe/${recId}`,
        type: 'DELETE',
        beforeSend: sendSessionToken,
        statusCode: {
            200: function (res) {
                console.log(res);
                if (res === true) {
                    alert("Recipe has been removed!");
                }
                else {
                    aler("Failed to remove recipe!");
                }
                getRecipes(callBackGetAllRecs);
            },
            400: function () {
                redirectFirstPage();
            }
        }

    });

}

//Handler for update recipe button <-- redirects to the update part
function updateRecipe() {
    window.update_id = this.id.substring(1);
    $("#updateContainer").css("display", "block");
    window.location.href = "#updateContainer";
}

// DOES ACTUAL UPDATE
function doUpdate() {
    let description = $("#getDescrUpdate").val();
    let steps = $("#getStepsUpdate").val();
    let typeRec = $("#getTypeUpdate").val();
    if (description === undefined || steps === undefined || typeRec === undefined) {
        alert("Fill in all fields!");
        return;
    }

    $.ajax({
        url: `https://localhost:44348/api/recipe/${window.update_id}`,
        type: 'PUT',
        data: JSON.stringify({
            id: parseInt(window.update_id),
            description: description,
            chef_name: window.sessionStorage.getItem("user_name"),
            name: "not used",
            steps: steps,
            typeRec: typeRec
        }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        beforeSend: sendSessionToken,
        statusCode: {
            200: function (res) {
                console.log(res);
                $("#updateContainer").css("display", "none");
                window.location.href = "#searchButton";
                getRecipes(callBackGetAllRecs);
                if (res)
                    alert("Recipe has been updated!");
                else
                    alert("Something went wrong");
            },
            400: function () {
                redirectFirstPage();
            }
        }
    })
}

// Handler to go to Add Part
function goToAddSection() {
    $("#addContainer").css("display", "block");
    window.location.href = "#addContainer";
}


$(document).ready(() => {
    var user_name = sessionStorage.getItem("user_name");
    var prevSearch = null;
    var update_id = null;
    
    // TODO: CHECK USER SESSION
    

    $("#updateContainer").css("display", "none");
    $("#addContainer").css("display", "none");
    //set the user name for the upper bar (left of log out button)
    $("#userNameUpBar").text(user_name);

    // log out clicked
    $("#l-cont-2").click(function () { doLogOut(); });

    $("#goToAdd").click(goToAddSection);

    getRecipes(callBackGetAllRecs);

    $("#searchButton").click(doSearchClicked);

    $("#addButton").click(addRecipe);

    $("#doUpdate").click(doUpdate);

});