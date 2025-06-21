function addToCart(itemId) {
    $.ajax({
        url: "/User/AddToCart",
        type: "POST",
        data: { itemId: itemId },
        success: function (response) {
            if (response.success) {
                if (typeof callSuccessToaster === 'function') {
                    callSuccessToaster("Item added to cart!");
                }
            } else {
                if (typeof callErrorToaster === 'function') {
                    callErrorToaster(response.message || "Failed to add item to cart");
                }
            }
        },
        error: function () {
            if (typeof callErrorToaster === 'function') {
                callErrorToaster("Failed to add item to cart");
            }
        }
    });
}

function toggleWishlist(event, element) {
    event.preventDefault();
    event.stopPropagation();

    const itemId = $(element).data('item-id');
    const isFavourite = $(element).hasClass('fa-solid');
    $.ajax({
        url: "/User/ToggleWishlistItem",
        type: 'POST',
        data: { itemId: itemId },
        success: function (response) {
            if (response.success) {
                if (response.isFavourite) {
                    $(element).removeClass('fa-regular').addClass('fa-solid text-danger')
                        .attr('title', 'Remove from wishlist');
                } else {
                    $(element).removeClass('fa-solid text-danger').addClass('fa-regular')
                        .attr('title', 'Add to wishlist');
                }
            }
        },
        error: function () {
            if (typeof callErrorToaster === 'function') {
                callErrorToaster('Failed to update wishlist');
            } else {
                alert('Failed to update wishlist');
            }
        }
    });
}