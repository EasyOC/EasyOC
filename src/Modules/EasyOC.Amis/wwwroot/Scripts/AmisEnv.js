/*import axios from 'axios';*/


const buildEnv = (axios, fetcherFn) => {
    var env = {

        // tracker: this.handleTrace,
        jumpTo: (to, action, ctx) => {
            console.log('jumpTo to, action, ctx: ', to, action, ctx);
            window.location.href = to;
        },
        // adaptor: (payload: any, response: any, api: any) => {
        //     if (api.redirect) {
        //         jumpTo(api.redirect)
        //     }
        //     return response
        // },
        isCancel: (value) => (axios).isCancel(value),
        // eslint-disable-next-line @typescript-eslint/no-unused-vars

        // copy: (contents: string, options: any = {}) => {
        //     const ret = copy(contents, options);
        //     // eslint-disable-next-line @typescript-eslint/no-unused-expressions
        //     ret && (!options || options.shutup !== true) && toast.info('内容已拷贝到剪切板');
        //     return ret;
        // },
        enableAMISDebug: false,
        locale: 'zh-CN'
    }

    if (fetcherFn) {
        env.fetcher = fetcherFn
    } else {
        env.fetcher = async api => {
            console.log("config", api)
            if (api === false) {
                return Promise.reject();
            }
            var result = await axios(api);
            console.log("result", result)
            if (result.data.msg) {
                 var firstmsg = result.data.msg 
                if(result.data.msg instanceof Array){
                   firstmsg= result.data.msg[0]
                }               
                result.data.msg = firstmsg.message.value
                result.data.status = firstmsg.type
            } else if (!result.data.status) {
                result.data.status = result.status == 200 ? 0 : result.status
            }
            return result.data;
           
        }
    }

    return env;
}

