function rpajax(url,data,success) {
    $.ajax({
        type:"post",
        dataType:"json",
        url:url,
        data:data,
        success:success,
        error: function () {
            alert("请求出错！")
        }
    });
}