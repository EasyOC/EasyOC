var controls = {
   setPage: function (page) {
      $('#id_page').val(page);
   },
   submit: function (page) {
      controls.setPage(page);
      $("#id_report").submit();
   },
   setSize: function (size) {
      $('#id_size').val(size);
      controls.submit(1);
   },
   setSort: function (sort) {
      $('#id_sort').val(sort);
   },
   lastChecked: null,
   bulkActionLength: 0
}

// https://stackoverflow.com/questions/1634748/how-can-i-delete-a-query-string-parameter-in-javascript
function removeUrlParameter(url, parameter) {
   //prefer to use l.search if you have a location/link object
   var urlparts = url.split('?');
   if (urlparts.length >= 2) {

      var prefix = encodeURIComponent(parameter) + '=';
      var pars = urlparts[1].split(/[&;]/g);

      //reverse iteration as may be destructive
      for (var i = pars.length; i-- > 0;) {
         //idiom for string.startsWith
         if (pars[i].lastIndexOf(prefix, 0) !== -1) {
            pars.splice(i, 1);
         }
      }

      url = urlparts[0] + (pars.length > 0 ? '?' + pars.join('&') : "");
      return url;
   } else {
      return url;
   }
}

function bulkAction(page, name) {
   var length = $('.bulk-action:checked').length;
   if (length > 0) {

      $.blockUI({
         message: null,
         css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff',
            baseZ: 1021
         }
      });

      var $form = $('#id_report');
      $form.attr('method', 'POST');
      $form.attr('action', server.bulkActionUrl);
      $form.append($(".AntiForge").html());
      $form.append('<input type="hidden" name="ActionName" value="' + name + '" />');
      $form.append('<input type="hidden" name="ActionCount" value="' + controls.bulkActionLength + '" />');
      controls.submit(page);
   }
}

function updateBulkActions() {
   var length = $(".bulk-action:checked").length;
   var all = length === $(".bulk-action").length;
   $(".bulk-action-link").each(function () {
      var link = $(this);
      var len = (all ? "All" : $('#select-all:checked').length > 0 ? length - 1 : length);
      controls.bulkActionLength = len;
      link.html(link.attr('rel') + ' <span class="badge badge-pill badge-dark">' + len + "</span>");
   });
}

$(document).ready(function () {

   var cleared = "_Cleared";
   var lastFilter;

   var $boxes = $('.shift-select');
   $boxes.click(function (e) {
      if (!controls.lastChecked) {
         controls.lastChecked = this;
         return;
      }

      if (e.shiftKey) {
         var start = $boxes.index(this);
         var end = $boxes.index(controls.lastChecked);

         $boxes.slice(Math.min(start, end), Math.max(start, end) + 1).prop('checked', controls.lastChecked.checked);
      }

      controls.lastChecked = this;
   });

   $('#id_report select').selectpicker({
      iconBase: "fas",
      tickIcon: "fa-check",
      liveSearch: true,
      deselectAllText: "Off",
      noneSelectedText: "All",
      noneResultsText: "Not Found",
      selectAllText: "On",
      selectedTextFormat: "count > 2",
      style: "btn-sm btn-light",
      width: "fit",
      sanitize: false
   });

   $("#id_report select").on("changed.bs.select", function (e, clickedIndex, isSelected, previousValue) {
      lastFilter = this.name;
      controls.setPage(1);
      if (!this.multiple || $(this).val().length === 0) {
         controls.submit(1);
      }
   });
   $("#id_report select").css("visibility", "visible");

   $('.search-button').bind('click', function () {
      $('#id_report').submit();
   });

   $(".form-control.date").datepicker({ dateFormat: "yy-mm-dd" });

   $('#id_report').bind('submit', function () {

      // trim white space from text input
      $('input[type="text"]').each(function () {
         this.value = $.trim(this.value);
      });

      // stop double submit
      $('#id_submit').prop('disabled', true);

      // the rest of this just cleans up the URL (bookmark)
      var page = parseInt($('#id_page').val());

      if (page <= 1) {
         $('#id_page').attr('disabled', true);
      }

      $('#id_report input').filter(function () {
         var value = $(this).val();
         return value === "*" || value === "";
      }).attr('disabled', true);

      $("#id_report select").each(function () {
         if (lastFilter !== $(this).attr("name") && lastFilter !== cleared) {
            var selected = $('option:selected', this);
            var count = selected.length;
            if (count === 0) {
               $(this).attr('disabled', true);
            } else if (count === 1) {
               var value = $(selected[0]).val();
               if (value === "" || value === "*") {
                  $(this).attr('disabled', true);
               }
            }
         }
      });

      $('#busy').show();

      // normal submit handler fires
      return true;
   });

   $('#id_clear').click(function () {

      lastFilter = cleared;

      // set single select back to all
      $('#id_report select:not([multiple])').selectpicker('val', '*');

      // set multi-select to none
      $('#id_report select[multiple]').selectpicker('deselectAll');
      $('#id_report select[multiple]').selectpicker('render');

      // set other inputs to blank
      $('#id_report input:visible').val("");

      controls.submit(server.entity.page === 0 ? 0 : 1);
   });

   $('.sortable').click(function () {
      $(this).toggleClass('btn-sort').toggleClass('btn-primary');

      $(this).siblings().each(function (i) {
         if ($(this).hasClass('btn-primary')) {
            $(this).removeClass('btn-primary').addClass('btn-sort');
         }
      });

      var sort = '';
      $('td.sorter').each(function (i) {
         var field = $(this).attr('data-order-by');
         if (field) {
            var index = 0;
            $('a', $(this)).each(function (j) {
               if ($(this).hasClass('btn-primary')) {
                  switch (index) {
                     case 0:
                        sort += field + 'a.';
                        break;
                     case 1:
                        sort += field + 'd.';
                        break;
                     default:
                        break;
                  }
               }
               index++;
            });
         }
      });
      var expression = sort.replace(/^\.+|\.+$/gm, '');
      console.log(expression);
      controls.setSort(expression);
      controls.submit(server.entity.page === 0 ? 0 : 1);
   });

   $(":checkbox[name=select-all]").click(function () {
      $(":checkbox[name=Records]").prop("checked", this.checked);
      updateBulkActions();
   });

   $(":checkbox[name=Records]").click(function () {
      updateBulkActions();
   });

   $('input[type="text"]').on("focus", function () {
      if ($(this).val() == "*") {
         $(this).select();
      }
   });

});
