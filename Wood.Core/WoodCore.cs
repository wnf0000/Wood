using System;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Wood.Core
{
    public class WoodCore
    {
        static HttpListener HttpListener;
        static readonly string BasePrefix;
        IWebView webView;
        string Addr;
        string FullPrefix;
        static int Port;

        static WoodCore()
        {
            HttpListener = new HttpListener();
            int port = 8850;
            while (port < 8899)
            {
                BasePrefix = "http://*:" + port + "/";
                try
                {
                    HttpListener.Prefixes.Add(BasePrefix);
                    HttpListener.Prefixes.Remove(BasePrefix);
                    Port = port;
                    break;
                }
                catch
                {
                    port++;
                }
            }
            if (port >= 8870)
            {
                throw new Exception("端口8850-8899都被占用了！");
            }
        }
        public WoodCore(IWebView webView)
        {
            Addr = Guid.NewGuid().ToString("N");
            FullPrefix = BasePrefix + Addr + "/";
            this.webView = webView;
            HttpListener.Prefixes.Add(FullPrefix);
            Start();
        }
        public void Start()
        {
            if (!HttpListener.IsListening)
            {
                HttpListener.Start();
                HttpListener.BeginGetContext(new AsyncCallback(Callback), HttpListener);
            }

        }
        void Callback(IAsyncResult result)
        {
            var context = HttpListener.EndGetContext(result);
            HttpListener.BeginGetContext(new AsyncCallback(Callback), HttpListener);
            //权限判断
            //-----
            //获取参数
            var args = FilterArgs(context);
            //调用服务
            InvokeService(context, args);
        }
        ServiceArgs FilterArgs(HttpListenerContext context)
        {
            StreamReader sr = new StreamReader(context.Request.InputStream);
            var content = sr.ReadToEnd();
            var obj = JsonConvert.DeserializeObject<ServiceArgs>(content);
            return obj;
        }
        public void InvokeCallback(string callback, object data)
        {
            var json = data.ToJsonString();
            var script = string.Format("var arg={0};Wood.ExecuteCallback('{1}',arg);", json, callback);
            webView.ExecuteScript(script);
        }
        void InvokeService(HttpListenerContext context, ServiceArgs args)
        {
            try
            {
                var service = ServiceManager.GetService(args.Service);
                if (args.RequireReturn)
                {
                    var result = service.ExecuteHasReturn(this, args);
                    ReturnContentToClient(context, result);
                }
                else
                {
                    service.Execute(this, args);
                    ReturnContentToClient(context);
                }

            }
            catch (Exception exp)
            {
                ReturnErrorToClient(context, exp.Message);
            }
        }
        void ReturnContentToClient(HttpListenerContext context, object result=null)
        {
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "text/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (result != null)
            {
                var responseBytes = System.Text.Encoding.UTF8.GetBytes(result.ToJsonString());
                context.Response.ContentLength64 = responseBytes.Length;
                context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            }

            context.Response.OutputStream.Close();
        }
        void ReturnErrorToClient(HttpListenerContext context,string error)
        {
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusDescription = error;
            context.Response.OutputStream.Close();
        }
        public string CoreScript
        {
            get
            {


                var _coreScript = "(function () {" + string.Format("    var path = 'http://127.0.0.1:{0}/{1}/\';", Port, Addr);
#if ANDROID
                _coreScript += " var platform = 'android';";
#endif
#if IOS
                 _coreScript += " var platform = 'ios';";
#endif
#if WP
                 _coreScript += " var platform = 'wp';";
#endif
                _coreScript +=
@"
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
                if(hasReturn)
                return JSON.parse(xhr.responseText);

            } else {
                alert(xhr.statusText);
            }
        }
        catch (err) {
            
        }
    }

    function GenerateCallBackname() {
        return 'callback_' + Math.random().toString().replace('.', '') + '_' + new Date().getTime() ;
    }
    var Wood = {
        Callbacks :{},
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

"
#if ANDROID
+
@"
function FastClick(n,t){'use strict';function i(n,t){return function(){return n.apply(t,arguments)}}var r;(t=t||{},this.trackingClick=!1,this.trackingClickStart=0,this.targetElement=null,this.touchStartX=0,this.touchStartY=0,this.lastTouchIdentifier=0,this.touchBoundary=t.touchBoundary||10,this.layer=n,this.tapDelay=t.tapDelay||100,FastClick.notNeeded(n))||(deviceIsAndroid&&(n.addEventListener('mouseover',i(this.onMouse,this),!0),n.addEventListener('mousedown',i(this.onMouse,this),!0),n.addEventListener('mouseup',i(this.onMouse,this),!0)),n.addEventListener('click',i(this.onClick,this),!0),n.addEventListener('touchstart',i(this.onTouchStart,this),!1),n.addEventListener('touchmove',i(this.onTouchMove,this),!1),n.addEventListener('touchend',i(this.onTouchEnd,this),!1),n.addEventListener('touchcancel',i(this.onTouchCancel,this),!1),Event.prototype.stopImmediatePropagation||(n.removeEventListener=function(t,i,r){var u=Node.prototype.removeEventListener;t==='click'?u.call(n,t,i.hijacked||i,r):u.call(n,t,i,r)},n.addEventListener=function(t,i,r){var u=Node.prototype.addEventListener;t==='click'?u.call(n,t,i.hijacked||(i.hijacked=function(n){n.propagationStopped||i(n)}),r):u.call(n,t,i,r)}),typeof n.onclick=='function'&&(r=n.onclick,n.addEventListener('click',function(n){r(n)},!1),n.onclick=null))}var deviceIsAndroid=navigator.userAgent.indexOf('Android')>0,deviceIsIOS=/iP(ad|hone|od)/.test(navigator.userAgent),deviceIsIOS4=deviceIsIOS&&/OS 4_\d(_\d)?/.test(navigator.userAgent),deviceIsIOSWithBadTarget=deviceIsIOS&&/OS ([6-9]|\d{2})_\d/.test(navigator.userAgent);FastClick.prototype.needsClick=function(n){'use strict';switch(n.nodeName.toLowerCase()){case'button':case'select':case'textarea':if(n.disabled)return!0;break;case'input':if(deviceIsIOS&&n.type==='file'||n.disabled)return!0;break;case'label':case'video':return!0}return/\bneedsclick\b/.test(n.className)};FastClick.prototype.needsFocus=function(n){'use strict';switch(n.nodeName.toLowerCase()){case'textarea':return!0;case'select':return!deviceIsAndroid;case'input':switch(n.type){case'button':case'checkbox':case'file':case'image':case'radio':case'submit':return!1}return!n.disabled&&!n.readOnly;default:return/\bneedsfocus\b/.test(n.className)}};FastClick.prototype.sendClick=function(n,t){'use strict';var r,i;document.activeElement&&document.activeElement!==n&&document.activeElement.blur();i=t.changedTouches[0];r=document.createEvent('MouseEvents');r.initMouseEvent(this.determineEventType(n),!0,!0,window,1,i.screenX,i.screenY,i.clientX,i.clientY,!1,!1,!1,!1,0,null);r.forwardedTouchEvent=!0;n.dispatchEvent(r)};FastClick.prototype.determineEventType=function(n){'use strict';return deviceIsAndroid&&n.tagName.toLowerCase()==='select'?'mousedown':'click'};FastClick.prototype.focus=function(n){'use strict';var t;deviceIsIOS&&n.setSelectionRange&&n.type.indexOf('date')!==0&&n.type!=='time'?(t=n.value.length,n.setSelectionRange(t,t)):n.focus()};FastClick.prototype.updateScrollParent=function(n){'use strict';var i,t;if(i=n.fastClickScrollParent,!i||!i.contains(n)){t=n;do{if(t.scrollHeight>t.offsetHeight){i=t;n.fastClickScrollParent=t;break}t=t.parentElement}while(t)}i&&(i.fastClickLastScrollTop=i.scrollTop)};FastClick.prototype.getTargetElementFromEventTarget=function(n){'use strict';return n.nodeType===Node.TEXT_NODE?n.parentNode:n};FastClick.prototype.onTouchStart=function(n){'use strict';var i,t,r;if(n.targetTouches.length>1)return!0;if(i=this.getTargetElementFromEventTarget(n.target),t=n.targetTouches[0],deviceIsIOS){if(r=window.getSelection(),r.rangeCount&&!r.isCollapsed)return!0;if(!deviceIsIOS4){if(t.identifier===this.lastTouchIdentifier)return n.preventDefault(),!1;this.lastTouchIdentifier=t.identifier;this.updateScrollParent(i)}}return this.trackingClick=!0,this.trackingClickStart=n.timeStamp,this.targetElement=i,this.touchStartX=t.pageX,this.touchStartY=t.pageY,n.timeStamp-this.lastClickTime<this.tapDelay&&n.preventDefault(),!0};FastClick.prototype.touchHasMoved=function(n){'use strict';var t=n.changedTouches[0],i=this.touchBoundary;return Math.abs(t.pageX-this.touchStartX)>i||Math.abs(t.pageY-this.touchStartY)>i?!0:!1};FastClick.prototype.onTouchMove=function(n){'use strict';return this.trackingClick?((this.targetElement!==this.getTargetElementFromEventTarget(n.target)||this.touchHasMoved(n))&&(this.trackingClick=!1,this.targetElement=null),!0):!0};FastClick.prototype.findControl=function(n){'use strict';return n.control!==undefined?n.control:n.htmlFor?document.getElementById(n.htmlFor):n.querySelector('button, input:not([type=hidden]), keygen, meter, output, progress, select, textarea')};FastClick.prototype.onTouchEnd=function(n){'use strict';var u,e,i,r,f,t=this.targetElement;if(!this.trackingClick)return!0;if(n.timeStamp-this.lastClickTime<this.tapDelay)return this.cancelNextClick=!0,!0;if(this.cancelNextClick=!1,this.lastClickTime=n.timeStamp,e=this.trackingClickStart,this.trackingClick=!1,this.trackingClickStart=0,deviceIsIOSWithBadTarget&&(f=n.changedTouches[0],t=document.elementFromPoint(f.pageX-window.pageXOffset,f.pageY-window.pageYOffset)||t,t.fastClickScrollParent=this.targetElement.fastClickScrollParent),i=t.tagName.toLowerCase(),i==='label'){if(u=this.findControl(t),u){if(this.focus(t),deviceIsAndroid)return!1;t=u}}else if(this.needsFocus(t))return n.timeStamp-e>100||deviceIsIOS&&window.top!==window&&i==='input'?(this.targetElement=null,!1):(this.focus(t),this.sendClick(t,n),deviceIsIOS4&&i==='select'||(this.targetElement=null,n.preventDefault()),!1);return deviceIsIOS&&!deviceIsIOS4&&(r=t.fastClickScrollParent,r&&r.fastClickLastScrollTop!==r.scrollTop)?!0:(this.needsClick(t)||(n.preventDefault(),this.sendClick(t,n)),!1)};FastClick.prototype.onTouchCancel=function(){'use strict';this.trackingClick=!1;this.targetElement=null};FastClick.prototype.onMouse=function(n){'use strict';return this.targetElement?n.forwardedTouchEvent?!0:n.cancelable?!this.needsClick(this.targetElement)||this.cancelNextClick?(n.stopImmediatePropagation?n.stopImmediatePropagation():n.propagationStopped=!0,n.stopPropagation(),n.preventDefault(),!1):!0:!0:!0};FastClick.prototype.onClick=function(n){'use strict';var t;return this.trackingClick?(this.targetElement=null,this.trackingClick=!1,!0):n.target.type==='submit'&&n.detail===0?!0:(t=this.onMouse(n),t||(this.targetElement=null),t)};FastClick.prototype.destroy=function(){'use strict';var n=this.layer;deviceIsAndroid&&(n.removeEventListener('mouseover',this.onMouse,!0),n.removeEventListener('mousedown',this.onMouse,!0),n.removeEventListener('mouseup',this.onMouse,!0));n.removeEventListener('click',this.onClick,!0);n.removeEventListener('touchstart',this.onTouchStart,!1);n.removeEventListener('touchmove',this.onTouchMove,!1);n.removeEventListener('touchend',this.onTouchEnd,!1);n.removeEventListener('touchcancel',this.onTouchCancel,!1)};FastClick.notNeeded=function(n){'use strict';var t,i;if(typeof ontouchstart=='undefined')return!0;if(i=+(/Chrome\/([0-9]+)/.exec(navigator.userAgent)||[,0])[1],i)if(deviceIsAndroid){if(t=document.querySelector('meta[name=viewport]'),t&&(t.content.indexOf('user-scalable=no')!==-1||i>31&&window.innerWidth<=window.screen.width))return!0}else return!0;return n.style.msTouchAction==='none'?!0:!1};FastClick.attach=function(n,t){'use strict';return new FastClick(n,t)};typeof define!='undefined'&&define.amd?define(function(){'use strict';return FastClick}):typeof module!='undefined'&&module.exports?(module.exports=FastClick.attach,module.exports.FastClick=FastClick):window.FastClick=FastClick;window.addEventListener('load',function(){FastClick.attach(document.body)},!1);FastClick.attach(document.body);
"
#endif
;


                return _coreScript;
            }
        }
    }

}
