amis.define('loadMonacoEditor.ts', function(require, exports, module, define) {

  "use strict";
  Object.defineProperty(exports, "__esModule", { value: true });
  exports.__mod__async__load = void 0;
  // 这是个特殊的方法，请看考 mod.js 里面的实现。
  function __mod__async__load(callback) {
      // @ts-ignore
      var monacoLoader = 'node_modules/monaco-editor/min/vs/loader.js';
      // @ts-ignore
      var script = amis.require.loadJs(filterUrl(monacoLoader));
      script.onload = function () { return onLoad(window.require, callback); };
  }
  exports.__mod__async__load = __mod__async__load;
  function __uri(url) {
      return url;
  }
  // 用于发布 sdk 版本的时候替换，因为不确定 sdk 版本怎么部署，而 worker 地址路径不可知。
  // 所以会被 fis3 替换成取相对的代码。
  function filterUrl(url) {
      return url;
  }
  function onLoad(req, callback) {
      var config = {
          'vs/nls': {
              availableLanguages: {
                  '*': 'zh-cn'
              }
          },
          paths: {
              vs: 'node_modules/monaco-editor/min/vs/editor/editor.main.js'.replace(/\/vs\/.*$/, ''),
              'vs/base/worker/workerMain': 'node_modules/monaco-editor/min/vs/base/worker/workerMain.js',
              'vs/basic-languages/apex/apex': 'node_modules/monaco-editor/min/vs/basic-languages/apex/apex.js',
              'vs/basic-languages/azcli/azcli': 'node_modules/monaco-editor/min/vs/basic-languages/azcli/azcli.js',
              'vs/basic-languages/clojure/clojure': 'node_modules/monaco-editor/min/vs/basic-languages/clojure/clojure.js',
              'vs/basic-languages/bat/bat': 'node_modules/monaco-editor/min/vs/basic-languages/bat/bat.js',
              'vs/basic-languages/coffee/coffee': 'node_modules/monaco-editor/min/vs/basic-languages/coffee/coffee.js',
              'vs/basic-languages/cpp/cpp': 'node_modules/monaco-editor/min/vs/basic-languages/cpp/cpp.js',
              'vs/basic-languages/csharp/csharp': 'node_modules/monaco-editor/min/vs/basic-languages/csharp/csharp.js',
              'vs/basic-languages/css/css': 'node_modules/monaco-editor/min/vs/basic-languages/css/css.js',
              'vs/basic-languages/dockerfile/dockerfile': 'node_modules/monaco-editor/min/vs/basic-languages/dockerfile/dockerfile.js',
              'vs/basic-languages/fsharp/fsharp': 'node_modules/monaco-editor/min/vs/basic-languages/fsharp/fsharp.js',
              'vs/basic-languages/go/go': 'node_modules/monaco-editor/min/vs/basic-languages/go/go.js',
              'vs/basic-languages/handlebars/handlebars': 'node_modules/monaco-editor/min/vs/basic-languages/handlebars/handlebars.js',
              'vs/basic-languages/html/html': 'node_modules/monaco-editor/min/vs/basic-languages/html/html.js',
              'vs/basic-languages/ini/ini': 'node_modules/monaco-editor/min/vs/basic-languages/ini/ini.js',
              'vs/basic-languages/java/java': 'node_modules/monaco-editor/min/vs/basic-languages/java/java.js',
              'vs/basic-languages/javascript/javascript': 'node_modules/monaco-editor/min/vs/basic-languages/javascript/javascript.js',
              'vs/basic-languages/less/less': 'node_modules/monaco-editor/min/vs/basic-languages/less/less.js',
              'vs/basic-languages/lua/lua': 'node_modules/monaco-editor/min/vs/basic-languages/lua/lua.js',
              'vs/basic-languages/markdown/markdown': 'node_modules/monaco-editor/min/vs/basic-languages/markdown/markdown.js',
              'vs/basic-languages/msdax/msdax': 'node_modules/monaco-editor/min/vs/basic-languages/msdax/msdax.js',
              'vs/basic-languages/objective-c/objective-c': 'node_modules/monaco-editor/min/vs/basic-languages/objective-c/objective-c.js',
              'vs/basic-languages/php/php': 'node_modules/monaco-editor/min/vs/basic-languages/php/php.js',
              'vs/basic-languages/postiats/postiats': 'node_modules/monaco-editor/min/vs/basic-languages/postiats/postiats.js',
              'vs/basic-languages/powershell/powershell': 'node_modules/monaco-editor/min/vs/basic-languages/powershell/powershell.js',
              'vs/basic-languages/pug/pug': 'node_modules/monaco-editor/min/vs/basic-languages/pug/pug.js',
              'vs/basic-languages/python/python': 'node_modules/monaco-editor/min/vs/basic-languages/python/python.js',
              'vs/basic-languages/r/r': 'node_modules/monaco-editor/min/vs/basic-languages/r/r.js',
              'vs/basic-languages/razor/razor': 'node_modules/monaco-editor/min/vs/basic-languages/razor/razor.js',
              'vs/basic-languages/redis/redis': 'node_modules/monaco-editor/min/vs/basic-languages/redis/redis.js',
              'vs/basic-languages/redshift/redshift': 'node_modules/monaco-editor/min/vs/basic-languages/redshift/redshift.js',
              'vs/basic-languages/ruby/ruby': 'node_modules/monaco-editor/min/vs/basic-languages/ruby/ruby.js',
              'vs/basic-languages/rust/rust': 'node_modules/monaco-editor/min/vs/basic-languages/rust/rust.js',
              'vs/basic-languages/sb/sb': 'node_modules/monaco-editor/min/vs/basic-languages/sb/sb.js',
              'vs/basic-languages/scheme/scheme': 'node_modules/monaco-editor/min/vs/basic-languages/scheme/scheme.js',
              'vs/basic-languages/scss/scss': 'node_modules/monaco-editor/min/vs/basic-languages/scss/scss.js',
              'vs/basic-languages/shell/shell': 'node_modules/monaco-editor/min/vs/basic-languages/shell/shell.js',
              'vs/basic-languages/solidity/solidity': 'node_modules/monaco-editor/min/vs/basic-languages/solidity/solidity.js',
              'vs/basic-languages/sql/sql': 'node_modules/monaco-editor/min/vs/basic-languages/sql/sql.js',
              'vs/basic-languages/st/st': 'node_modules/monaco-editor/min/vs/basic-languages/st/st.js',
              'vs/basic-languages/swift/swift': 'node_modules/monaco-editor/min/vs/basic-languages/swift/swift.js',
              'vs/basic-languages/typescript/typescript': 'node_modules/monaco-editor/min/vs/basic-languages/typescript/typescript.js',
              'vs/basic-languages/vb/vb': 'node_modules/monaco-editor/min/vs/basic-languages/vb/vb.js',
              'vs/basic-languages/xml/xml': 'node_modules/monaco-editor/min/vs/basic-languages/xml/xml.js',
              'vs/basic-languages/yaml/yaml': 'node_modules/monaco-editor/min/vs/basic-languages/yaml/yaml.js',
              'vs/editor/editor.main': 'node_modules/monaco-editor/min/vs/editor/editor.main.js',
              'vs/editor/editor.main.css': 'node_modules/monaco-editor/min/vs/editor/editor.main.css',
              'vs/editor/editor.main.nls': 'node_modules/monaco-editor/min/vs/editor/editor.main.nls.js',
              'vs/editor/editor.main.nls.zh-cn': 'node_modules/monaco-editor/min/vs/editor/editor.main.nls.zh-cn.js',
              // 'vs/editor/contrib/suggest/media/String_16x.svg': __uri('monaco-editor/min/vs/editor/contrib/suggest/media/String_16x.svg'),
              // 'vs/editor/contrib/suggest/media/String_inverse_16x.svg': __uri('monaco-editor/min/vs/editor/contrib/suggest/media/String_inverse_16x.svg'),
              // 'vs/editor/standalone/browser/quickOpen/symbol-sprite.svg': __uri('monaco-editor/min/vs/editor/standalone/browser/quickOpen/symbol-sprite.svg'),
              'vs/language/typescript/tsMode': 'node_modules/monaco-editor/min/vs/language/typescript/tsMode.js',
              // 'vs/language/typescript/lib/typescriptServices': __uri('monaco-editor/min/vs/language/typescript/lib/typescriptServices.js'),
              'vs/language/typescript/tsWorker': 'node_modules/monaco-editor/min/vs/language/typescript/tsWorker.js',
              'vs/language/json/jsonMode': 'node_modules/monaco-editor/min/vs/language/json/jsonMode.js',
              'vs/language/json/jsonWorker': 'node_modules/monaco-editor/min/vs/language/json/jsonWorker.js',
              'vs/language/html/htmlMode': 'node_modules/monaco-editor/min/vs/language/html/htmlMode.js',
              'vs/language/html/htmlWorker': 'node_modules/monaco-editor/min/vs/language/html/htmlWorker.js',
              'vs/language/css/cssMode': 'node_modules/monaco-editor/min/vs/language/css/cssMode.js',
              'vs/language/css/cssWorker': 'node_modules/monaco-editor/min/vs/language/css/cssWorker.js'
          }
      };
      Object.keys(config.paths).forEach(function (key) {
          config.paths[key] = filterUrl(config.paths[key].replace(/\.js$/, ''));
      });
      req.config(config);
      if (/^(https?:)?\/\//.test(config.paths.vs)) {
          window.MonacoEnvironment = {
              getWorkerUrl: function () {
                  return "data:text/javascript;charset=utf-8," + encodeURIComponent("\n              self.MonacoEnvironment = {\n                  baseUrl: '" + config.paths.vs + "',\n                  paths: " + JSON.stringify(config.paths) + "\n              };\n              importScripts('" + 'node_modules/monaco-editor/min/vs/base/worker/workerMain.js' + "');");
              }
          };
      }
      else {
          delete window.MonacoEnvironment;
      }
      req(['vs/editor/editor.main'], function (ret) {
          callback(ret);
      });
  }
  //# sourceMappingURL=/loadMonacoEditor.js.map
  

});
