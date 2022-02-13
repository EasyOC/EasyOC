using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace TransformalizeModule {

   public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions> {

      private static readonly ResourceManifest _manifest;

      static ResourceManagementOptionsConfiguration() {
         _manifest = new ResourceManifest();

         /* CodeMirror */
         _manifest
          .DefineScript("codemirror")
          .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/codemirror.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/codemirror.js")
          .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/codemirror.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/codemirror.js")
          .SetCdnIntegrity("sha256-id5Qk/MwQJxgNlDFDpVymUuReXfTUZiaQKb8arrddQM=", "sha256-C1vNlVkHhwXPBxyzyyUmeHeOdlVRzbzC+teAGXhUEUc=")
          .SetVersion("5.52.2");

         _manifest
             .DefineStyle("codemirror")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/codemirror.min.css", $"~/{Common.ModuleName}/Scripts/codemirror/codemirror.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/codemirror.min.css", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/codemirror.css")
             .SetCdnIntegrity("sha256-vZ3SaLOjnKO/gGvcUWegySoDU6ff33CS5i9ot8J9Czk=", "sha256-PWWDlCsO+b6rKHz9dgBeS5cqTmBbebfAtNXnEqy3Xvc=")
             .SetVersion("5.52.2");

         /* CodeMirror XML */
         _manifest
             .DefineScript("codemirror-mode-xml")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/mode/xml/xml.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/mode/xml/xml.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/mode/xml/xml.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/mode/xml/xml.js")
             .SetCdnIntegrity("sha256-Lfk8z6WUsBN6YiCaMpH6bxBHyRqkPK4O2QbQHFNUS40=", "sha256-yhHPVEbMcHCb0TOtv6Leq8f3VEVe3+Ot0oCy83K+jvs=")
             .SetVersion("5.52.2");

         /* Full Screen (not working correctly) */
         _manifest
            .DefineScript("codemirror-addon-display-fullscreen")
            .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/display/fullscreen.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/display/fullscreen.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/display/fullscreen.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/display/fullscreen.js")
            .SetCdnIntegrity("sha256-ttglgk8dprl46qouhLrnP75y3ykP97gJf53RKg9htE4=", "sha256-HhKG06Ib+xZ6RYvmBap8s0os+CEXWPArOCet4VaME6Q=")
            .SetVersion("5.52.2");

         /* Fold Gutter */
         _manifest
             .DefineScript("codemirror-addon-fold-foldgutter")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldgutter.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldgutter.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldgutter.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldgutter.js")
             .SetCdnIntegrity("sha256-FAVRbCrAsSe4mCyvHf9LZ8u7nT0d7gwaC1MhJzxoTNE=", "sha256-UjqbKBTcJwani3fXnb3B6UWguF8iEYAouMsTn425SKU=")
             .SetVersion("5.52.2");

         /* Dialog */
         _manifest
             .DefineScript("codemirror-addon-dialog-dialog")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/dialog/dialog.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/dialog/dialog.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/dialog/dialog.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/dialog/dialog.js")
             .SetCdnIntegrity("sha256-G+QhvxjUNi5P5cyQqjROwriSUy2lZtCFUQh+8W1o6I0=", "sha256-HuoOUA3OENhZTY1oGpHRtxpCCTVcnCmIXobiay12aF4=")
             .SetVersion("5.52.2");

         /* Show Hint */
         _manifest
             .DefineScript("codemirror-addon-hint-show-hint")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/show-hint.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/show-hint.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/show-hint.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/show-hint.js")
             .SetCdnIntegrity("sha256-s030a2NmcCVudLqFb85WQPTfkO1LlDXHxo3XuWxGYGA=", "sha256-c5yCM5a9nH56AOWRp59e1TjsRi1EPfsAHf0O7WFl30s=")
             .SetVersion("5.52.2");

         /* Any Word Hint */
         _manifest
             .DefineScript("codemirror-addon-hint-anyword-hint")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/anyword-hint.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/anyword-hint.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/anyword-hint.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/anyword-hint.min.js")
             .SetCdnIntegrity("sha256-RAnsThvT8u3giTsRpAwZ+KWs/LPrEdNtwq2GyPb/RsQ=", "sha256-lCVMIcSF33YZe03bIZs2h/Cs2TIWgZGvmIs8FTYO3e0=")
             .SetVersion("5.52.2");

         /* Fold Code */
         _manifest
             .DefineScript("codemirror-addon-fold-foldcode")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldcode.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldcode.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldcode.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldcode.js")
             .SetCdnIntegrity("sha256-qhqsqHF0ep05BTkxty6v8+3968ipN9bdCxXx5Q5dbgk=", "sha256-eHCxXsZStfhMVmmDh1UQZwVz6RDWmxDTohRrwg/Wifo=")
             .SetVersion("5.52.2");

         /* Search */
         _manifest
             .DefineScript("codemirror-addon-search-search")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/search/search.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/search/search.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/search.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/search.js")
             .SetCdnIntegrity("sha256-pk1ahN30IsCG20LJu38Va1A7tQagksJwAJUJK3rBFe0=", "sha256-iUnNlgkrU5Jj8oKl2zBBCTmESI2xpXwZrTX+arxSEKc=")
             .SetVersion("5.52.2");

         /* Search Cursor */
         _manifest
             .DefineScript("codemirror-addon-search-searchcursor")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/search/searchcursor.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/search/searchcursor.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/searchcursor.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/searchcursor.js")
             .SetCdnIntegrity("sha256-LE6iKRf3wBxVYAW8kInWVcU3A9hYtDaphl4Wgdg4ZkU=", "sha256-B1aSPEmOtZ0K3toDpFwq4H3V3nbtCpIziOHNOG80wkk=")
             .SetVersion("5.52.2");

         /* Search Jump to Line */
         _manifest
             .DefineScript("codemirror-addon-search-jump-to-line")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/search/jump-to-line.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/search/jump-to-line.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/jump-to-line.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/jump-to-line.js")
             .SetCdnIntegrity("sha256-6hE+UvbWF7EVpwVlstz+DltSX0qu32C/v5neucv+f0E=", "sha256-sMswWLNouxW7Fh3iOm5ozacSAnrp9shgUaJomWMlDtQ=")
             .SetVersion("5.52.2");

         /* Search Match Highlighter */
         _manifest
             .DefineScript("codemirror-addon-search-match-highlighter")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/search/match-highlighter.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/search/match-highlighter.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/match-highlighter.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/search/match-highlighter.js")
             .SetCdnIntegrity("sha256-1ldu9k2AN9gh9HWCK61lA5l/XVCZ7qsExtiEPl9J19w=", "sha256-PtGq+Pjt1eqhreHfFTNKzfdrX59WufE+PQRJIJop9XM=")
             .SetVersion("5.52.2");

         /* XML Fold */
         _manifest
             .DefineScript("codemirror-addon-fold-xml-fold")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/xml-fold.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/xml-fold.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/xml-fold.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/xml-fold.js")
             .SetCdnIntegrity("sha256-6CqMc7XMXXv0ffuJv52X0GHqK1/q7PjETdy2GOLIhAY=", "sha256-6qrza98BMjZqcPGsh7xpweWNL52WV7KmoCz6NUkA1qo=")
             .SetVersion("5.52.2");

         /* Close Tag */
         _manifest
             .DefineScript("codemirror-addon-edit-closetag")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/edit/closetag.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/edit/closetag.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/edit/closetag.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/edit/closetag.js")
             .SetCdnIntegrity("sha256-HNFx5VDaZS4Zl4JNDio1E6ISPgCbPmP/LU/wug8PS08=", "sha256-cAuKp8RuBAKH4y/9HtuGpC0J7cWyqVlCQVkLFUwCMr0=")
             .SetVersion("5.52.2");

         /* Match Tags */
         _manifest
             .DefineScript("codemirror-addon-edit-matchtags")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/edit/matchtags.min.js", $"~/{Common.ModuleName}/Scripts/codemirror/addon/edit/matchtags.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/edit/matchtags.min.js", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/edit/matchtags.js")
             .SetCdnIntegrity("sha256-7+ar9rS4zfA49+LlLzDc0O7Wzf7tFqxTjo38KHBObAA=", "sha256-oCAwj6P1/BzATEuHMQxLOWONXkQHh4FLz8JFcIH/+hQ=")
             .SetVersion("5.52.2");

         /* Full Screen Style */
         _manifest
             .DefineStyle("codemirror-addon-display-fullscreen")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/display/fullscreen.css", $"~/{Common.ModuleName}/Scripts/codemirror/addon/display/fullscreen.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/display/fullscreen.css", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/display/fullscreen.css")
             .SetCdnIntegrity("sha256-SpuaNYgDjBMdeyjrjtsC+U5fpSDpftPNv7oO8HQvG7w=", "sha256-SpuaNYgDjBMdeyjrjtsC+U5fpSDpftPNv7oO8HQvG7w=")
             .SetVersion("5.52.2");

         /* Fold Gutter Style */
         _manifest
             .DefineStyle("codemirror-addon-fold-foldgutter")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldgutter.min.css", $"~/{Common.ModuleName}/Scripts/codemirror/addon/fold/foldgutter.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldgutter.min.css", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/fold/foldgutter.css")
             .SetCdnIntegrity("sha256-PAZt4Yo+uLbowOVolpiWbNrg1VUtA43Zvw/TPBABeaQ=", "sha256-V27800C3cLYNd9jCnp3za/WFdjkb6rUbQ5EU5O0BrFY=")
             .SetVersion("5.52.2");

         /* Dialog Style */
         _manifest
             .DefineStyle("codemirror-addon-dialog-dialog")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/dialog/dialog.min.css", $"~/{Common.ModuleName}/Scripts/codemirror/addon/dialog/dialog.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/dialog/dialog.min.css", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/dialog/dialog.min.css")
             .SetCdnIntegrity("sha256-OjF42ew3ra0/zNWgjfDTydf609RXv0cXcassXEeW0O8=", "sha256-XfaQ13HxIRg0hWLdKpAGBDOuLt7M0JCKvKpEgLHj5Gg=")
             .SetVersion("5.52.2");

         /* Show Hint Style */
         _manifest
             .DefineStyle("codemirror-addon-hint-show-hint")
             .SetUrl($"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/show-hint.min.css", $"~/{Common.ModuleName}/Scripts/codemirror/addon/hint/show-hint.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/show-hint.min.css", "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.52.2/addon/hint/show-hint.css")
             .SetCdnIntegrity("sha256-Ng5EdzHS/CC37tR7tE75e4Th9+fBvOB4eYITOkXS22Q=", "sha256-9PjI1IwVXcv44E37KbDVAJdEYNCCBK3+Ddo757s6L8g=")
             .SetVersion("5.52.2");

         // 1.14.0-beta for Bootstrap 5 Support
         _manifest
             .DefineScript("bootstrap-select-beta")
             .SetDependencies("jQuery", "bootstrap")
             .SetUrl($"~/{Common.ModuleName}/Scripts/bootstrap-select.min.js", $"~/{Common.ModuleName}/Scripts/bootstrap-select.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.14.0-beta2/js/bootstrap-select.min.js", "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.14.0-beta2/js/bootstrap-select.js")
             .SetCdnIntegrity("sha512-FHZVRMUW9FsXobt+ONiix6Z0tIkxvQfxtCSirkKc5Sb4TKHmqq1dZa8DphF0XqKb3ldLu/wgMa8mT6uXiLlRlw==", "sha512-ygyrAVxux4a+xKAg4oj7ZKVKZgx+3lT4CYLkoYNT5VaSZcqfYDGDQkiNUBvS/Ggul5g10aCIlMsZ9qrjoUME9A==")
             .SetVersion("1.14.0");

         _manifest
             .DefineStyle("bootstrap-select-beta")
             .SetUrl($"~/{Common.ModuleName}/Styles/bootstrap-select.min.css", $"~/{Common.ModuleName}/Styles/bootstrap-select.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.14.0-beta2/css/bootstrap-select.min.css", "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.14.0-beta2/css/bootstrap-select.css")
             .SetCdnIntegrity("sha512-mR/b5Y7FRsKqrYZou7uysnOdCIJib/7r5QeJMFvLNHNhtye3xJp1TdJVPLtetkukFn227nKpXD9OjUc09lx97Q==", "sha512-txEvwMt/+RgCCbxKwzCZNTusZHDw1To/n29O2UglIWO01ce9Ys/QOp3YV/ZEOyxxKTQuFFNZFgaOiPmN6EgPyw==")
             .SetVersion("1.14.0");

         // Prismjs
         _manifest
             .DefineScript("prism")
             .SetUrl($"~/{Common.ModuleName}/Scripts/prismjs/prism.min.js", $"~/{Common.ModuleName}/Scripts/prismjs/prism.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/prism.min.js", "https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/prism.js")
             .SetCdnIntegrity("sha256-3teItwIfMuVB74Alnxw/y5HAZ2irOsCULFff3EgbtEs=", "sha256-ooBCsFqD8/ih41/S1sKjWdtTyGWUuetgVvUWRF1CbN0=")
             .SetVersion("1.20.0");

         _manifest
             .DefineScript("prism-markup")
             .SetUrl($"~/{Common.ModuleName}/Scripts/prismjs/components/prism-markup.min.js", $"~/{Common.ModuleName}/Scripts/prismjs/components/prism-markup.js")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/components/prism-markup.min.js", "https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/components/prism-markup.js")
             .SetCdnIntegrity("sha256-Jp/hNSrSnxiyz2YWkA6uZSI7FMULRHemA0yA/5BhEdU=", "sha256-n9xE+cin+LSEDenLFLMs1i+Q/EidigipCXNw/Hx+4Ys=")
             .SetVersion("1.20.0");

         _manifest
             .DefineStyle("prism")
             .SetUrl($"~/{Common.ModuleName}/Styles/prismjs/prism.min.css", $"~/{Common.ModuleName}/Styles/prismjs/prism.min.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/themes/prism.min.css", "https://cdnjs.cloudflare.com/ajax/libs/prism/1.20.0/themes/prism.css")
             .SetCdnIntegrity("sha256-cuvic28gVvjQIo3Q4hnRpQSNB0aMw3C+kjkR0i+hrWg=", "sha256-LArq5Cddn4QETLgVbBZppSkBLJbL1dhacPVQF/EPH00=")
             .SetVersion("1.20.0");

         _manifest
            .DefineScript("block-ui")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.blockUI.min.js", $"~/{Common.ModuleName}/Scripts/jquery.blockUI.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js", "https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.js")
            .SetCdnIntegrity("sha256-9wRM03dUw6ABCs+AU69WbK33oktrlXamEXMvxUaF+KU=", "sha256-oQaw+JJuUcJQ9QVYMcFnPxICDT+hv8+kuxT2FNzTGhc=")
            .SetVersion("2.70.0");

         _manifest
            .DefineScript("parsley")
            .SetUrl($"~/{Common.ModuleName}/Scripts/parsley.min.js", $"~/{Common.ModuleName}/Scripts/parsley.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/parsley.js/2.9.2/parsley.min.js", "https://cdnjs.cloudflare.com/ajax/libs/parsley.js/2.9.2/parsley.js")
            .SetCdnIntegrity("sha256-pEdn/pJ2tyT37axbEIPkyUUfuG1yXR0+YV+h+jphem4=", "sha256-vkPt2+rodbvJto9KapXeP61nmLcz3VXyzcK/gaWjOEg=")
            .SetVersion("2.9.2");

         _manifest
            .DefineScript("moment")
            .SetUrl($"~/{Common.ModuleName}/Scripts/moment.min.js", $"~/{Common.ModuleName}/Scripts/moment.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.26.0/moment.min.js", "https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.26.0/moment.js")
            .SetCdnIntegrity("sha256-5oApc/wMda1ntIEK4qoWJ4YItnV4fBHMwywunj8gPqc=", "sha256-GIi3faatmXJKbOQPmLgUPDHXKYmXBSszcO9Euf0BQPk=")
            .SetVersion("2.26.0");

         _manifest
            .DefineScript("pickadate-picker")
            .SetUrl($"~/{Common.ModuleName}/Scripts/pickadate/picker.min.js", $"~/{Common.ModuleName}/Scripts/pickadate/picker.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/compressed/picker.js", "https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/picker.js")
            .SetCdnIntegrity("sha256-Ir/Txs2EGYQz5HcltQCu06WpUQRhmU4tgHHYbNV0+Cs=", "sha256-oJXL7pU64acHojjSG2JPJy+FOGtYh5qnKhtdwk4YlrU=")
            .SetVersion("3.6.4");

         _manifest
            .DefineScript("pickadate-picker-date")
            .SetDependencies(new[] { "pickadate-picker" })
            .SetUrl($"~/{Common.ModuleName}/Scripts/pickadate/picker.date.min.js", $"~/{Common.ModuleName}/Scripts/pickadate/picker.date.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/compressed/picker.date.js", "https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/picker.date.js")
            .SetCdnIntegrity("sha256-WpEr1Ovyxho8DRYP1DyZgjVonSAGF4uDVVZXoe379vw=", "sha256-4jRSfwItZuLvFKlnkeHM+vL0bWgvrVbIF8peMhB7t2Q=")
            .SetVersion("3.6.4");

         _manifest
            .DefineStyle("pickadate-themes-default")
            .SetUrl($"~/{Common.ModuleName}/Styles/pickadate/themes/default.css", $"~/{Common.ModuleName}/Styles/pickadate/themes/default.min.css")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/compressed/themes/default.css", "https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/themes/default.css")
            .SetCdnIntegrity("sha512-CIn19F6Q/91ayiK+V1qCXZf5SB96gnFh89uEvEUWTlYnKua5D2r8W3Vb7ghBsTzVjmWv/VZgQyK8RrK0JKQYXg==", "sha512-x9ZSPqJJfUhtPuo+fw6331wHeC3vhDpNI3Iu4KC05zJrxx7MWYewaDaASGxAUgWyrwU50oFn6Xk0CrQnTSuoOA==")
            .SetVersion("3.6.4");

         _manifest
            .DefineStyle("pickadate-themes-default-date")
            .SetUrl($"~/{Common.ModuleName}/Styles/pickadate/themes/default.date.css", $"~/{Common.ModuleName}/Styles/pickadate/themes/default.date.min.css")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/compressed/themes/default.date.css", "https://cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.6.4/themes/default.date.css")
            .SetCdnIntegrity("sha512-KUP5BIvEUWCw+9FbI4LJz+0J6tG6+V+ZY2Lzpzx48YtPitTypKSy1U+yzcOBymRv2isdItYcYCTgLDtHw0Z3rA==", "sha512-Ix4qjGzOeoBtc8sdu1i79G1Gxy6azm56P4z+KFl+po7kOtlKhYSJdquftaI4hj1USIahQuZq5xpg7WgRykDJPA==")
            .SetVersion("3.6.4");

         // mapbox-gl 
         _manifest
            .DefineScript("mapbox-gl")
            .SetUrl($"~/{Common.ModuleName}/Scripts/mapbox-gl.min.js", $"~/{Common.ModuleName}/Scripts/mapbox-gl.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/mapbox-gl/1.11.0/mapbox-gl.min.js", "https://cdnjs.cloudflare.com/ajax/libs/mapbox-gl/1.11.0/mapbox-gl.js")
            .SetCdnIntegrity("sha256-hLz/HqgIJeNiL4hp7kU0r9ADehgHb1JjZUYvbOECxGE=", "sha256-W5jjCUuqo0OITPxQlNtQW7e+YHLzNGgI9XOtTSBWtbI=")
            .SetVersion("1.11.0");

         _manifest
             .DefineStyle("mapbox-gl")
             .SetUrl($"~/{Common.ModuleName}/Styles/mapbox-gl.min.css", $"~/{Common.ModuleName}/Styles/mapbox-gl.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/mapbox-gl/1.11.0/mapbox-gl.min.css", "https://cdnjs.cloudflare.com/ajax/libs/mapbox-gl/1.11.0/mapbox-gl.css")
             .SetCdnIntegrity("sha256-ETq/yEgk/50QT5U21jYaBxXCgyJNasHovjc3h06k+kw=", "sha256-+A0NJBMh5IIbhodBU6EO4tNTW4dNHmW7LgzCwGGsD1I=")
             .SetVersion("1.11.0");

         _manifest
            .DefineScript("mapbox-gl-draw")
            .SetCdn("https://cdn.jsdelivr.net/npm/@mapbox/mapbox-gl-draw@1.1.2/dist/mapbox-gl-draw.js", "https://cdn.jsdelivr.net/npm/@mapbox/mapbox-gl-draw@1.1.2/dist/mapbox-gl-draw-unminified.js")
            .SetCdnIntegrity("sha256-SUQgYRFU5w8druRUF892Yk8vffWzabkNDl/2J0FsGGU=", "sha256-Uw4D7+Dcz15WM0mKBUcIiobarKkhotl9Y8zETc7yszw=")
            .SetUrl($"~/{Common.ModuleName}/Scripts/mapbox-gl-draw.min.js", $"~/{Common.ModuleName}/Scripts/mapbox-gl.js")
            .SetVersion("1.1.2");

         _manifest
             .DefineStyle("mapbox-gl-draw")
             .SetCdn("https://cdn.jsdelivr.net/npm/@mapbox/mapbox-gl-draw@1.1.2/dist/mapbox-gl-draw.css", "https://cdn.jsdelivr.net/npm/@mapbox/mapbox-gl-draw@1.1.2/dist/mapbox-gl-draw.css")
             .SetCdnIntegrity("sha256-M/XwXDzL3ye8si/4fN/D8RxD/wq0y/M5nlOXsRAcZvc=", "sha256-M/XwXDzL3ye8si/4fN/D8RxD/wq0y/M5nlOXsRAcZvc=")
             .SetUrl($"~/{Common.ModuleName}/Styles/mapbox-gl-draw.min.css", $"~/{Common.ModuleName}/Styles/mapbox-gl-draw.css")
             .SetVersion("1.1.2");

         _manifest
            .DefineScript("turf")
            .SetUrl($"~/{Common.ModuleName}/Scripts/turf.min.js", $"~/{Common.ModuleName}/Scripts/turf.js")
            .SetCdn("https://cdn.jsdelivr.net/npm/@turf/turf@5.1.6/turf.min.js", "https://cdn.jsdelivr.net/npm/@turf/turf@5.1.6/turf.js")
            .SetCdnIntegrity("sha256-Nhtik+K3xORhs5S9o0qlol4eRNB9O5OyTOJqbLucPk8=", "sha256-9WY2IAez52ncSmDHF2+kRA47S3/U9rBcRB6x2bUpfR4=")
            .SetVersion("5.1.6");

         _manifest
            .DefineScript("bootstrap-calendar")
            .SetUrl($"~/{Common.ModuleName}/Scripts/calendar.min.js", $"~/{Common.ModuleName}/Scripts/calendar.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/bootstrap-calendar/0.2.5/js/calendar.min.js", "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-calendar/0.2.5/js/calendar.js")
            .SetCdnIntegrity("sha512-1tpLJ4USZ1V/MWwEfikVjpCMnKv9uy0jVWs+jX7Hl9Uca129P9lrFXyBcMx2ckIm6MM1MlELeK2dLIo5nHhP7A==", "sha512-oXhFBLw3n73Zt5berP4kOmKksbaFi4NdO4/yoDJid4u5zk8vmOQVZ0QTCGh95L+hsKMgt7d31KqmSm3FdRO9kQ==")
            .SetVersion("0.2.5");

         _manifest
             .DefineStyle("bootstrap-calendar")
             .SetUrl($"~/{Common.ModuleName}/Styles/calendar.min.css", $"~/{Common.ModuleName}/Styles/calendar.css")
             .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/bootstrap-calendar/0.2.5/css/calendar.min.css", "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-calendar/0.2.5/css/calendar.css")
             .SetCdnIntegrity("sha512-zmbBaOaW0O7Zo4gYPd/Ef9sAMiv1doq5erbIWhuYcfGacLP4hscq/V45c5CwYKhqO9ffFW+YYCyhqY+WJJyO4g==", "sha512-z3HKbBdaba6ixpItVs/TjdnOEQvJO6Zf3pK4T7zzncPYNz10J4y/4CeJ+2iGmut86H7PMAK74duJP9KLKCT3UA==")
             .SetVersion("0.2.5");

         _manifest
            .DefineScript("underscore")
            .SetUrl($"~/{Common.ModuleName}/Scripts/underscore.min.js", $"~/{Common.ModuleName}/Scripts/underscore.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/underscore.js/1.4.1/underscore-min.js", "https://cdnjs.cloudflare.com/ajax/libs/underscore.js/1.4.1/underscore.js")
            .SetCdnIntegrity("sha512-xHPfYya0Ac9NYcp0d6YKVnP/n7dcRGiQCsGKC+BMpziXwgg/6VogplMOS+nqUXQIPmtuGwZ25fAcSgtjBxBVfg==", "sha512-qbJl2/KyecDVJdgAwr0Gzhm5KBmkzdUouG+sKmCWP1Idb144ZnfSmCRReuYXPEHIa4j38TwrR2ySeGoFH5gKzA==")
            .SetVersion("1.4.1");

         /* scripts for jquery file upload (blueimp) */

         _manifest
            .DefineScript("load-image-all")
            .SetUrl($"~/{Common.ModuleName}/Scripts/load-image.all.min.js", $"~/{Common.ModuleName}/Scripts/load-image.all.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/blueimp-load-image/5.14.0/load-image.all.min.js", "https://cdnjs.cloudflare.com/ajax/libs/blueimp-load-image/5.14.0/load-image.all.min.js")
            .SetCdnIntegrity("sha512-HZg0q8NV+VTxnU6hdkK0rL+fSmTGCbXZ3mHjqCCi87St5QRdvXENfRxkMK692inskRsCPee07d7VwcKNWaByCQ==", "sha512-HZg0q8NV+VTxnU6hdkK0rL+fSmTGCbXZ3mHjqCCi87St5QRdvXENfRxkMK692inskRsCPee07d7VwcKNWaByCQ==")
            .SetVersion("5.14.0");

         _manifest
            .DefineScript("canvas-to-blob")
            .SetUrl($"~/{Common.ModuleName}/Scripts/canvas-to-blob.js", $"~/{Common.ModuleName}/Scripts/canvas-to-blob.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/javascript-canvas-to-blob/3.28.0/js/canvas-to-blob.min.js", "https://cdnjs.cloudflare.com/ajax/libs/javascript-canvas-to-blob/3.28.0/js/canvas-to-blob.js")
            .SetCdnIntegrity("sha512-Q0RYciuNUJLzUgA9T0A0yj1A5wR+rM7MDr3hKBJuNBaSW5O2TqRyBMMLswe7Z/M9czXRnD/5mh1/xQpDpxFZaA==", "sha512-QinObFNs7mVBtipyw8BEERLHKQ1P2n5Wbxd8Kt+G9ST/lp99qZKlJUUsNSZYSPf/yGL7eNN0UCcDaMiZjXMtGg==")
            .SetVersion("3.28.0");

         _manifest
            .DefineScript("jquery-iframe-transport")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.iframe-transport.js", $"~/{Common.ModuleName}/Scripts/jquery.iframe-transport.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.iframe-transport.min.js", "https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.iframe-transport.js")
            .SetCdnIntegrity("sha512-nPWpXBDIYz79dENtnSj+4vttX5zcmZ4RE2pLj24hdMjFqNxEh5TNwjnyhpdJJxq9cxMy7NLgG06w5s6wytsOiw==", "sha512-0Qbkur+WwOSY3vH1iE0Q7dEZPQGqjtyjaO5u2FoRKNfBGXv2Fjxv8pysarhgHxRXBlr9Yc9Bbi58m4k0qxmYhw==")
            .SetVersion("10.31.0");

         _manifest
            .DefineScript("jquery-fileupload")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.fileupload.js", $"~/{Common.ModuleName}/Scripts/jquery.fileupload.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload.min.js", "https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload.js")
            .SetCdnIntegrity("sha512-qPkNWpUqYz8bhO5bGNPBvlCB9hPZBil2ez5Mo8yVmpCKI315UDDPQeg/TE7KwZ+U/wdSO8JguwVxYY/Ha7U+vQ==", "sha512-mPBKSUQd9V0RWuHe31Q2nvLSUSBOh4yjW21MBFIYdR8PPZL1mDU9clClGv9SwVSJC9m44+wcfljUPSqWTv91Xg==")
            .SetVersion("10.31.0");

         _manifest
            .DefineScript("jquery-fileupload-process")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.fileupload-process.js", $"~/{Common.ModuleName}/Scripts/jquery.fileupload-process.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload-process.min.js", "https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload-process.js")
            .SetCdnIntegrity("sha512-/PpB/cqFe0WgMQRLimSpDJtdyl3sRyo0bxwtiapDaYhs3AkEeNnDUBKtdLKpEEE2X/Xr2YzfCZTdLIOSC2JHVA==", "sha512-nSNTn7MSa1e/I0wf7fi4vbSAQVgO1x3Tum/spzI00lOV+Yd9uJWH18jjYiwx4WHVj3p7UR6tF2de7U094x5rCw==")
            .SetVersion("10.31.0");

         _manifest
            .DefineScript("jquery-fileupload-image")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.fileupload-image.js", $"~/{Common.ModuleName}/Scripts/jquery.fileupload-image.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload-image.min.js", "https://cdnjs.cloudflare.com/ajax/libs/blueimp-file-upload/10.31.0/js/jquery.fileupload-image.js")
            .SetCdnIntegrity("sha512-/zXq+I7ihnFX2Jw9+7lNZX9/oZ323b3rOMtwtowHN2VS3xoeLY1srC11oiQidw1YDTxrHVLWp9dehlZqqLKqhg==", "sha512-e0MZer7eLu4GULyhtYxafD0hyDK9anMSL27EqC4H/c1paOzw0xT4d7nuat2KG+Np65HQzz9fmO1HeW1bt6P9MQ==")
            .SetVersion("10.31.0");

         _manifest
            .DefineScript("bs-custom-file-input")
            .SetUrl($"~/{Common.ModuleName}/Scripts/bs-custom-file-input.js", $"~/{Common.ModuleName}/Scripts/bs-custom-file-input.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/bs-custom-file-input/1.3.4/bs-custom-file-input.min.js", "https://cdnjs.cloudflare.com/ajax/libs/bs-custom-file-input/1.3.4/bs-custom-file-input.js")
            .SetCdnIntegrity("sha512-91BoXI7UENvgjyH31ug0ga7o1Ov41tOzbMM3+RPqFVohn1UbVcjL/f5sl6YSOFfaJp+rF+/IEbOOEwtBONMz+w==", "sha512-TkqvBtxfAZgTiRN9Xuoh/dkowLIcuUp4BKHRC6BFINpofxwDaK683A/XqLZlGtGeF+9f2+vBss6hP91EaJ5D7w==")
            .SetVersion("1.3.4");

         _manifest
            .DefineScript("jquery-are-you-sure")
            .SetUrl($"~/{Common.ModuleName}/Scripts/jquery.are-you-sure.js", $"~/{Common.ModuleName}/Scripts/jquery.are-you-sure.min.js")
            .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/jquery.AreYouSure/1.9.0/jquery.are-you-sure.min.js", "https://cdnjs.cloudflare.com/ajax/libs/jquery.AreYouSure/1.9.0/jquery.are-you-sure.js")
            .SetCdnIntegrity("sha512-YuZemcyQ8r4w8tdxIzkJVgWfHrzSQN9PuF18I490DE8H97DOkrt+bolBf3/mve+9t8SLWODBzFCCPxH/vZYqBg==", "sha512-mYoXRCW7jn4Pm7gT7iqWdM42rpYge7g/Hegu5ttt+KqTmiuX8y6OpFUU644Ld+PlTTiS5Cecoiwoc+5homJ1oQ==")
            .SetVersion("1.9.0");


      }

      public void Configure(ResourceManagementOptions options) {
         options.ResourceManifests.Add(_manifest);
      }

   }
}
