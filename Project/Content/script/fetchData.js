const clickCheck = async () => {
    const idVideo = $('#id').val().trim()
    let result = false;

    await fetch('https://youtube.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails%2Cstatistics&id=' +
            idVideo +
            '&key=AIzaSyBQ0NJnKeEJHXvSs3vvk5jAd_Qx675PCog')
        .then(response => response.json())
        .then(data => {
            try {
                console.log(convert_time(data.items[0].contentDetails.duration))
                $('#id').val(idVideo)
                $('#image-overlay').attr("src", data.items[0].snippet.thumbnails.standard.url)
                $('#name').val(data.items[0].snippet.localized.title)
                $('#description').val(data.items[0].snippet.localized.description)
                $('#time').val(convert_time(data.items[0].contentDetails.duration))

                $("#name").attr("readonly", "readonly");
                $("#description").attr("readonly", "readonly");
                $("#time").attr("readonly", "readonly");
                result = true;
            } catch (e) {
                alert('ID not valid')
                $('#image-overlay').attr("src", "")
                $('#name').val("")
                $('#description').val("")
                $('#time').val("")
                result = false;
            }
        });
    if (result) {
        alert("Fetch data from youtube success !!!");
    }
    return result;
}

function convert_time(duration) {
    var a = duration.match(/\d+/g);

    if (duration.indexOf('M') >= 0 && duration.indexOf('H') == -1 && duration.indexOf('S') == -1) {
        a = [0, a[0], 0];
    }
    if (duration.indexOf('H') >= 0 && duration.indexOf('M') == -1) {
        a = [a[0], 0, a[1]];
    }
    if (duration.indexOf('H') >= 0 && duration.indexOf('M') == -1 && duration.indexOf('S') == -1) {
        a = [a[0], 0, 0];
    }

    duration = 0;

    if (a.length == 3) {
        duration = duration + parseInt(a[0]) * 3600;
        duration = duration + parseInt(a[1]) * 60;
        duration = duration + parseInt(a[2]);
    }

    if (a.length == 2) {
        duration = duration + parseInt(a[0]) * 60;
        duration = duration + parseInt(a[1]);
    }

    if (a.length == 1) {
        duration = duration + parseInt(a[0]);
    }
    return duration
}

const clickSubmit = () => {
    setTimeout(() => {
            $('#myFrom').submit();
        },
        1000)
}