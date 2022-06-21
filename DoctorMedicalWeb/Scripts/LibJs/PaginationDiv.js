$(document).ready(function () {
    //Pagination
    pageSize = 1;

    showPage = function (page) {
        $('.line-content').hide();

        $('.line-content:gt(' + ((page - 1) * pageSize) + '):lt(' + (page) * (pageSize - 1) + ')').show();
        $('.line-content:eq(' + ((page - 1) * pageSize) + ')').show();
    }
    //:eq(index)	$("ul li:eq(3)")	The fourth element in a list (index starts at 0)
    //:gt(no)	$("ul li:gt(3)")	List elements with an index greater than 3
    //:lt(no)	$("ul li:lt(3)")	List elements with an index less than 3

    var pgs = Math.ceil($('.line-content').length / pageSize);
    //var pgs = Math.ceil($('.line-content').length / pageSize);
    var pgnt = '';
    for (var i = 1; i <= pgs; i++) {
        pgnt += '<li><a href="#">' + i + '</a></li>';
    }
    $('#pagin').html(pgnt);
    $("#pagin li a").click(function () {

        $("#pagin li a").removeClass("current");
        $(this).addClass("current");
        showPage(parseInt($(this).text()))
    });

    showPage(1);
})
