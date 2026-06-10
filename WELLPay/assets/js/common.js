var chineseTableLang = {
    "processing": "处理中...",
    "loadingRecords": "载入中...",
    "lengthMenu": "显示 _MENU_ 项结果",
    "zeroRecords": "没有符合的结果",
    "info": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
    "infoEmpty": "显示第 0 至 0 项结果，共 0 项",
    "infoFiltered": "(從 _MAX_ 项结果中过滤)",
    "infoPostFix": "",
    "search": "搜寻:",
    "paginate": {
        "first": "第一页",
        "previous": "上一页",
        "next": "下一页",
        "last": "最后一页"
    },
    "aria": {
        "sortAscending": ": 升幂划分",
        "sortDescending": ": 降幂划分"
    }
};

//#region 取得可選擇商戶列表
//isProxy:是否只顯示身分為代理的商户
//showAllCompanyBtn:是否有所有商户的選項
function layout_getCompanyTableResult(isProxy, showAllCompanyBtn) {

    api.getCompanyTableResult(function (success, obj) {

        if (obj.ResultCode == 0) {
            if (isProxy) {
                obj.CompanyResults = $.grep(obj.CompanyResults, function (data) {
                    return data.CompanyType != 1;
                });
            }
            updateSortable(obj.CompanyResults, "CompanyName", "CompanyID", "ParentCompanyID", showAllCompanyBtn);
        } else {
            var message = ""
            switch (obj.ResultCode) {
                case 5:
                    message = "没有资料";
                    break;
                default:
                    message = "其他错误";
                    break;
            }

            sweetAlert(message);
        }
    });
}
//#endregion

//#region 重整供應商查詢列表資料
function updateSortable(sortData, showName, dataId, parentId, showAllCompanyBtn) {
    $('.dd > .dd-list').remove();
    if (showAllCompanyBtn) {
        $('.dd').append('<ol class="dd-list"><li class="dd-item" data-id="' + "-99" + '" ><div class="dd-handle">' + "所有商户" + '</div></li></ol>');
    } else {
        $('.dd').append('<ol class="dd-list"></ol>');
    }


    var oldSortKey = '';
    for (var i = 0; i < sortData.length; i++) {
        var parentli = $('.dd>.dd-list li[data-id=' + sortData[i][parentId] + ']');

        var icon;
        var childcount;
        if (sortData[i]["CompanyType"] == 2) {

            icon = '<i class="material-icons" style="vertical-align:middle;font-size: 20px;">person</i>';

        } else {
            //icon = '<i class="material-icons" style="vertical-align:middle;font-size: 20px;">people</i>';
            icon = '';
        }

        if (sortData[i]["ChildCompanyCount"] != 0) {

            childcount = ' ' + '(' + sortData[i]["ChildCompanyCount"] + ")";
        } else {
            childcount = '';
        }


        if (parentli.length > 0) {
            if (parentli.find('.dd-list').length == 0) {
                parentli.append('<ol class="dd-list"></ol>');
                parentli.find('.dd-list').append('<li class="dd-item" data-id="' + sortData[i][dataId] + '" ><div class="dd-handle" style="line-height:20px">' + icon + "<span>" + sortData[i][showName] + '</span><span>' + childcount + '</span></div></li>');
            } else {
                parentli.find('.dd-list').eq(0).append('<li class="dd-item" data-id="' + sortData[i][dataId] + '" ><div class="dd-handle" style="line-height:20px">' + icon + "<span>" + sortData[i][showName] + '</span><span>' + childcount + '</span></div></li>');
            }
        } else {
            $('.dd > .dd-list').append('<li class="dd-item" data-id="' + sortData[i][dataId] + '"><div class="dd-handle" style="line-height:20px">' + icon + "<span>" + sortData[i][showName] + '</span><span>' + childcount + '</span></div></li>');
        }
    }

    $('.dd').nestable({ updateTree: true, allowDrag: false });

    BindingBtn();

}
//#endregion

//添加千分符號
function toCurrency(num) {

    num = parseFloat(Number(num).toFixed(2));
    var parts = num.toString().split('.');
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    return parts.join('.');
}

//刪除千分符號
function toNumber(num) {
    var returnNum = Number(num.trim().replace(/,/g, ""));
    return returnNum;
}
//是否為浮點數
function isFloat(n) {
    return n === n && n !== (n | 0);
}

//#region 建立供應商列表後綁定按鈕事件
function BindingBtn() {
    $('.dd-handle').on('click', function () {

        if ($(this).hasClass('selectedItem')) {
            $(this).removeClass('selectedItem');
        } else {
            $('.dd .dd-handle').removeClass('selectedItem');
            $(this).addClass('selectedItem');
        }
        event.stopPropagation();
    });

    $('#btnexpandall').on('click', function () {
        $('.dd').nestable('expandAll');
    });

    $('#btncollapseAll').on('click', function () {
        $('.dd').nestable('collapseAll');
    });

    $('#btnSaveselectcompany').on('click', function () {

        if ($('.selectedItem').length == 0) {
            sweetAlert("请先选择供应商!");
        } else {

            $('#selectCompany_menu').hide();
            docChangeCompanyLabel.attr("data-id", $('.selectedItem').closest("li").attr("data-id"));
            if ($('.selectedItem').closest("li").attr("data-id") == "-99") {
                docChangeCompanyLabel.text("所有商户");
            } else {
                docChangeCompanyLabel.text($('.selectedItem').find('span').eq(0).text());
            }

            if (typeof btnSaveselectcompanyCallback != "undefined") {
                btnSaveselectcompanyCallback();
            }
        }
    });

    $('#btnCancelselectcompany').on('click', function () {
        $('#selectCompany_menu').hide();
    });

    //搜尋 搜尋商户
    $("#input_CompanySerach").on("blur", function () {
        var searchContent = $(this).val();
        $(".divCompanyTree>ol>.d-none").removeClass("d-none");
        $(".divCompanyTree>ol>.OK").removeClass("OK");
        if (searchContent != "") {
            $(".divCompanyTree").find("span").each(function (i, v) {
                var itemContent = $(v).text();
                if ((itemContent.toLocaleUpperCase().includes(searchContent.toLocaleUpperCase()))) {
                    $(v).parents(".divCompanyTree>ol>li").addClass("OK");
                    $(v).parents(".divCompanyTree>ol>li").removeClass("d-none");
                } else {
                    if (!$(v).parents(".divCompanyTree>ol>li").hasClass("OK")) {
                        $(v).parents(".divCompanyTree>ol>li").addClass("d-none");
                    }
                }
            });
        }
    })

    $('.dd').nestable('collapseAll');
}
//#endregion

