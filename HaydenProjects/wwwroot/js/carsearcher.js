
// cache all the downloaded makemodels
const dict = { "All Makes": [ "All Models" ] };

// event for the selection box car makes
function getModels() {

    clearSearch();

    // check of makemodel is in cache
    if (dict[selectMake.value] === undefined) {

        // make post request to get the models
        $.ajax({
            url: '/api/CarSearcherApi?make=' + selectMake.value,
            type: 'POST',
            success: function (responseArray) {

                // clear the selection box
                clearSelectBox(selectModel, 0);

                // add every model to the model selection box
                for (let i = 0; i < responseArray.length; i++) {
                    var opt = document.createElement("option");
                    opt.value = responseArray[i];
                    opt.innerHTML = responseArray[i];

                    // then append it to the select element
                    selectModel.appendChild(opt);
                }

                // cache the data
                dict[selectMake.value] = responseArray;
            },
            error: function () {
                console.log('error!');
            },
        });
    }
    else {

        // clear the selection box
        clearSelectBox(selectModel, 0);

        modelArray = dict[selectMake.value];

        if (modelArray !== null) {

            // add every model to the model selection box
            for (let i = 0; i < modelArray.length; i++) {
                var opt = document.createElement("option");
                opt.value = modelArray[i];
                opt.innerHTML = modelArray[i];

                // then append it to the select element
                selectModel.appendChild(opt);
            }
        }
    }
}

// clears selection boxs exspet the first element
function clearSelectBox(selectBox, n) {
    let length = selectBox.options.length;
    for (i = length - 1; i >= n; i--) {
        selectBox.options[i] = null;
    }
}

function clearMakeModel() {

    selectMake.selectedIndex = 0;
    clearSelectBox(selectModel, 1);
}

function clearSearch() {
    SearchTerm.value = "";
}

// sorts the list
function sortList(ul, idx) {

    var new_ul = ul.cloneNode(false);
    var lis = [];

    for (var i = ul.childNodes.length; i--;) {
        if (ul.childNodes[i].nodeName === 'LI')
            lis.push(ul.childNodes[i]);
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

    for (var i = 0; i < lis.length; i++)
        new_ul.appendChild(lis[i]);
    ul.parentNode.replaceChild(new_ul, ul);
}
