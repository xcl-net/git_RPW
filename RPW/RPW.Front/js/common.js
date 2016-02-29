$(function () {
    //每次页面打开，都触发，ajax请求，服务器，我是不是有用户登录了？？

    $.ajax({
        type: "post", url: "/FrontUserController.ashx",
        dataType: "json",
        data: { action: "IsLogin" },
        success: function (data) {
            if (data.Status == "yes") {
                $("#liUserName").show();
                $("#LiLogout").show();
                $("#spanUserName").text(data.Msg);
            } else if (data.Status == "no") {
                $("#LiLogin").show();
                $("#LiRegister").show();
            }
        },
        error: function () {
            //alert("获取登录状态失败");
            window.location.href = "/login.shtml";
        }
    });
});