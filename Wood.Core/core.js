
(function () {
    var path = 'http://127.0.0.1:1711/';
    var platform = 'android';
    function createStandardXHR() {
        try {
            return new window.XMLHttpRequest();
        } catch (e) { }
    }
    function createActiveXHR() {
        try {
            return new window.ActiveXObject('Microsoft.XMLHTTP');
        } catch (e) { }
    }
    function PostRequest(data, hasReturn) {
        try {
            var xhr = window.ActiveXObject ? function () {
                return createStandardXHR() || createActiveXHR();
            } : createStandardXHR();

            var url = path + '?_=' + Math.random();
            xhr.open('POST', url, false);
            xhr.send(JSON.stringify(data));

            if (xhr.status == 200) {
                if (hasReturn)
                    return JSON.parse(xhr.responseText);

            } else {
                alert(xhr.statusText);
            }
        }
        catch (err) {

        }
    }
    function GenerateCallBackname() {
        return 'callback_' + Math.random().toString().replace('.', '') + '_' + new Date().getTime();
    }
    var Wood = {
        Callbacks: {},
        RegisterService: function (serviceName) {
            return new service(serviceName);
        },
        ExecuteCallback: function (callbackName, result) {
            this.Callbacks[callbackName](result);
        },
        InvokeService: function (service, method, paramsArray, hasReturn) {

            var paramObj = {
                Service: service,
                Method: method,
                RequireReturn: hasReturn,
                Parms: {}
            };
            var last = paramsArray[paramsArray.length - 1];
            if (last != undefined && typeof last === 'function') {
                var cbname = GenerateCallBackname();
                this.Callbacks[cbname] = last;
                paramObj.CallbackName = cbname;
                paramObj.Parms['cb'] = cbname;
                paramsArray.pop();
            }
            for (var i = 0; i < paramsArray.length; i++) {
                paramObj.Parms['p' + i] = paramsArray[i];
            }
            if (hasReturn) {
                return PostRequest(paramObj, hasReturn);
            }
            else {
                PostRequest(paramObj, hasReturn);
            }

        },
        OnPlatform: function (android, ios, wp) {
            switch (platform) {
                case 'android':
                    {
                        if (android && typeof android === 'function') android();
                        break;
                    }
                case 'ios':
                    {
                        if (ios && typeof ios === 'function') ios();
                        break;
                    }
                case 'wp':
                    {
                        if (wp && typeof wp === 'function') wp();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    };
    var service = function (serviceName) {
        Wood[serviceName] = this;
        this.serviceName = serviceName;
        this.RegisterMethod = function (method, argsNameArray, hasReturn) {
            function createFunc() {
                var arglist = '';
                var params = '';
                for (var i = 0; i < argsNameArray.length; i++) {
                    arglist += ',\'' + argsNameArray[i] + '\'';
                    params += ',' + argsNameArray[i];
                }
                if (arglist.length > 0) {
                    params = params.substr(1);
                    arglist = arglist.substr(1);
                }
                if (arglist.length > 0) {
                    arglist += ',\'callback\'';
                    params += ',callback';
                } else {
                    arglist += '\'callback\'';
                    params += 'callback';
                }
                var body = '';
                if (!hasReturn) {
                    body = '\'Wood.InvokeService(this.serviceName,\\\'' + method + '\\\', [' + params + '],false)\'';
                } else {
                    body = '\'return Wood.InvokeService(this.serviceName,\\\'' + method + '\\\',[' + params + '],true)\'';
                }
                var functionStr;
                functionStr = 'new Function(' + arglist + ',' + body + ')';
                return eval(functionStr);
            }

            this[method] = createFunc();
            return this;
        };
    };
    window.Wood = window.Wood || Wood;
})();

(function () {
    Wood.RegisterService('Toast')
    .RegisterMethod('showLong', ['text'])
    .RegisterMethod('showShort', ['text']);

    Wood.RegisterService('Vibrator')
        .RegisterMethod('vibrate', ['milliseconds'])
        .RegisterMethod('vibrate', ['pattern', 'repeat'])
        .RegisterMethod('cancel', []);

    Wood.RegisterService('Location')
        .RegisterMethod('currentPos', [], false)
        .RegisterMethod('getPos', [], true)
        .RegisterMethod('watch', [], true)
        .RegisterMethod('unwatch', []);

})();
