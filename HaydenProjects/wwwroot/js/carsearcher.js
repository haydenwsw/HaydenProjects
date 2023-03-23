
// triggers when the document is ready
$("document").ready(function () {

    // make input
    $("#selectMake").on("input", function () {
        searchDropdown(this.value, $("#selectMakeDropdown .droplist-item"));
        makeText = $("#selectMake").val();
        $("#selectMake").dropdown("show");
    });

    // make dropdown item click
    $("#selectMakeDropdown .droplist-item").click(function () {
        $("#selectMakeDropdown .droplist-item").show();
        $("#selectModel").val("All Models");
        $("#selectModelDropdown .droplist-item").show();
    });

    // make dropdown hover
    $("#selectMakeDropdown .droplist-item").hover(function () {
        $("#selectMake").val(this.innerText);
        makeText = this.innerText;
    });

    // make v button
    $("#makeDropdownBtn").click(function (e) {
        dropDown(e, $("#selectMake"));
    });

    // make click or focus
    $("#selectMake").on("click focus", function () {
        $("#selectMake").attr("placeholder", makeText);
        $("#selectMakeDropdown .droplist-item").show();
        $("#selectMake").val("");
        $("#SearchTerm").val("");
    });

    // make blur
    $("#selectMake").blur(function () {
        $("#selectMake").val(makeText)
    });

    // model input
    $("#selectModel").on("input", function () {
        searchDropdown(this.value, $("#selectModelDropdown .droplist-item"));
    });

    // model v button
    $("#modelDropdownBtn").click(function (e) {
        dropDown(e, $("#selectModel"));
    });

    // model focus
    $("#selectModel").on("click focus", function () {
        getModels();
    });

    // model blur
    $("#selectModel").blur(function () {
        $("#selectModel").val(modelText)
    });

    // search x button
    $("#searchClearBtn").click(function () {

        $("#SearchTerm").val("");
        $("#SearchTerm").focus();
    });

    // search on input
    $("#SearchTerm").on("input", function () {
        $("#selectMake").val("");
        $("#selectModel").val("");
    });

    // stop anything but numbers getting inputted into the min and max inputs
    $("#minPrice, #maxPrice").keydown(function (e) {
        return (!(e.keyCode >= 65) && e.keyCode != 32);
    });

    // min price input
    $("#minPrice").on("input", function () {
        searchDropdown(this.value, $("#minPriceDropdown .droplist-item"));
    });

    // min price dropdown item click
    $("#minPriceDropdown .droplist-item").click(function () {
        $("#minPrice").val(this.innerText);
    });

    // min price v button
    $("#minPriceDropdownBtn").click(function (e) {
        dropDown(e, $("#minPrice"));
    });

    // min price focus
    $("#minPrice").focus(function () {
        $("#minPrice").val("");
    });

    // max price input
    $("#maxPrice").on("input", function () {
        searchDropdown(this.value, $("#maxPriceDropdown .droplist-item"));
    });

    // max price dropdown item click
    $("#maxPriceDropdown .droplist-item").click(function () {
        $("#maxPrice").val(this.innerText);
    });

    // max price v button
    $("#maxPriceDropdownBtn").click(function (e) {
        dropDown(e, $("#maxPrice"));
    });

    // max price focus
    $("#maxPrice").focus(function () {
        $("#maxPrice").val("");
    });
});

var makeText = "All Makes";
var modelText = "All Models";

// cache all the downloaded make models
const dict = { "All Makes": null };

// event for the selection box car makes
function getModels() {

    $("#selectModelDropdown .droplist-item").show();
    $("#selectModel").attr("placeholder", modelText);
    $("#selectModel").val("");
    $("#SearchTerm").val("");

    // clears the model selection box
    clearModelDropdown();
    
    if (selectMake.value == "")
        return;

    // check of makemodel is in cache
    if (dict[selectMake.value] === undefined) {

        // make post request to get the models
        $.ajax({
            url: "/api/CarSearcherApi?make=" + selectMake.value,
            type: "POST",
            success: function (responseArray) {

                if (responseArray.constructor.name != "Array")
                    return;

                // add every model to the model selection box
                addModelDropdown(responseArray);

                // cache the data
                dict[selectMake.value] = responseArray;
            },
            error: function () {
                console.log("error!");
            },
        });
    }
    else {

        modelArray = dict[selectMake.value];

        if (modelArray !== null) {

            // add every model to the model selection box
            addModelDropdown(modelArray);
        }
    }
}

// clears selection boxes except the first element
function clearModelDropdown() {

    let items = $("#selectModelDropdown .droplist-item");
    for (i = 1; i < items.length; i++) {
        items[i].remove();
    }
}

// adds an array of strings to the model dropdown
function addModelDropdown(array) {

    for (let i = 0; i < array.length; i++) {
        let opt = allModels.cloneNode(true);
        opt.innerHTML = array[i];

        // then append it to the select element
        selectModelDropdown.appendChild(opt);
    }
}

// clears make model inputs
function clearMakeModel() {

    selectMake.selectedIndex = 0;
    clearModelDropdown();
}

// 'v' button event for dropping the dropdown
function dropDown(e, $select) {

    e.stopPropagation();
    $select.focus();
    $select.dropdown("toggle");
}

// simple search for the dropdown item
function searchDropdown(text, $items) {

    text = text.toLowerCase();

    if (text == "") {
        $items.show();
    }
    else {
        for (let i = 0; i < $items.length; i++) {

            if ($items[i].innerText.toLowerCase().startsWith(text))
                $items[i].style.display = "block";
            else
                $items[i].style.display = "none";
        }
    }
}

// sorts the list
function sortList(idx) {

    var new_ul = itemList.cloneNode(false);
    var lis = [];

    for (let i = itemList.childNodes.length; i--;) {
        if (itemList.childNodes[i].nodeName === "LI")
            lis.push(itemList.childNodes[i]);
    }

    switch (idx) {

        // sort price low to high
        case 0:
            lis.sort(function (a, b) {
                return parseInt(a.getElementsByClassName("price")[0].innerText, 10) - parseInt(b.getElementsByClassName("price")[0].innerText, 10);
            });
            break;

        // sort price high to low
        case 1:
            lis.sort(function (a, b) {
                return parseInt(b.getElementsByClassName("price")[0].innerText, 10) - parseInt(a.getElementsByClassName("price")[0].innerText, 10);
            });
            break;

        // sort KMs low to high
        case 3:
            lis.sort(function (a, b) {
                return parseInt(a.getElementsByClassName("KMs")[0].innerText.replace(" KM", ""), 10) - parseInt(b.getElementsByClassName("KMs")[0].innerText.replace(" KM", ""), 10);
            });
            break;

        // sort KMs high to low
        case 4:
            lis.sort(function (a, b) {
                return parseInt(b.getElementsByClassName("KMs")[0].innerText.replace(" KM", ""), 10) - parseInt(a.getElementsByClassName("KMs")[0].innerText.replace(" KM", ""), 10);
            });
            break;
    }

    for (let i = 0; i < lis.length; i++)
        new_ul.appendChild(lis[i]);
    itemList.parentNode.replaceChild(new_ul, itemList);
}
