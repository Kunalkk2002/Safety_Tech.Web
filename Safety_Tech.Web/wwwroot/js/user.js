// Wait for the DOM to be fully loaded
$(document).ready(function () {

    // 🔐 User handler: Handles click event for user list
    $("#userList").click(function (e) {
        e.preventDefault();
        GetAllUsers(); // Fetch and display all users
    });

    // 🧼 Clean modal before showing: Handles edit button click
    $(document).on("click", ".btn-warning", function (e) {
        e.preventDefault();

        // Clear all input values and validation errors in the edit user form
        $("#editUserForm input, #editUserForm select").val("").removeClass("is-invalid");

        // Open the edit user modal
        $("#editUserModal").modal("show");

        // Activate the first tab in the modal
        $(".nav-tabs .nav-link").removeClass("active");
        $(".nav-tabs .nav-link:first").addClass("active");
        $(".tab-pane").removeClass("show active");
        $(".tab-pane:first").addClass("show active");
    });

    // ❗ Remove error class when user starts typing or selects again
    $(document).on("input change", "#editUserForm :input[required]", function () {
        if ($(this).val()) {
            $(this).removeClass("is-invalid");
        }
    });

   //  ✅ Validation helper: Validates required fields in a tab
    function validateTab(tabId) {
        let isValid = true;
        $(`${tabId} :input[required]`).each(function () {
            if (!$(this).val()) {
                $(this).addClass("is-invalid");
                if (isValid) $(this).focus(); // Focus first invalid field
                isValid = false;
            } else {
                $(this).removeClass("is-invalid");
            }
        });
        return isValid;
    }

    // 👉 Next button tab movement: Moves to the next tab if current tab is valid
    $(".next-tab").click(function () {
        let $active = $(".nav-tabs .nav-link.active");
        let nextTab = $active.parent().next().find(".nav-link");

        if (validateTab($active.attr("href"))) {
            $active.removeClass("active");
            $(nextTab).tab("show");
        }
    });

    // 👈 Prev button tab movement: Moves to the previous tab
    $(".prev-tab").click(function () {
        let $active = $(".nav-tabs .nav-link.active");
        let prevTab = $active.parent().prev().find(".nav-link");

        $active.removeClass("active");
        $(prevTab).tab("show");
    });

    // ✅ Final Submit: Validate ALL Tabs before submitting the form
    $("#editUserForm").submit(function (e) {
        e.preventDefault();

        // Validate all tabs before submission
        const isTab1Valid = validateTab("#tab1");
        const isTab2Valid = validateTab("#tab2");
        const isTab3Valid = validateTab("#tab3");

        if (!(isTab1Valid && isTab2Valid && isTab3Valid)) {
            return;
        }

        // Gather user data from form fields
        const userData = {
            username: $("#username").val(),
            usermail: $("#usermail").val(),
            password: $("#userpassword").val(),
            role: $("#userrole").val(),
            address: $("#useraddress").val()
        };

        console.log("Updating user:", userData);

        // TODO: Replace with real AJAX call to update user
        alert("User updated successfully!");
        $("#editUserModal").modal("hide");
    });

    // Handle delete button click: Fetch user by ID for deletion
    $(document).on("click", ".btn-danger", function (e) {
        e.preventDefault();
        var id = $(this).closest("form").find("input[name='id']").val();
        GetUserById(id);
    });
});

function GetUserById(id) {
    const token = localStorage.getItem("access_token");

    $.ajax({
        url: "/user/GetUserById?id=" + id,
        type: "GET",
        contentType: "application/json",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (data) {
            if (data.roleName === 'SuperAdmin') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Access Denied',
                    text: 'Super Admin cannot be deleted.',
                    confirmButtonText: 'OK'
                });
            } else if (data.roleName === 'Admin') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Are you sure?',
                    text: 'Do you really want to delete this Admin?',
                    showCancelButton: true,
                    confirmButtonText: 'Yes, Delete',
                    cancelButtonText: 'No',
                }).then((result) => {
                    if (result.isConfirmed) {
                        deleteUser(id, token, 'Admin');
                    }
                });
            } else {
                // Directly delete User (no confirmation)
                deleteUser(id, token, 'User');
            }
            GetAllUsers();
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                alert("Unauthorized. Please log in.");
            } else {
                alert("Error fetching user information.");
            }
        }
    });
}

function deleteUser(id, token, role) {
    $.ajax({
        url: "/user/DeleteUser?id=" + id,
        type: "GET",
        contentType: "application/json",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function () {
            Swal.fire({
                icon: 'success',
                title: 'Deleted',
                text: `${role} deleted successfully.`,
                confirmButtonText: 'OK'
            });
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                alert("Unauthorized. Please log in.");
            } else {
                alert("Error deleting user.");
            }
        }
    });
}

function GetAllUsers() {
    const token = localStorage.getItem("access_token");

    $.ajax({
        url: "/user/GetAllUser",
        type: "GET",
        contentType: "text/html",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (html) {
            // Replace target div content with the returned view
            $("#targetContainer").html(html);
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                alert("Unauthorized. Please log in.");
            } else {
                alert("Error loading user list.");
            }
        }
    });
}

