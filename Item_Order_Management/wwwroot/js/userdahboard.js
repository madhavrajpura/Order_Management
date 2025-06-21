let itemsCurrentPage = 1;
let itemsPageSize = $("#itemsPerPage").val();
let itemsSearchText = "";
let itemsSortColumn = "ID";
let itemsSortDirection = "asc";
let itemsIsLoading = false;
let itemsHasMore = true;

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

function loadItemsForTab(append = true) {
    if (itemsIsLoading || !itemsHasMore) return;
    itemsIsLoading = true;
    $("#loading").removeClass("d-none");

    $.ajax({
        url: "/User/GetItemList",
        type: "GET",
        data: { pageNumber: itemsCurrentPage, search: itemsSearchText, pageSize: itemsPageSize, sortColumn: itemsSortColumn, sortDirection: itemsSortDirection },
        success: function (data) {
            if (append) {
                $("#itemList").append(data);
            } else {
                $("#itemList").html(data);
            }

            const totalRecords = parseInt($("#totalRecords").val()) || 0;
            if (itemsCurrentPage * itemsPageSize >= totalRecords) {
                itemsHasMore = false;
                $("#noMoreItems").removeClass("d-none");
            }
            itemsCurrentPage++;
            itemsIsLoading = false;
            $("#loading").addClass("d-none");
        },
        error: function () {
            itemsIsLoading = false;
            $("#loading").addClass("d-none");
            console.error("Error loading items");
        }
    });
}

$(document).ready(function () {
    loadNavbar();
    loadItemsForTab(false);

    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100 && $("#items-content").hasClass("show")) {
            loadItemsForTab();
        }
    });

    $("#searchInput").on("keyup", function () {
        itemsSearchText = $(this).val().trim();
        itemsCurrentPage = 1;
        itemsHasMore = true;
        $("#noMoreItems").addClass("d-none");
        loadItemsForTab(false);
    });

    $("#itemsPerPage").on("change", function () {
        itemsPageSize = $(this).val();
        itemsCurrentPage = 1;
        itemsHasMore = true;
        $("#noMoreItems").addClass("d-none");
        loadItemsForTab(false);
    });

    $(document).on('click', '.fa-heart', function (e) {
        toggleWishlist(e, this);
    });
});