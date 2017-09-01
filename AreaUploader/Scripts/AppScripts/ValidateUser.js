$(function () {
    $("#btnAddUser").click(function () {
        var uName = $("#txtUserRegname").val();
        var email = $("#txtEmail").val();
        var pwd = $("#txtAddPassword").val();

        var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i;
        //var passwordPattern = (/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{6,20}$/);

        if (uName == "" && pwd == "" &&  email == "") {
            $("#validateMsg").empty().html("All the fields are mandatory");
            $("#validateMsg").show();
            return false;
        }
        else {
            if (uName == "") {
                $("#txtUserRegname").focus();
                $("#validateMsg").empty().html("Username cannot be empty");
                $("#validateMsg").show();
                return false;
            }
           
            if (email == "") {
                $("#txtEmail").focus();
                $("#validateMsg").empty().html("Email cannot be empty");
                $("#validateMsg").show();
                return (false);
            }
            if (!emailPattern.test(email)) {
                $("#txtEmail").focus();
                $("#validateMsg").empty().html("Please enter a valid email address");
                $("#validateMsg").show();
                return (false);
            }
            else if (uName != ""  && email != "") {
                $.ajax({
                    url: '../Access/CheckUserExist',
                    data: { "userName": uName, "email": email, "userId": 0 },
                    success: function (response) {
                        if (response.response == "Username Found") {
                            $("#txtUsername").focus();
                            $("#validateMsg").empty().html("Username already exist");
                            $("#validateMsg").show();
                            return false;
                        }
                       
                        else if (response.response == "Email Found") {
                            $("#txtEmail").focus();
                            $("#validateMsg").empty().html("Email already exist");
                            $("#validateMsg").show();
                            return false;
                        }
                        else {
                            if (pwd == "") {
                                $("#txtPassword").focus();
                                $("#validateMsg").empty().html("Password cannot be empty");
                                $("#validateMsg").show();
                                return false;
                            }
                            if (pwd.length < 8) {
                                alert(pwd.length);
                                $("#txtAddPassword").focus();
                                $("#validateMsg").empty().html("Minimum 8 Characters required for Password");
                                $("#validateMsg").show();
                                return false;
                            }
                            $("#btnAddUser").prop("type", "submit");
                            $("#addUserform").submit();
                            return false;
                        }
                    }
                });
            }
        }
    })
})


//$('#txtUsername,#txtAccountname,#txtPassword').bind('keypress', function (e) {
//    var keyCode = e.keyCode || e.charCode || e.which;
//    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 122) && (keyCode < 48 || keyCode > 57) && (event.keyCode != 8 || event.keyCode != 46
//|| event.keyCode != 37 || event.keyCode != 39) && keyCode != 8 && keyCode != 32) {
//        return false;
//    }
//});