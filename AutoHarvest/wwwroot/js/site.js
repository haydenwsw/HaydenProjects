// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

// only used for the index page also lags probably the worst function in the whole solution
function sortList(list, idx) {

    let n = list.length;

    let i, j;
    let swapped;
    let item1, item2;
    for (i = 0; i < n - 1; i++) {

        swapped = false;
        for (j = 0; j < n - i - 1; j++) {

            switch (idx) {
                // sort price low to high
                case 0:
                    item1 = Number(list[j].getElementsByClassName("price")[0].innerText);
                    item2 = Number(list[j + 1].getElementsByClassName("price")[0].innerText);
                    if (item1 > item2) {
                        list[i].parentNode.insertBefore(list[j + 1], list[j]);
                        swapped = true;
                    }
                    break;
                // sort price high to low
                case 1:
                    item1 = Number(list[j].getElementsByClassName("price")[0].innerText);
                    item2 = Number(list[j + 1].getElementsByClassName("price")[0].innerText);
                    if (item1 < item2) {
                        list[i].parentNode.insertBefore(list[j + 1], list[j]);
                        swapped = true;
                    }
                    break;
                // sort KMs low to high
                case 3:
                    item1 = Number(list[j].getElementsByClassName("KMs")[0].innerText.replace(" KM", ""));
                    item2 = Number(list[j + 1].getElementsByClassName("KMs")[0].innerText.replace(" KM", ""));
                    if (item1 > item2) {
                        list[i].parentNode.insertBefore(list[j + 1], list[j]);
                        swapped = true;
                    }
                    break;
                // sort KMs high to low
                case 4:
                    item1 = Number(list[j].getElementsByClassName("KMs")[0].innerText.replace(" KM", ""));
                    item2 = Number(list[j + 1].getElementsByClassName("KMs")[0].innerText.replace(" KM", ""));
                    if (item1 < item2) {
                        list[i].parentNode.insertBefore(list[j + 1], list[j]);
                        swapped = true;
                    }
                    break;
            }
        }

        if (swapped == false)
            break;
    }
}