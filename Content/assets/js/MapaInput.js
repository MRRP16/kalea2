let map;
var previousMarker;
function initAutocomplete() {

    let myLatlng = { lat: 14.595865824260637, lng: - 90.50606240254116 };

    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 15,
        center: myLatlng,
    });

    marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        draggable: true,
    });

    map.addListener('click', function (e) {
        previousMarker = marker;
        if (previousMarker)
            previousMarker.setMap(null);
        placeMarker(e.latLng, map);
    });

    function placeMarker(position, map) {
        previousMarker = new google.maps.Marker({
            position: position,
            map: map
        });
        marker = previousMarker;
        map.setCenter(position);
        document.getElementById("Geolocalizacion").value = position;
    }

    const infowindow = new google.maps.InfoWindow({
        content: "<p>" + marker.getPosition() + "</p>",
    });

    google.maps.event.addListener(marker, "click", () => {
        infowindow.open(map, marker);
    });

    // Create the search box and link it to the UI element.
    const input = document.getElementById("pac-input");
    const searchBox = new google.maps.places.SearchBox(input);

    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
    // Bias the SearchBox results towards current map's viewport.
    map.addListener("bounds_changed", () => {
        searchBox.setBounds(map.getBounds());
    });

    let markers = [];

    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach((marker) => {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        const bounds = new google.maps.LatLngBounds();

        places.forEach((place) => {
            if (!place.geometry || !place.geometry.location) {
                console.log("Returned place contains no geometry");
                return;
            }

            const icon = {
                url: "https://maps.gstatic.com/mapfiles/place_api/icons/v1/png_71/geocode-71.png",
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25),
            };


            marker = new google.maps.Marker({ position: place.geometry.location, map: map, draggable: true, });

            const infowindow = new google.maps.InfoWindow({
                content: "<p>" + marker.getPosition() + "</p>",
            });

            google.maps.event.addListener(marker, "click", () => {
                infowindow.open(map, marker);
            });


            document.getElementById("Geolocalizacion").value = place.geometry.location;

            // Create a marker for each place.
            //markers.push(
            //    new google.maps.Marker({
            //        map,
            //        icon,
            //        title: place.name,
            //        position: place.geometry.location,
            //    })
            //);

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);
    });

    marker.addListener("click", () => {
        document.getElementById("Geolocalizacion").value = marker.getPosition();
    });

    google.maps.event.addListener(marker, 'dragend', function (evt) {
        //console.log(evt.latLng.lat().toFixed(6));
        //console.log(evt.latLng.lng().toFixed(6));

        document.getElementById("Geolocalizacion").value = "(" + evt.latLng.lat().toFixed(6) + "," + evt.latLng.lng().toFixed(6)+")";
        //$("#txtLat").val(evt.latLng.lat().toFixed(6));
        //$("#txtLng").val(evt.latLng.lng().toFixed(6));

        map.panTo(evt.latLng);
    });
}

function GetDistancia() {
    const bounds = new google.maps.LatLngBounds();
    const markersArray = [];
    const map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 55.53, lng: 9.4 },
        zoom: 10,
    });
    // initialize services
    const geocoder = new google.maps.Geocoder();
    const service = new google.maps.DistanceMatrixService();
    // build request
    const origin1 = { lat: 55.93, lng: -3.118 };
    const origin2 = "Greenwich, England";
    const destinationA = "Stockholm, Sweden";
    const destinationB = { lat: 50.087, lng: 14.421 };
    const request = {
        origins: [origin1, origin2],
        destinations: [destinationA, destinationB],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false,
    };
}

function deleteMarkers(markersArray) {
    for (let i = 0; i < markersArray.length; i++) {
        markersArray[i].setMap(null);
    }

    markersArray = [];
}