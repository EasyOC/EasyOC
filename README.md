EasyOC 是一个基于OrchardCore 的SPA 开发框架

它在采用无入侵的方式基于Orchard Core CMS 增加了一系列模块用于提供 SPA服务 ，参考 ABP 的功能设计实现了一些用于 SPA 的特性和功能

EasyOC is a SPA development framework based on OrchardCore

It adds a series of modules based on Orchard Core CMS to provide SPA services in a non-invasive manner, and implements some features and functions specifically for SPA by referring to the functional design of ABP

前端项目：https://github.com/EasyOC/EasyOC.VbenAdmin

QQ群：877196442

## Features


### For SPA Service Application

- [X] GraphQL lucene queries can support Total and items
- [X] wrapper API returns the result, You can use the Restful API to query OrchardCore for notifications added using `INotify`
- [X] SwaggerUI
- [X] [Dynamic Web API](https://github.com/EasyOC/EasyOC/blob/master/src/Modules/EasyOC.OrchardCore.OpenApi/Services/Users/UsersAppService.cs) 
- [x] [Authorization Attribute](https://github.com/EasyOC/EasyOC/blob/master/src/Core/EasyOC.Core/Authorization/Attributes/EOCAuthorizationAttribute.cs)
- [X] Users And Roles Api


### For OrchardCore CMS
- [X] Generate ContentType From RDBMS
- [X] Intergration [FreeSQL](https://github.com/dotnetcore/FreeSql) to current session (read current shell database)
- [X] [FreeSQL](https://github.com/dotnetcore/FreeSql) external database connection. With FreeSQL, you can use [any of the 16 databases supported by FreeSQL](http://www.freesql.net/guide/type-mapping.html#%E5%88%A0%E9%99%A4) in OC
- [X] [Workflow Error Handle Event](#拦截所有工作流异常)
- [X] [Read Excel Workflow Task](https://github.com/EasyOC/EasyOC/issues/1) , you can read excel data from Upload or from local/network path
- [X] [SQL Task ](#数据同步)
- [X] [Create ContentItem Helper](#generate-contentmapping-with-rdbms)
- [X] Powershell Task
- [X] [User Profile](https://github.com/EasyOC/EasyOC/blob/master/src/Modules/EasyOC.OrchardCore.OpenApi/Handlers/UserEventHandler.cs) , SQL indexes that implement CustomUserSettings Indexable
- [X] WeChat Authentication , Thanks to [@PimHwang](https://github.com/PimHwang/OrchardCore.Community)



### install

![image](https://user-images.githubusercontent.com/15613121/146934540-f079cf5a-d462-4458-9ee4-5dd76960e265.png)

### Generate ContentType form RDBMS

![image](https://user-images.githubusercontent.com/15613121/146937454-2c890612-4432-4557-82c1-a9795b183782.png)

![RDBMS](https://user-images.githubusercontent.com/15613121/146941715-9d2c9a33-85b3-437b-a61a-0575dee6e0d0.gif)




### 数据同步
![image](https://user-images.githubusercontent.com/15613121/146942701-38d9107e-bab9-4b62-be29-5fc89b43ace1.png)

注意！尽量不要一次更新太多数据，使Document表被长时间锁死，导致其他页面无法访问

#### Generate ContentMapping with RDBMS
![image](https://user-images.githubusercontent.com/15613121/146943163-507150d8-e0a3-48a2-9d32-0ed52dbf1caa.png)

可以在Management Studio 中选中某条数据，然后右键选择“连同标题一起复制”  使用 https://www.bejson.com/json/col2json/ 之类的工具将Excel 转换为json
复制结果到内容模板中

![image](https://user-images.githubusercontent.com/15613121/146943754-954829f4-9a82-4134-b702-0698b2d043f1.png)


### 拦截所有工作流异常 

虽然代码中不会拦截触发的节点，但尽量不要在异常处理工作流中再次引发异常，不然可能会造成死循环
如果要触发当前工作流，如果表达式成立，则将触发工作流，捕获全局异常，直接返回true

![image](https://user-images.githubusercontent.com/15613121/146947303-a3a231b6-500f-43e6-b7c5-2ba5bcc8b240.png)

获取错误信息： input('ErrorInfo')
包含如下属性：

- WorkflowId：工作流Id
- WorkflowName：工作流名称
- ExcutedActivityCount：已执行节点数量
- ActivityDisplayName：节点名称
- ActivityTypeName：节点类型名称
- ActivityId：节点Id
- ErrorMessage：异常信息概要
- ExceptionDetails：异常详细信息
- FaultMessage：包含工作流节点手动引发的错误信息
