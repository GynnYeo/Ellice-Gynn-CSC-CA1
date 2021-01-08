function detectReceipt() {



    document.getElementById("spinner").hidden = false;

    var file_data = document.getElementById('imageInput').files[0];
    var fd = new FormData();

    fd.append("file", file_data);
    fd.append("refresh", false);
    fd.append("incognito", true);
    fd.append("language", "en");
    fd.append("extractTime", true);

    console.log(fd);
    $.ajax({
        url: 'https://api.taggun.io/api/receipt/v1/simple/file',
        method: "POST",
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("apikey", "YOUR_TAGGUN_API_KEY");
           
        },
        data: fd,
        success: function(data){
            console.log(data);
            document.getElementById("spinner").hidden = true;

            var Output = "Is Receipt: "
            if (data.confidenceLevel < 0.5) {
                Output += "False </br>Conficence Level: " + data.confidenceLevel;
            } else {
                Output += "True </br>Conficence Level: " + data.confidenceLevel + "</br>Amount: $" + data.totalAmount.data +
                    "</br>Tax Amount: $" + data.taxAmount.data + "</br>Date: " + data.date.data +
                    "</br >Merchant: " + data.merchantName.data + "</br >Address: " + data.merchantAddress.data + "</br >";

            }
            document.getElementById("OutputTxt").innerHTML = Output;
        }
    });
    


}


var loadFile = function (event) {
    var image = document.getElementById('output');
    image.src = URL.createObjectURL(event.target.files[0]);

    var Output = "Is Receipt: </br>Conficence Level: </br>Amount: </br>Tax Amount: </br>Date: </br >Merchant: </br >Address: </br >";
    document.getElementById("OutputTxt").innerHTML = Output;
};