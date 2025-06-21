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

function loadCartItems() {
    $.ajax({
        url: "/User/GetCartItems",
        type: "GET",
        success: function (data) {
            $("#cartItems").html(data);
            updateTotalAmount();
        },
        error: function () {
            console.error("Error loading cart items");
        }
    });
}

function updateTotalAmount() {
    let total = 0;
    $(".cart-item").each(function () {
        const price = parseFloat($(this).data("price"));
        const quantity = parseInt($(this).find(".quantity-input").val());
        const itemTotal = price * quantity;
        $(this).find(".item-total").text(`₹${itemTotal.toFixed(2)}`);
        total += itemTotal;
    });
    $("#totalAmount").text(`₹${total.toFixed(2)}`);
}

$(document).ready(function () {
    loadNavbar();
    loadCartItems();

    $(document).on("click", ".quantity-btn", function () {
        const $input = $(this).siblings(".quantity-input");
        let qty = parseInt($input.val());
        if (isNaN(qty)) qty = 1; // Reset to 1 if invalid
        const $card = $(this).closest(".cart-item");
        const price = parseFloat($card.data("price"));

        if ($(this).hasClass("plus")) {
            if (price > 50000 && qty >= 2) {
                if (typeof callWarningToaster === 'function') {
                    callWarningToaster("Maximum quantity limit of 2 reached for items above ₹50,000!");
                }
                return;
            } else if (price > 30000 && qty >= 5) {
                if (typeof callWarningToaster === 'function') {
                    callWarningToaster("Maximum quantity limit of 5 reached for items above ₹30,000!");
                }
                return;
            }
            qty += 1;
        } else {
            qty = qty > 1 ? qty - 1 : 1;
        }

        $input.val(qty);
        updateTotalAmount();
    });

    $(document).on("click", ".remove-btn", function () {
        const cartId = $(this).data("cart-id");
        $.ajax({
            url: "/User/RemoveFromCart",
            type: "POST",
            data: { cartId: cartId },
            success: function (data) {
                $("#cartItems").html(data);
                updateTotalAmount();
            },
            error: function () {
                if (typeof callErrorToaster === 'function') {
                    callErrorToaster("Failed to remove item");
                }
            }
        });
    });
});