// Jquery Dependency

$("input[data-type='currency']").on({
    keyup: function () {
        formatCurrency($(this));
    },
    blur: function () {
        formatCurrency($(this), "blur");
    }
});


function formatNumber(n) {
    // format number 1000000 to 1,234,567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}


function formatCurrency(input, blur) {
    // appends $ to value, validates decimal side
    // and puts cursor back in right position.

    // get input value
    var inputVal = input.val();

    // don't validate empty input
    if (inputVal === "") { return; }

    // original length
    var originalLen = inputVal.length;

    // initial caret position 
    var caretPos = input.prop("selectionStart");

    // check for decimal
    if (inputVal.indexOf(".") >= 0) {

        //// get position of first decimal
        //// this prevents multiple decimals from
        //// being entered
        //var decimal_pos = input_val.indexOf(".");

        // split number by decimal point
        var leftSide = inputVal.substring(0, decimal_pos);
        //var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        leftSide = formatNumber(leftSide);

        //// validate right side
        //right_side = formatNumber(right_side);

        //// On blur make sure 2 numbers after decimal
        //if (blur === "blur") {
        //    right_side += "00";
        //}

        //// Limit decimal to only 2 digits
        //right_side = right_side.substring(0, 2);

        // join number by .
        //input_val = "$" + left_side + "." + right_side;
        inputVal = leftSide;

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        inputVal = formatNumber(inputVal);
        //input_val = "$" + input_val;

        //// final formatting
        //if (blur === "blur") {
        //    input_val += ".00";
        //}
    }

    // send updated string to input
    input.val(inputVal);

    // put caret back in the right position
    var updatedLen = inputVal.length;
    caretPos = updatedLen - originalLen + caretPos;
    input[0].setSelectionRange(caretPos, caretPos);
}


