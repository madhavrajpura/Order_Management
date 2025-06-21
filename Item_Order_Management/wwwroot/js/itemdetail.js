function loadNavbar() {
    $.ajax({
        url: "/User/GetUserNavbarData",
        type: "GET",
        success: function (data) {
            $("#Navbar").html(data);
        },
        error: function () {
            console.error("Error loading navbar");
        }
    });
}

$(document).ready(function () {
    loadNavbar();

    $(document).on('click', '.fa-heart', function (e) {
        toggleWishlist(e, this);
    });
});