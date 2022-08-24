import {Promise} from "es6-promise";


const jumpTo = (to: string, action: any, ctx?: object) => {
    console.log('jumpTo to, action, ctx: ', to, action, ctx);
    window.location.href = to;
}



const AmisEnv = {
    // fetcher: umiRequest as any,
    fetcher: async (config: any) => {
        console.log(config)
        if (config === false) {
            return await  Promise.reject(null)
        }
        return await config
    },
    // tracker: this.handleTrace,
    jumpTo,
    // adaptor: (payload: any, response: any, api: any) => {
    //     if (api.redirect) {
    //         jumpTo(api.redirect)
    //     }
    //     return response
    // },
    // isCancel: (value: any) => (axios as any).isCancel(value),
    // eslint-disable-next-line @typescript-eslint/no-unused-vars

    // copy: (contents: string, options: any = {}) => {
    //     const ret = copy(contents, options);
    //     // eslint-disable-next-line @typescript-eslint/no-unused-expressions
    //     ret && (!options || options.shutup !== true) && toast.info('内容已拷贝到剪切板');
    //     return ret;
    // },
    enableAMISDebug: true,
    locale: 'zh-CN'
}
