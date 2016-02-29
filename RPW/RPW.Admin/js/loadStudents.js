var loadStudents = function () {

    //加载班级成员
    var classId = $("#ClassId").val();
    $.ajax({
        type: "post", url: "/ClassesMgr/ClassesConroller.ashx",
        dataType: "json",
        data: { action: "LoadMembers", classId: classId },
        success: function (data) {
            if (data.Status == "ok") {
                var members = data.Data;
                $("#listMembersDiv").append("<ul id='ul_2' >");
                for (var i = 0; i < members.length ; i++) {
                    var member = members[i];
                    var li = "<li>" + member.Name + "</li>";
                    $("#ul_2").append(li);//把li标签添加到 ul_2 中
                }
                $("#listResultDiv").append("</ul>");


                //给动态加载的数据列表 li 添加事件

                $("#listMembersDiv").find('li').mouseover(function () {
                    $(this).css('background', 'blue');
                }).mouseout(function () {
                    $(this).css('background', '');
                }).click(function () {  //li的单击事件
                    //删除学生的ajax请求

                    //读取li标签的值
                    var name = $(this).text();
                    //执行删除ajax的请求
                    $.ajax({
                        type: "post", url: "/ClassesMgr/ClassesConroller.ashx",
                        dataType: "json",
                        data: { action: "DeleteFromClass", name: name},
                        success: function (data) {
                            if (data.Status == "ok") {

                                // 1.界面显示，添加到了班级成员列表
                                // 2.显示之前，将旧的数据节点，首先清除
                                deleteChildNode();
                                //通过刷新班级列表函数执行
                                loadStudents();   //----单击了，就执行了查询，不单机这里也不会出现递归反复的调用自己
                            }
                        },
                        error: function () {
                            alert("删除学生请求失败");
                            return;  //如果添加错误，就不再执行
                        }

                    });

                });
            } else if (data.Status == "error") {
                alert("加载失败：" + data.Msg);
            }
        },
        error: function () {
            alert("加载班级成员列表请求失败");
        }
    })
};




var deleteChildNode = function removeAllChild() {
    var div = document.getElementById("listMembersDiv");
        while (div.hasChildNodes()) //当div下还存在子节点时 循环继续
        {
            div.removeChild(div.firstChild);
        }
    }
