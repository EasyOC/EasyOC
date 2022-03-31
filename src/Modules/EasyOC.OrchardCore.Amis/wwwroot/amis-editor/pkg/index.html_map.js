amis.require.resourceMap({
  "res": {
    "loadMonacoEditor.ts": {
      "url": "loadMonacoEditor.js",
      "type": "js"
    },
    "route/NotFound.tsx": {
      "url": "route/NotFound.js",
      "type": "js"
    },
    "component/AMISRenderer.tsx": {
      "url": "component/AMISRenderer.js",
      "type": "js"
    },
    "component/AddPageModal.tsx": {
      "url": "component/AddPageModal.js",
      "type": "js",
      "deps": [
        "component/AMISRenderer.tsx"
      ]
    },
    "route/Preview.tsx": {
      "url": "route/Preview.js",
      "type": "js",
      "deps": [
        "route/NotFound.tsx",
        "component/AMISRenderer.tsx",
        "component/AddPageModal.tsx"
      ]
    },
    "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.production.min": {
      "url": "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.production.min.js",
      "type": "js"
    },
    "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.development": {
      "url": "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.development.js",
      "type": "js"
    },
    "node_modules/amis-editor/node_modules/mobx-react/dist/index": {
      "url": "node_modules/amis-editor/node_modules/mobx-react/dist/index.js",
      "type": "js",
      "deps": [
        "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.production.min",
        "node_modules/amis-editor/node_modules/mobx-react/dist/mobxreact.cjs.development"
      ]
    },
    "node_modules/amis-editor/dist/index.min": {
      "url": "node_modules/amis-editor/dist/index.min.js",
      "type": "js",
      "deps": [
        "node_modules/amis-editor/node_modules/mobx-react/dist/index"
      ]
    },
    "renderer/MyRenderer.tsx": {
      "url": "renderer/MyRenderer.js",
      "type": "js"
    },
    "editor/MyRenderer.tsx": {
      "url": "editor/MyRenderer.js",
      "type": "js",
      "deps": [
        "node_modules/amis-editor/dist/index.min"
      ]
    },
    "route/Editor.tsx": {
      "url": "route/Editor.js",
      "type": "js",
      "deps": [
        "node_modules/amis-editor/dist/index.min",
        "renderer/MyRenderer.tsx",
        "editor/MyRenderer.tsx"
      ]
    }
  },
  "pkg": {}
});