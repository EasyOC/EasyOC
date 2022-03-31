amis.define('editor/MyRenderer.tsx', function(require, exports, module, define) {

  "use strict";
  Object.defineProperty(exports, "__esModule", { value: true });
  var tslib_1 = require("node_modules/tslib/tslib");
  var amis_editor_1 = require("node_modules/amis-editor/dist/index.min");
  var MyRendererEditor = /** @class */ (function (_super) {
      tslib_1.__extends(MyRendererEditor, _super);
      function MyRendererEditor() {
          var _this = _super !== null && _super.apply(this, arguments) || this;
          _this.tipName = '自定义组件';
          _this.settingsSchema = {
              title: '自定义组件配置',
              controls: [
                  {
                      type: 'tabs',
                      tabsMode: 'line',
                      className: 'm-t-n-xs',
                      contentClassName: 'no-border p-l-none p-r-none',
                      tabs: [
                          {
                              title: '常规',
                              controls: [
                                  {
                                      name: 'target',
                                      label: 'Target',
                                      type: 'text'
                                  }
                              ]
                          },
                          {
                              title: '外观',
                              controls: []
                          }
                      ]
                  }
              ]
          };
          return _this;
          // 配置表单一些简单的基本上够用了。
          // 还有一些逻辑可以复写来自定义的，但是我现在没时间写说明了。
      }
      MyRendererEditor = tslib_1.__decorate([
          amis_editor_1.RendererEditor('my-renderer', {
              name: '自定义渲染器',
              description: '这只是个示例',
              // docLink: '/docs/renderers/Nav',
              type: 'my-renderer',
              previewSchema: {
                  // 用来生成预览图的
                  type: 'my-renderer',
                  target: 'demo'
              },
              scaffold: {
                  // 拖入组件里面时的初始数据
                  type: 'my-renderer',
                  target: '233'
              }
          })
      ], MyRendererEditor);
      return MyRendererEditor;
  }(amis_editor_1.BasicEditor));
  exports.default = MyRendererEditor;
  //# sourceMappingURL=/editor/MyRenderer.js.map
  

});
