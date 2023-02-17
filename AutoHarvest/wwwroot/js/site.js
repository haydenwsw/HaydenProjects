
window.onload = (event) => {

    let ul = document.getElementById("itemList")

    if (ul.childNodes.length > 1)
        sortList(ul, document.getElementById('selectSort').selectedIndex)
};

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
