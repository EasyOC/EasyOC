$(document).ready(function () {

   bsCustomFileInput.init();

   window.Parsley.on('field:error', function () {
      this.$element.closest('div.form-group').addClass("is-invalid").removeClass("is-valid");
      this.$element.addClass("is-invalid").removeClass("is-valid");
   });

   window.Parsley.on('field:success', function () {
      this.$element.closest('div.form-group').removeClass("is-invalid").addClass("is-valid").find('.help-block').empty();
      this.$element.removeClass("is-invalid").addClass("is-valid");
   });

   window.Parsley.addValidator('string', {
      requirementType: 'date',
      validateString: function (value, requirement) {
         return moment(value).isValid();
      },
      messages: {
         en: 'This value should be a date'
      }
   });

   $(document).bind("ajaxSend", function () {
      block();
   }).bind("ajaxComplete", function () {
      $("#busy").hide();
      $.unblockUI();
   });

   $('#id_validate_button').bind('click', function () {
      post();
   });

   function bind(html) {

      $("#id_content").html(html);

      var actionUrl = "";
      if (settings.controller === "Form") {
         actionUrl = settings.updateUrl;
      } else {
         // this is a task or bulk action
         actionUrl = $('#id_submit').val() === "Run" ? settings.runUrl : settings.updateUrl;
      }

      $("#id_form").attr("action", actionUrl);

      $("#id_form").parsley({
         errorsContainer: function (e) {
            return e.$element.closest('div.form-group').find('span.help-container');
         },
         errorsWrapper: '<span class="help-container"></span>',
         errorTemplate: '<span class="help-block"></span> ',
         excluded: 'input[type=button], input[type=submit], input[type=reset], input[type=file]',
         inputs: 'input, textarea, select, input[type=hidden]'
      });

      if (settings.location.enabled) {
         getLocation();
      }
      setFocus();

      // if it is post back, then validate field and post back
      $("select[data-tfl-post-back='true'],input[data-tfl-post-back='true'],textarea[data-tfl-post-back='true']").change(function () {
         if ($(this).parsley().isValid()) {
            post();
         } else {
            $(this).parsley().validate();
         }
      });

      // if it's not post back, just validate the field
      $("select[data-tfl-post-back='false'],input[data-tfl-post-back='false'],textarea[data-tfl-post-back='false']").change(function () {
         $(this).parsley().validate();
      });

      // block automatic form submit on enter
      $("select,input:not(input[type='submit'])").keydown(function (e) {
         var code = e.keyCode || e.which;
         if (code === 13) {
            e.preventDefault();
            // move to next field
            var inputs = $(this).closest('form').find(':input');
            inputs.eq(inputs.index(this) + 1).focus();
            return false;
         }
         return true;
      });

      // track focus for back-end
      // file input focus needs help since jquery file upload hides it
      $("select,input,textarea").focusin(function () {
         $("#id_focus").val($(this).attr("name"));
         if ($(this).attr('type') === 'file') {
            $(this).parent().addClass('focus');
         }
         console.log($(this).attr("name") + " has focus");
      });

      // file input needs help since jquery file upload hides it
      $("input").focusout(function () {
         if ($(this).attr('type') === 'file') {
            $(this).parent().removeClass('focus');
         }
      });

      $("#id_form").submit(function (e) {
         block();
      });

      $("#id_location_button").click(function () {
         getLocation();
      });

      $('#id_form').areYouSure();

      // if a post has occured, we are dirty
      setTimeout(function () {
         if ($('#id_method').val() !== "GET") {
            $('#id_form').addClass("dirty");
         }
      }, 1000);
   }

   function post() {
      $.ajax({
         url: settings.ajaxUrl,
         type: "POST",
         data: $("#id_form").serialize(),
         success: function (html) {
            bind(html);
         },
         error: function (html) {
            bind(html);
         }
      }, "html");
   }

   function setFocus() {

      var name = $('#id_focus').val();
      console.log('setting focus to ' + name);
      var $target = $('#id_' + name);

      if ($target.length > 0) {
         $target.focus().select();
      } else {
         $("input[name='" + name + "']:checked").focus();
      }

      // ios doesn't refresh dropdowns when ajax re-populates
      if (navigator.userAgent.match(/(ip(hone|od|ad))/i)) {
         if ($target.is("select") && $target.closest("div").prev().has("select").length > 0) {
            $target.blur();
         }
      }
   }

   function getLocation() {

      var button = $('#id_location_button');
      var display = $('#id_location_accuracy');
      var busy = button.find('div.spinner-border');
      var arrow = button.find('span.fa-location-arrow');

      var red = function () { button.removeClass("btn-success").addClass("btn-danger"); }
      var green = function () { button.removeClass("btn-danger").addClass("btn-success"); }

      if ("geolocation" in navigator) {

         var options = {
            enableHighAccuracy: settings.location.enableHighAccuracy,
            maximumAge: settings.location.maximumAge < 0 ? Infinity : settings.location.maximumAge,
            timeout: settings.location.timeout < 0 ? Infinity : settings.location.timeout
         }

         var success = function (location) {

            settings.location.latitude.val(location.coords.latitude);
            settings.location.longitude.val(location.coords.longitude);
            settings.location.accuracy.val(location.coords.accuracy);
            settings.location.altitude.val(location.coords.altitude);
            settings.location.altitudeaccuracy.val(location.coords.altitudeAccuracy);
            settings.location.speed.val(location.coords.speed);
            settings.location.heading.val(location.coords.heading);

            green();
            busy.hide();
            display.text(location.coords.accuracy);
            console.log(location);

            // reset the button after 3 seconds
            setTimeout(function () {
               display.text("");
               arrow.fadeIn();
            }, 3000);
         }

         var locationError = function (error) {

            red();
            busy.hide();
            console.log(error);

            // show message instead of arrow
            arrow.hide();
            switch (error.code) {
               case error.PERMISSION_DENIED:
                  display.text("Blocked");
                  break;
               case error.POSITION_UNAVAILABLE:
                  display.text("Unknown");
                  break;
               case error.TIMEOUT:
                  display.text("Timeout");
                  break;
               case error.UNKNOWN_ERROR:
                  display.text("Error");
                  break;
            }

            // reset the button after 3 seconds
            setTimeout(function () {
               green();
               display.text("");
               arrow.fadeIn();
            }, 3000);
         }

         arrow.hide();
         busy.show();
         navigator.geolocation.getCurrentPosition(success, locationError, options);

      } else {
         button.removeClass("btn-success").addClass("btn-warning");
         button.prop("disabled", true);
      }
   }

   function block() {
      $('#busy').show();
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
   }

   bind();
});