// http://paulirish.com/2011/requestanimationframe-for-smart-animating/
// http://my.opera.com/emoller/blog/2011/12/20/requestanimationframe-for-smart-er-animating

// requestAnimationFrame polyfill by Erik Möller. fixes from Paul Irish and Tino Zijdel

// MIT license

(function() {
    var lastTime = 0;
    var vendors = ['ms', 'moz', 'webkit', 'o'];
    for(var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
        window.requestAnimationFrame = window[vendors[x]+'RequestAnimationFrame'];
        window.cancelAnimationFrame = window[vendors[x]+'CancelAnimationFrame']
                                   || window[vendors[x]+'CancelRequestAnimationFrame'];
    }

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = function(callback, element) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function() { callback(currTime + timeToCall); },
              timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        };

    if (!window.cancelAnimationFrame)
        window.cancelAnimationFrame = function(id) {
            clearTimeout(id);
        };
}());



/*! npm.im/iphone-inline-video 2.0.2 */

var enableInlineVideo=function(){"use strict";/*! npm.im/intervalometer */
function e(e,i,n,r){function t(n){d=i(t,r),e(n-(a||n)),a=n}var d,a;return{start:function(){d||t(0)},stop:function(){n(d),d=null,a=0}}}function i(i){return e(i,requestAnimationFrame,cancelAnimationFrame)}function n(e,i,n,r){function t(i){Boolean(e[n])===Boolean(r)&&i.stopImmediatePropagation(),delete e[n]}return e.addEventListener(i,t,!1),t}function r(e,i,n,r){function t(){return n[i]}function d(e){n[i]=e}r&&d(e[i]),Object.defineProperty(e,i,{get:t,set:d})}function t(e,i,n){n.addEventListener(i,function(){return e.dispatchEvent(new Event(i))})}function d(e,i){Promise.resolve().then(function(){e.dispatchEvent(new Event(i))})}function a(e){var i=new Audio;return t(e,"play",i),t(e,"playing",i),t(e,"pause",i),i.crossOrigin=e.crossOrigin,i.src=e.src||e.currentSrc||"data:",i}function o(e,i,n){(m||0)+200<Date.now()&&(e[b]=!0,m=Date.now()),n||(e.currentTime=i),w[++T%3]=100*i|0}function u(e){return e.driver.currentTime>=e.video.duration}function s(e){var i=this;i.video.readyState>=i.video.HAVE_FUTURE_DATA?(i.hasAudio||(i.driver.currentTime=i.video.currentTime+e*i.video.playbackRate/1e3,i.video.loop&&u(i)&&(i.driver.currentTime=0)),o(i.video,i.driver.currentTime)):i.video.networkState===i.video.NETWORK_IDLE&&0===i.video.buffered.length&&i.video.load(),i.video.ended&&(delete i.video[b],i.video.pause(!0))}function c(){var e=this,i=e[h];return e.webkitDisplayingFullscreen?void e[g]():("data:"!==i.driver.src&&i.driver.src!==e.src&&(o(e,0,!0),i.driver.src=e.src),void(e.paused&&(i.paused=!1,0===e.buffered.length&&e.load(),i.driver.play(),i.updater.start(),i.hasAudio||(d(e,"play"),i.video.readyState>=i.video.HAVE_ENOUGH_DATA&&d(e,"playing")))))}function v(e){var i=this,n=i[h];n.driver.pause(),n.updater.stop(),i.webkitDisplayingFullscreen&&i[E](),n.paused&&!e||(n.paused=!0,n.hasAudio||d(i,"pause"),i.ended&&(i[b]=!0,d(i,"ended")))}function p(e,n){var r=e[h]={};r.paused=!0,r.hasAudio=n,r.video=e,r.updater=i(s.bind(r)),n?r.driver=a(e):(e.addEventListener("canplay",function(){e.paused||d(e,"playing")}),r.driver={src:e.src||e.currentSrc||"data:",muted:!0,paused:!0,pause:function(){r.driver.paused=!0},play:function(){r.driver.paused=!1,u(r)&&o(e,0)},get ended(){return u(r)}}),e.addEventListener("emptied",function(){var i=!r.driver.src||"data:"===r.driver.src;r.driver.src&&r.driver.src!==e.src&&(o(e,0,!0),r.driver.src=e.src,i?r.driver.play():r.updater.stop())},!1),e.addEventListener("webkitbeginfullscreen",function(){e.paused?n&&0===r.driver.buffered.length&&r.driver.load():(e.pause(),e[g]())}),n&&(e.addEventListener("webkitendfullscreen",function(){r.driver.currentTime=e.currentTime}),e.addEventListener("seeking",function(){w.indexOf(100*e.currentTime|0)<0&&(r.driver.currentTime=e.currentTime)}))}function l(e){var i=e[h];e[g]=e.play,e[E]=e.pause,e.play=c,e.pause=v,r(e,"paused",i.driver),r(e,"muted",i.driver,!0),r(e,"playbackRate",i.driver,!0),r(e,"ended",i.driver),r(e,"loop",i.driver,!0),n(e,"seeking"),n(e,"seeked"),n(e,"timeupdate",b,!1),n(e,"ended",b,!1)}function f(e,i){if(void 0===i&&(i={}),!e[h]){if(!i.everywhere){if(!y)return;if(!(i.iPad||i.ipad?/iPhone|iPod|iPad/:/iPhone|iPod/).test(navigator.userAgent))return}!e.paused&&e.webkitDisplayingFullscreen&&e.pause(),p(e,!e.muted),l(e),e.classList.add("IIV"),e.muted&&e.autoplay&&e.play(),/iPhone|iPod|iPad/.test(navigator.platform)||console.warn("iphone-inline-video is not guaranteed to work in emulated environments")}}var m,y="object"==typeof document&&"object-fit"in document.head.style&&!matchMedia("(-webkit-video-playable-inline)").matches,h="bfred-it:iphone-inline-video",b="bfred-it:iphone-inline-video:event",g="bfred-it:iphone-inline-video:nativeplay",E="bfred-it:iphone-inline-video:nativepause",w=[],T=0;return f}();


(function() {
  var ua = navigator.userAgent.toLowerCase();
  var browser = {
    opera: (/opera/i.test(ua) || /opr/i.test(ua)),
    msie: (/msie/i.test(ua) && !/opera/i.test(ua) || /trident\//i.test(ua)) || /edge/i.test(ua),
    msie_edge: (/edge/i.test(ua) && !/opera/i.test(ua)),
    mozilla: /firefox/i.test(ua),
    chrome: /chrome/i.test(ua) && !/edge/i.test(ua),
    safari: (!(/chrome/i.test(ua)) && /webkit|safari|khtml/i.test(ua)),
    iphone: /iphone/i.test(ua),
    ipod: /ipod/i.test(ua),
    ipad: /ipad/i.test(ua),
    android: /android/i.test(ua),
    mobile: /iphone|ipod|ipad|opera mini|opera mobi|iemobile|android/i.test(ua),
    safari_mobile: /iphone|ipod|ipad/i.test(ua),
    opera_mobile: /opera mini|opera mobi/i.test(ua),
    opera_mini: /opera mini/i.test(ua),
    mac: /mac/i.test(ua),
  };
  var TBaseUrl = window.TBaseUrl || '//telegram.org/';

  function addEvent(el, event, handler) {
    var events = event.split(/\s+/);
    for (var i = 0; i < events.length; i++) {
      if (el.addEventListener) {
        el.addEventListener(events[i], handler, false);
      } else {
        el.attachEvent('on' + events[i], handler);
      }
    }
  }

  function removeEvent(el, event, handler) {
    var events = event.split(/\s+/);
    for (var i = 0; i < events.length; i++) {
      if (el.removeEventListener) {
        el.removeEventListener(events[i], handler);
      } else {
        el.detachEvent('on' + events[i], handler);
      }
    }
  }

  function getXHR() {
    if (navigator.appName == "Microsoft Internet Explorer"){
      return new ActiveXObject("Microsoft.XMLHTTP");
    } else {
      return new XMLHttpRequest();
    }
  }

  function getParentByClass(el, className) {
    while (el = el.parentNode) {
      if (el.classList.contains(className)) break;
    }
    return el;
  }

  function formatDate(datetime) {
    var date = new Date(datetime);
    var cur_date = new Date();
    var j = date.getDate();
    var M = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][date.getMonth()];
    var Y = date.getFullYear();
    var H = date.getHours();
    if (H < 10) H = '0' + H;
    var i = date.getMinutes();
    if (i < 10) i = '0' + i;
    if (cur_date.getFullYear() == date.getFullYear() ||
        (cur_date.getTime() - date.getTime()) < 90 * 86400000) {
      return M + ' ' + j + ' at ' + H + ':' + i;
    }
    return M + ' ' + j + ', ' + Y + ' at ' + H + ':' + i;
  }

  function formatDuration(duration) {
    duration = Math.floor(duration);
    duration = Math.max(0, duration);
    var duration_str = '';
    if (duration >= 3600) {
      var hours = Math.floor(duration / 3600);
      duration_str += hours + ':';
      var minutes = Math.floor((duration % 3600) / 60);
      if (minutes < 10) minutes = '0' + minutes;
    } else {
      var minutes = Math.floor(duration / 60);
    }
    duration_str += minutes + ':';
    var seconds = duration % 60;
    if (seconds < 10) seconds = '0' + seconds;
    duration_str += seconds;
    return duration_str;
  }

  function doesSupportEmoji() {
    var context, smile, canvas = document.createElement('canvas');
    if (!canvas.getContext) return false;
    context = canvas.getContext('2d');
    if (typeof context.fillText != 'function') return false;
    smile = '#' + String.fromCharCode(65039) + String.fromCharCode(8419);
    context.textBaseline = 'top';
    context.font = '32px Arial';
    context.fillText(smile, 0, 0);
    if (context.getImageData(16, 16, 1, 1).data[0] === 0) return false;
    var div = document.createElement('div');
    div.style.position = 'absolute';
    div.style.overflow = 'hidden';
    div.style.top = '-1000px';
    var span = document.createElement('span');
    div.style.fontSize = '16px';
    span.innerHTML = smile;
    div.appendChild(span);
    document.body.insertBefore(div, document.body.firstChild);
    var width = span.offsetWidth;
    document.body.removeChild(div);
    if (width < 18) return false;
    return true;
  }

  function sendPostMessage(data, origin) {
    try {
      window.parent.postMessage(JSON.stringify(data), origin || '*');
    } catch(e) {
      if (origin) alert('Bot domain invalid');
    }
  }

  var webpNativeSupport = null, webpFallbackSupport = null, webpImage = null, callbacks = [];
  function applyCallbacks() {
    for (var i = 0; i < callbacks.length; i++) {
      callbacks[i](webpNativeSupport, webpFallbackSupport);
    }
  }
  function doesSupportWebp(callback) {
    if (webpFallbackSupport !== null) {
      callback(webpNativeSupport, webpFallbackSupport);
    } else {
      callbacks.push(callback);
      if (!webpImage) {
        webpImage = new Image();
        webpImage.onerror = webpImage.onload = function() {
          if (this.width === 2 && this.height === 1) {
            webpNativeSupport = true;
            webpFallbackSupport = false;
            applyCallbacks();
          } else {
            webpNativeSupport = false;
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.async = true;
            script.src = TBaseUrl + 'js/libwebp-0.2.0.js';
            script.onerror = function() {
              webpFallbackSupport = false;
              applyCallbacks();
            }
            script.onload = function() {
              webpFallbackSupport = true;
              applyCallbacks();
            }
            var head = document.getElementsByTagName('head')[0];
            head.appendChild(script);
          }
        }
        webpImage.src = 'data:image/webp;base64,UklGRjIAAABXRUJQVlA4ICYAAACyAgCdASoCAAEALmk0mk0iIiIiIgBoSygABc6zbAAA/v56QAAAAA==';
      }
    }
  }

  function getPngDataUrlFromWebp(data) {
    var decoder = new WebPDecoder();
    var config = decoder.WebPDecoderConfig;
    var buffer = config.j || config.output;
    var bitstream = config.input;
    if (!decoder.WebPInitDecoderConfig(config)) {
      throw new Error('[webpjs] Library version mismatch!');
    }
    var StatusCode = decoder.VP8StatusCode;
    status = decoder.WebPGetFeatures(data, data.length, bitstream);
    if (status != (StatusCode.VP8_STATUS_OK || 0)) {
      throw new Error('[webpjs] status error');
    }
    var mode = decoder.WEBP_CSP_MODE;
    buffer.colorspace = mode.MODE_RGBA;
    buffer.J = 4;
    try {
      status = decoder.WebPDecode(data, data.length, config);
    } catch (e) {
      status = e
    }
    var ok = (status == 0);
    if (!ok) {
      throw new Error('[webpjs] decoding failed');
    }
    var bitmap = buffer.c.RGBA.ma;
    if (!bitmap) {
      throw new Error('[webpjs] bitmap error');
    }
    var biHeight = buffer.height;
    var biWidth = buffer.width;
    var canvas = document.createElement('canvas');
    var context = canvas.getContext('2d');
    canvas.height = biHeight;
    canvas.width = biWidth;
    var output = context.createImageData(canvas.width, canvas.height);
    var outputData = output.data;
    for (var h = 0; h < biHeight; h++) {
      for (var w = 0; w < biWidth; w++) {
        outputData[0 + w * 4 + (biWidth * 4) * h] = bitmap[1 + w * 4 + (biWidth * 4) * h];
        outputData[1 + w * 4 + (biWidth * 4) * h] = bitmap[2 + w * 4 + (biWidth * 4) * h];
        outputData[2 + w * 4 + (biWidth * 4) * h] = bitmap[3 + w * 4 + (biWidth * 4) * h];
        outputData[3 + w * 4 + (biWidth * 4) * h] = bitmap[0 + w * 4 + (biWidth * 4) * h];
      }
    }
    context.putImageData(output, 0, 0);
    return canvas.toDataURL('image/png');
  }

  function proccessWebpImage(id, failed_callback, success_callback) {
    var el = document.getElementById(id);
    if (!el) return;
    failed_callback = failed_callback || function(){};
    success_callback = success_callback || function(){};
    doesSupportWebp(function(nativeSupport, fallbackSupport) {
      if (nativeSupport) {
        success_callback();
        return;
      } else if (!fallbackSupport) {
        failed_callback();
        return;
      }
      var isImage, src;
      if (el.tagName && el.tagName.toUpperCase() == 'IMG' && el.src) {
        isImage = true;
        src = el.src;
      } else {
        isImage = false;
        var bgImage;
        if (window.getComputedStyle) {
          bgImage = window.getComputedStyle(el).backgroundImage;
        } else {
          bgImage = el.style && el.style.backgroundImage;
        }
        src = bgImage.slice(4, -1).replace(/["|']/g, '');
      }
      if (!src) {
        failed_callback();
        return;
      }
      var xhr = getXHR();
      xhr.open('get', src);
      if (xhr.overrideMimeType) {
        xhr.overrideMimeType('text/plain; charset=x-user-defined');
      } else {
        xhr.setRequestHeader('Accept-Charset', 'x-user-defined');
      }
      xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
          if (typeof xhr.responseBody == 'undefined' && xhr.responseText) {
            var rlen = xhr.responseText.length, uarr = new Uint8Array(rlen);
            for (var i = 0; i < rlen; i++) {
              uarr[i] = xhr.responseText.charCodeAt(i);
            }
            try {
              var src = getPngDataUrlFromWebp(uarr);
              if (isImage) {
                el.src = src;
              } else {
                el.style.backgroundImage = "url('" + src + "')";
              }
              success_callback();
            } catch(e) {
              failed_callback();
            }
          } else {
            failed_callback();
          }
        }
      };
      xhr.send(null);
    });
  }
  function checkVideo(el, error_callback) {
    var timeout, eventAdded;
    if (!eventAdded) {
      function destroyCheck() {
        clearTimeout(timeout);
        removeEvent(el, 'canplay', onCanPlay);
        removeEvent(el, 'error', onError);
      }
      function onCanPlay() {
        destroyCheck();
      }
      function onError() {
        destroyCheck();
        error_callback();
      }
      eventAdded = true;
      addEvent(el, 'canplay', onCanPlay);
      addEvent(el, 'error', onError);
    }
    if (el.readyState >= 2) {
      destroyCheck();
    } else {
      timeout = setTimeout(function() {
        checkVideo(el, error_callback);
      }, 50);
    }
  }

  var TGrouped = function(id) {
    var groupedEl, groupedWrapEl, groupedLayerEl;
    var margin_w = 2;
    var margin_h = 2;

    groupedEl = document.getElementById(id);
    groupedWrapEl = document.getElementById(id + '_wrap');
    groupedLayerEl = document.getElementById(id + '_layer');
    thumbsEl = groupedLayerEl.children;
    if (!thumbsEl.length) {
      return false;
    }

    addEvent(window, 'resize', function() {
      if (groupedLayerEl.offsetWidth != groupedEl.offsetWidth) {
        recalcGrouped(groupedWrapEl.offsetWidth);
      }
    });
    recalcGrouped(groupedWrapEl.offsetWidth);

    function cloneArr(arrLike) {
      return Array.prototype.slice.apply(arrLike);
    }

    function updateThumb(thumbEl, x, y, width, height, th_width, th_height, position) {
      if (!thumbEl) return;
      var t = false, r = false, b = false, l = false;
      for (var i = 0; i < position.length; i++) {
        if      (position[i] == 't') t = true;
        else if (position[i] == 'r') r = true;
        else if (position[i] == 'b') b = true;
        else if (position[i] == 'l') l = true;
      }
      thumbEl.style.left = x + 'px';
      thumbEl.style.top = y + 'px';
      thumbEl.style.width = width + 'px';
      thumbEl.style.height = height + 'px';
      thumbEl.style.marginRight = (!r ? margin_w : 0) + 'px';
      thumbEl.style.marginBottom = (!b ? margin_h : 0) + 'px';

      var th_ratio = th_width / th_height;
      var ratio    = +thumbEl.getAttribute('data-ratio') || 1.0;
      var mediaEl  = thumbEl.querySelector('.grouped_media');
      var helperEl = thumbEl.querySelector('.grouped_media_helper');

      if (mediaEl) {
        var media_height = Math.ceil(width / ratio);
        var media_tb = height - media_height;
        if (media_tb < 0) {
          var media_t = Math.floor(media_tb / 2);
          var media_b = media_tb - media_t;
          mediaEl.style.left = 0;
          mediaEl.style.right = 0;
          mediaEl.style.top = media_t + 'px';
          mediaEl.style.bottom = media_b + 'px';
        } else {
          var media_width = Math.ceil(height * ratio);
          var media_lr = width - media_width;
          var media_l = Math.floor(media_lr / 2);
          var media_r = media_lr - media_l;
          mediaEl.style.top = 0;
          mediaEl.style.bottom = 0;
          mediaEl.style.left = media_l + 'px';
          mediaEl.style.right = media_r + 'px';
        }
      }
      if (helperEl) {
        var helper_height = Math.floor(th_width / ratio);
        var helper_tb = th_height - helper_height;
        if (helper_tb > 0) {
          var helper_t = Math.floor(helper_tb / 2);
          var helper_b = helper_tb - helper_t;
          helperEl.style.left = 0;
          helperEl.style.right = 0;
          helperEl.style.top = helper_t + 'px';
          helperEl.style.bottom = helper_b + 'px';
        } else {
          var helper_width = Math.ceil(th_height * ratio);
          var helper_lr = th_width - helper_width;
          var helper_l = Math.floor(helper_lr / 2);
          var helper_r = helper_lr - helper_l;
          helperEl.style.top = 0;
          helperEl.style.bottom = 0;
          helperEl.style.left = helper_l + 'px';
          helperEl.style.right = helper_r + 'px';
        }
      }
    }

    function recalcGrouped(max_w) {
      var orients = '';
      var ratios = [];
      var cnt = thumbsEl.length;

      var ratios_sum = 0;
      for (var i = 0; i < thumbsEl.length; i++) {
        var thumbEl = thumbsEl[i];
        var ratio = +thumbEl.getAttribute('data-ratio') || 1.0;
        orients += ratio > 1.2 ? 'w' : (ratio < 0.8 ? 'n' : 'q');
        ratios_sum += ratio;
        ratios.push(ratio);
      }

      var avg_ratio = ratios.length ? ratios_sum / ratios.length : 1.0;
      var max_ratio = 0.75;
      var min_w     = 75;
      var max_h     = max_w / max_ratio;

      var w, h, w0, w1, w2, h0, h1, h2, x, y, x1, x2, y1, y2,
          th_width, th_height;

      if (cnt == 2) {
        if (orients == 'ww' &&
            avg_ratio > 1.4 * max_ratio &&
            (ratios[1] - ratios[0]) < 0.2) {
          // 2 wide pics are one below the other
          w = max_w;
          h = Math.min(w / ratios[0], Math.min(w / ratios[1], (max_h - margin_h) / 2.0));
          th_width  = max_w;
          th_height = 2 * h + margin_h;
          h0 = Math.floor(h);
          h1 = th_height - h0 - margin_h;
          y = h0 + margin_h;
          updateThumb(thumbsEl[0], 0, 0, w, h0, th_width, th_height, 'trl');
          updateThumb(thumbsEl[1], 0, y, w, h1, th_width, th_height, 'rbl');
        }
        else if (orients == 'ww' || orients == 'qq') {
          // 2 equal width pics
          w = (max_w - margin_w) / 2;
          h = Math.floor(Math.min(max_h, Math.min(w / ratios[0], w / ratios[1])));
          th_width  = max_w;
          th_height = h;
          w0 = Math.floor(w);
          w1 = max_w - w0 - margin_w;
          x = w0 + margin_w;
          updateThumb(thumbsEl[0], 0, 0, w0, h, th_width, th_height, 'tbl');
          updateThumb(thumbsEl[1], x, 0, w1, h, th_width, th_height, 'trb');
        }
        else {
          // so, we have one wide and one not wide (square or narrow)
          w0 = Math.floor((max_w - margin_w) / ratios[1] / (1 / ratios[0] + 1 / ratios[1]));
          w1 = max_w - w0 - margin_w;
          h = Math.floor(Math.min(max_h, Math.min(w0 / ratios[0], w1 / ratios[1])));
          th_width  = max_w;
          th_height = h;
          x = w0 + margin_w;
          updateThumb(thumbsEl[0], 0, 0, w0, h, th_width, th_height, 'tbl');
          updateThumb(thumbsEl[1], x, 0, w1, h, th_width, th_height, 'trb');
        }
      }
      else if (cnt == 3) {
        if (orients[0] == 'n') {
          // 2nd and 3rd photos are on the right part
          h0 = max_h;
          h2 = Math.floor(Math.min((max_h - margin_h) * 0.5, ratios[1] * (max_h - margin_h) / (ratios[2] + ratios[1])));
          h1 = max_h - h2 - margin_h;
          w1 = Math.floor(Math.max(min_w, Math.min((max_w - margin_w) * 0.5, Math.min(h2 * ratios[2], h1 * ratios[1]))));
          w0 = Math.min(Math.floor(h0 * ratios[0]), (max_w - w1 - margin_w));
          th_width  = w0 + w1 + margin_w;
          th_height = max_h;
          x = w0 + margin_w;
          y = h1 + margin_h;
          updateThumb(thumbsEl[0], 0, 0, w0, h0, th_width, th_height, 'tbl');
          updateThumb(thumbsEl[1], x, 0, w1, h1, th_width, th_height, 'tr');
          updateThumb(thumbsEl[2], x, y, w1, h2, th_width, th_height, 'rb');
        }
        else {
          // 2nd and 3rd photos are on the next line
          w0 = max_w;
          h0 = Math.floor(Math.min(w0 / ratios[0], (max_h - margin_h) * 0.66));
          w = (max_w - margin_w) / 2;
          h = Math.floor(Math.min(max_h - h0 - margin_h, Math.min(w / ratios[1], w / ratios[2])));
          th_width  = max_w;
          th_height = h0 + h + margin_h;
          w1 = Math.floor(w);
          w2 = max_w - w1 - margin_w;
          x = w1 + margin_w;
          y = h0 + margin_h;
          updateThumb(thumbsEl[0], 0, 0, w0, h0, th_width, th_height, 'tlr');
          updateThumb(thumbsEl[1], 0, y, w1, h, th_width, th_height, 'bl');
          updateThumb(thumbsEl[2], x, y, w2, h, th_width, th_height, 'rb');
        }
      }
      else if (cnt == 4) {
        if (orients == 'wwww' || orients[0] == 'w') {
          // 2nd, 3rd and 4th photos are on the next line
          w = max_w;
          h0 = Math.floor(Math.min(w / ratios[0], (max_h - margin_h) * 0.66));
          h = (max_w - 2 * margin_w) / (ratios[1] + ratios[2] + ratios[3]);
          w0 = Math.floor(Math.max(min_w, Math.min((max_w - 2 * margin_w) * 0.4, h * ratios[1])));
          w2 = Math.floor(Math.max(min_w, Math.min((max_w - 2 * margin_w) * 0.33, h * ratios[3])));
          w1 = w - w0 - w2 - 2 * margin_w;
          h = Math.floor(Math.min(max_h - h0 - margin_h, h));
          th_width  = max_w;
          th_height = h0 + h + margin_h;
          y = h0 + margin_h;
          x1 = w0 + margin_w;
          x2 = x1 + w1 + margin_w;
          updateThumb(thumbsEl[0], 0, 0, w, h0, th_width, th_height, 'tlr');
          updateThumb(thumbsEl[1], 0, y, w0, h, th_width, th_height, 'bl');
          updateThumb(thumbsEl[2], x1, y, w1, h, th_width, th_height, 'b');
          updateThumb(thumbsEl[3], x2, y, w2, h, th_width, th_height, 'rb');
        }
        else {
          // 2nd, 3rd and 4th photos are on the right part
          h = max_h;
          w0 = Math.floor(Math.min(h * ratios[0], (max_w - margin_w) * 0.66));
          w = Math.floor((max_h - 2 * margin_h) / (1 / ratios[1] + 1 / ratios[2] + 1 / ratios[3]));
          h0 = Math.floor(w / ratios[1]);
          h2 = Math.floor(w / ratios[3]);
          h1 = h - h2 - 2 * margin_h;
          w = Math.max(min_w, Math.min(max_w - w0 - margin_w, w));
          th_width  = w0 + w + margin_w;
          th_height = max_h;
          x = w0 + margin_w;
          y1 = h0 + margin_h;
          y2 = y1 + h1 + margin_h;
          updateThumb(thumbsEl[0], 0, 0, w0, h, th_width, th_height, 'tbl');
          updateThumb(thumbsEl[1], x, 0, w, h0, th_width, th_height, 'tr');
          updateThumb(thumbsEl[2], x, y1, w, h1, th_width, th_height, 'r');
          updateThumb(thumbsEl[3], x, y2, w, h2, th_width, th_height, 'rb');
        }
      }
      else {
        var ratios_cropped = [];
        for (var i = 0; i < ratios.length; i++) {
          var ratio = ratios[i];
          if (avg_ratio > 1.1) {
            ratio_cropped = Math.max(1.0, ratio);
          } else {
            ratio_cropped = Math.min(1.0, ratio);
          }
          ratio_cropped = Math.max(0.66667, Math.min(1.7, ratio_cropped));
          ratios_cropped.push(ratio_cropped);
        }

        var multiHeight = function(ratios) {
          var ratios_sum = 0;
          for (var i = 0; i < ratios.length; i++) {
            var ratio = ratios[i];
            ratios_sum += ratio;
          }
          return (max_w - (ratios.length - 1) * margin_w) / ratios_sum;
        };

        var tries = [];
        var first_line, second_line, third_line, fourth_line;
        // Two lines
        for (first_line = 1; first_line <= cnt - 1; first_line++) {
          second_line = cnt - first_line;
          if (first_line  > 3 ||
              second_line > 3) {
            continue;
          }
          tries.push([[first_line, second_line], [
            multiHeight(ratios_cropped.slice(0, first_line)),
            multiHeight(ratios_cropped.slice(first_line)),
          ]]);
        }

        // Three lines
        for (first_line = 1; first_line <= cnt - 2; first_line++) {
          for (second_line = 1; second_line <= cnt - first_line - 1; second_line++) {
            third_line = cnt - first_line - second_line;
            if (first_line  > 3 ||
                second_line > (avg_ratio < 0.85 ? 4 : 3) ||
                third_line  > 3) {
              continue;
            }
            tries.push([[first_line, second_line, third_line], [
              multiHeight(ratios_cropped.slice(0, first_line)),
              multiHeight(ratios_cropped.slice(first_line, first_line + second_line)),
              multiHeight(ratios_cropped.slice(first_line + second_line)),
            ]]);
          }
        }

        // Four lines
        for (first_line = 1; first_line <= cnt - 3; first_line++) {
          for (second_line = 1; second_line <= cnt - first_line - 2; second_line++) {
            for (third_line = 1; third_line <= cnt - first_line - second_line - 1; third_line++) {
              fourth_line = cnt - first_line - second_line - third_line;
              if (first_line  > 3 ||
                  second_line > 3 ||
                  third_line  > 3 ||
                  fourth_line > 3) {
                continue;
              }
              tries.push([[first_line, second_line, third_line, fourth_line], [
                multiHeight(ratios_cropped.slice(0, first_line)),
                multiHeight(ratios_cropped.slice(first_line, first_line + second_line)),
                multiHeight(ratios_cropped.slice(first_line + second_line, first_line + second_line + third_line)),
                multiHeight(ratios_cropped.slice(first_line + second_line + third_line)),
              ]]);
            }
          }
        }

        // Looking for minimum difference between thumbs block height and max_h (may probably be little over)
        var opt_i = false;
        var opt_conf = false;
        var opt_diff = false;
        var opt_h = false;
        for (var i = 0; i < tries.length; i++) {
          var conf_nums = tries[i][0];
          var heights = tries[i][1];
          var heights_sum = 0;
          var heights_min = Infinity;
          for (var j = 0; j < heights.length; j++) {
            heights_sum += heights[j];
            if (heights_min > heights[j]) {
              heights_min = heights[j];
            }
          }
          var conf_h = Math.floor(heights_sum + margin_h * (heights.length - 1));
          var conf_diff = Math.abs(conf_h - max_h);
          if (conf_nums.length > 1) {
            if (conf_nums[0] > conf_nums[1] ||
                conf_nums[2] && conf_nums[1] > conf_nums[2] ||
                conf_nums[3] && conf_nums[2] > conf_nums[3]) {
              conf_diff *= 1.5;
            }
          }
          if (heights_min < min_w) {
            conf_diff *= 1.5;
          }
          if (opt_conf === false || conf_diff < opt_diff) {
            opt_i = i;
            opt_conf = cloneArr(conf_nums);
            opt_diff = conf_diff;
            opt_h = conf_h;
          }
        }

        // Generating optimal thumbs
        th_width  = max_w;
        th_height = opt_h;
        var thumbs_remain = cloneArr(thumbsEl);
        var ratios_remain = cloneArr(ratios_cropped);
        var chunks = cloneArr(opt_conf);
        var opt_heights = cloneArr(tries[opt_i][1]);
        var chunks_num = chunks.length;
        var last_row = chunks_num - 1;
        var sy = 0;
        for (var i = 0; i < chunks.length; i++) {
          var line_chunks_num = chunks[i];
          var line_thumbs = thumbs_remain.splice(0, line_chunks_num);
          var line_height = opt_heights.shift();
          var last_column = line_thumbs.length - 1;
          var h = Math.floor(line_height);
          var sx = 0;
          var t = '', r = '', b = '', l = '';
          if (i == 0) {
            t = 't';
          }
          if (i == last_row) {
            b = 'b';
            h = th_height - sy;
          }
          for (var j = 0; j < line_thumbs.length; j++) {
            var thumbEl = line_thumbs[j];
            var thumb_ratio = ratios_remain.shift();
            var w = Math.floor(thumb_ratio * h);
            if (j == 0) {
              l = 'l';
            }
            if (j == last_column) {
              r = 'r';
              w = th_width - sx;
            }
            updateThumb(thumbEl, sx, sy, w, h, th_width, th_height, t+r+b+l);
            sx += w + margin_w;
          }
          sy += h + margin_h;
        }
      }

      groupedEl.style.paddingTop  = (th_height / th_width * 100) + '%';
      groupedLayerEl.style.width  = th_width + 'px';
      groupedLayerEl.style.height = th_height + 'px';
    }
  };
  window.TGrouped = TGrouped;

  function initWidgetFrame(options) {
    options = options || {};
    if (window.devicePixelRatio >= 2) {
      document.body.classList.add('r2x');
    }

    var lastHeight = null,
        lastWidth = null,
        timeout = 100,
        timeoutId = null,
        wasVisible = false,
        observer;

    function checkFrameSize() {
      var height, width, style;
      if (window.getComputedStyle) {
        style = window.getComputedStyle(document.body);
        height = style.height;
        if (height.substr(-2) == 'px') {
          height = height.slice(0, -2);
        }
        width = style.width;
        if (width.substr(-2) == 'px') {
          width = width.slice(0, -2);
        }
      } else {
        height = document.body.offsetHeight;
        width = document.body.offsetWidth;
      }
      var data = {event: 'resize'}, resized = false;
      if (options.auto_height) {
        height = Math.ceil(height);
        if (height != lastHeight) {
          lastHeight = height;
          data.height = height;
          resized = true;
        }
      }
      if (options.auto_width) {
        width = Math.ceil(width);
        if (width != lastWidth) {
          lastWidth = width;
          data.width = width;
          resized = true;
        }
      }
      if (resized) {
        sendPostMessage(data);
      }
      requestAnimationFrame(checkFrameSize);
    }
    if (options.auto_height || options.auto_width) {
      addEvent(window, 'resize', checkFrameSize);
      checkFrameSize();
    }

    function postMessageHandler(event) {
      try {
        var data = JSON.parse(event.data);
      } catch(e) {
        var data = {};
      }
      if (event.source !== window.parent) return;
      if (data.event == 'visible') {
        if (!wasVisible) {
          wasVisible = true;
          options.onVisible && options.onVisible();
        }
        sendPostMessage({event: 'visible_off'});
      }
    }
    addEvent(window, 'message', postMessageHandler);
  }

  window.TPhoto = (function() {
    return {
      init: function(id) {
        var TPhoto = {
          init: function(id) {
            TPhoto.photoEl = document.getElementById(id);
            if (!TPhoto.photoEl) return;
            TPhoto.inGroup = TPhoto.photoEl.classList.contains('grouped_media_wrap');
            if (TPhoto.inGroup) {
              addEvent(TPhoto.photoEl, 'click', TPhoto.toggle);
            }
          },
          toggle: function(e) {
            if (e.metaKey || e.ctrlKey) return true;
            e.stopPropagation();
            if (!TPhoto.photoEl) return true;
            if (TPhoto.opened) {
              TPhoto.opened = false;
              TPhoto.photoEl.classList.remove('active');
              TPhoto.overTo = setTimeout(function() {
                TPhoto.photoEl.classList.remove('over');
              }, 200);
            } else {
              TPhoto.opened = true;
              clearTimeout(TPhoto.overTo);
              TPhoto.photoEl.classList.add('over');
              TPhoto.photoEl.classList.add('active');
            }
            e.preventDefault();
            return false;
          }
        };
        TPhoto.init(id);
      }
    }
  })();

  window.TVideo = (function() {
    return {
      init: function(id) {
        var TVideo = {
          init: function(id) {
            TVideo.videoEl = document.getElementById(id);
            TVideo.videoBluredEl = document.getElementById(id + '_blured');
            TVideo.playerEl = document.getElementById(id + '_player');
            TVideo.playEl = document.getElementById(id + '_play');
            TVideo.durationEl = document.getElementById(id + '_duration');
            TVideo.notSupportedEl = document.getElementById(id + '_not_supported');
            TVideo.inGroup = TVideo.playerEl.classList.contains('grouped_media_wrap');
            if (TVideo.inGroup) {
              addEvent(TVideo.playerEl, 'click', TVideo.inGroupToggle);
            }
            if (!TVideo.videoEl) return;
            if (TVideo.videoEl.hasAttribute('playsinline')) {
              enableInlineVideo(TVideo.videoEl);
              if (TVideo.videoBluredEl) {
                enableInlineVideo(TVideo.videoBluredEl);
              }
            }
            var looped = TVideo.videoEl.hasAttribute('loop');
            function fixControls() {
              if (TVideo.videoEl.controls) TVideo.videoEl.controls = false;
            }
            var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
            if (MutationObserver) {
              (new MutationObserver(fixControls)).observe(TVideo.videoEl, {
                attributes: true
              });
            }

            checkVideo(TVideo.videoEl, function() {
              TVideo.playerEl.classList.add('not_supported');
            });
            addEvent(TVideo.videoEl, 'play', function() {
              fixControls();
              TVideo.playerEl.classList.add('playing');
              if (TVideo.inGroup) {
                clearTimeout(TVideo.overTo);
                TVideo.playerEl.classList.add('over');
                TVideo.playerEl.classList.add('active');
              }
              if (TVideo.videoBluredEl) {
                TVideo.videoBluredEl.currentTime = TVideo.videoEl.currentTime;
                if (!browser.mobile) {
                  TVideo.videoBluredEl.play();
                }
              }
            });
            addEvent(TVideo.videoEl, 'pause', function() {
              fixControls();
              TVideo.playerEl.classList.remove('playing');
              if (TVideo.inGroup) {
                TVideo.playerEl.classList.remove('active');
                TVideo.overTo = setTimeout(function() {
                  TVideo.playerEl.classList.remove('over');
                }, 200);
              }
              if (TVideo.videoBluredEl) {
                TVideo.videoBluredEl.currentTime = TVideo.videoEl.currentTime;
                TVideo.videoBluredEl.pause();
              }
              if (looped) {
                TVideo.videoEl.play();
              }
            });
            addEvent(TVideo.videoEl, 'timeupdate', function(e) {
              fixControls();
              if (TVideo.videoBluredEl && TVideo.videoBluredEl.currentTime != TVideo.videoEl.currentTime) {
                TVideo.videoBluredEl.currentTime = TVideo.videoEl.currentTime;
              }
              if (TVideo.durationEl && TVideo.videoEl.duration) {
                var duration = Math.floor(TVideo.videoEl.duration);
                TVideo.durationEl.innerHTML = formatDuration(duration - TVideo.videoEl.currentTime);
              }
            });
            addEvent(TVideo.videoEl, 'ended load', function(e) {
              fixControls();
              if (TVideo.durationEl && TVideo.videoEl.duration) {
                var duration = Math.floor(TVideo.videoEl.duration);
                TVideo.durationEl.innerHTML = formatDuration(duration);
              }
            });
            if (looped) {
              addEvent(document, 'touchstart', function(){ TVideo.videoEl.play(); });
            } else {
              TVideo.playerEl.classList.add('ready');
            }
            if (TVideo.playEl) {
              addEvent(TVideo.playEl, 'click', TVideo.toggle);
            }
          },
          toggle: function(e) {
            if (e.metaKey || e.ctrlKey) return true;
            e.stopPropagation();
            if (!TVideo.videoEl) return true;
            if (TVideo.videoEl.paused) {
              TVideo.videoEl.play();
              if (TVideo.videoBluredEl) {
                TVideo.videoBluredEl.play();
              }
            } else {
              TVideo.videoEl.pause();
              if (TVideo.videoBluredEl) {
                TVideo.videoBluredEl.pause();
              }
            }
            e.preventDefault();
            return false;
          },
          inGroupToggle: function(e) {
            if (e.metaKey || e.ctrlKey) return true;
            if (e.target.classList.contains('message_media_view_in_telegram')) return true;
            e.stopPropagation();
            if (!TVideo.playerEl) return true;
            if (TVideo.playerEl.classList.contains('active')) {
              TVideo.playerEl.classList.remove('active');
              TVideo.overTo = setTimeout(function() {
                TVideo.playerEl.classList.remove('over');
              }, 200);
            } else {
              clearTimeout(TVideo.overTo);
              TVideo.playerEl.classList.add('over');
              TVideo.playerEl.classList.add('active');
            }
            e.preventDefault();
            return false;
          }
        };
        TVideo.init(id);
      }
    }
  })();

  var TRoundVideo = {
    init: function(id) {
      TRoundVideo.videoEl = document.getElementById(id);
      TRoundVideo.playerEl = document.getElementById(id + '_player');
      TRoundVideo.progressEl = document.getElementById(id + '_progress');
      TRoundVideo.durationEl = document.getElementById(id + '_duration');
      if (!TRoundVideo.videoEl) return;

      enableInlineVideo(TRoundVideo.videoEl);

      checkVideo(TRoundVideo.videoEl, function() {
        TRoundVideo.playerEl.classList.add('not_supported');
      });
      TRoundVideo.autoplay();
      addEvent(document, 'touchstart', TRoundVideo.autoplay);
      addEvent(TRoundVideo.videoEl, 'ended', function(e) {
        TRoundVideo.autoplay();
      });
      addEvent(window, 'resize', function(){ TRoundVideo.redrawProgress(true) });
    },
    showProgress: function() {
      if (!TRoundVideo.videoEl) return;
      if (TRoundVideo.playing && !TRoundVideo.videoEl.paused) {
        requestAnimationFrame(function(){ TRoundVideo.showProgress(); });
      }
      TRoundVideo.redrawProgress();
      if (TRoundVideo.videoEl.duration) {
        var duration = Math.floor(TRoundVideo.videoEl.duration);
        TRoundVideo.durationEl.innerHTML = formatDuration(duration - TRoundVideo.videoEl.currentTime);
      }
    },
    redrawProgress: function(updateSVG) {
      if (!TRoundVideo.videoEl) return;
      var progress;
      if (TRoundVideo.playing) {
        progress = TRoundVideo.videoEl.currentTime / TRoundVideo.videoEl.duration;
      } else {
        progress = 0;
      }
      progress = Math.max(0, Math.min(progress, 1));
      var wrapWidth = TRoundVideo.playerEl.offsetWidth;
      var d = (wrapWidth - 15);
      var l = (d * Math.PI);
      TRoundVideo.progressEl.setAttribute('r', (d / 2));
      TRoundVideo.progressEl.setAttribute('stroke-dasharray', l);
      TRoundVideo.progressEl.setAttribute('stroke-dashoffset', l * (1 - progress));
      if (updateSVG) {
        TRoundVideo.progressEl.style.transform = !TRoundVideo.progressEl.style.transform ? 'rotateZ(270deg)' : '';
      }
    },
    play: function() {
      if (!TRoundVideo.videoEl) return;
      var video = TRoundVideo.videoEl;
      var isPlaying = (video.currentTime > 0) &&
                      !video.paused &&
                      !video.ended &&
                      (video.readyState > 2);
      if (!isPlaying) video.play();
    },
    pause: function() {
      if (!TRoundVideo.videoEl) return;
      TRoundVideo.videoEl.pause();
    },
    autoplay: function() {
      if (!TRoundVideo.videoEl) return;
      removeEvent(document, 'touchstart', TRoundVideo.autoplay);
      TRoundVideo.playerEl.classList.remove('playing');
      TRoundVideo.playing = false;
      TRoundVideo.videoEl.muted = true;
      TRoundVideo.videoEl.loop = true;
      TRoundVideo.videoEl.currentTime = 0;
      TRoundVideo.play();
      TRoundVideo.showProgress();
    },
    toggle: function(e) {
      e.stopPropagation();
      if (!TRoundVideo.playing) {
        TRoundVideo.redrawProgress();
        TRoundVideo.playerEl.classList.add('playing');
        TRoundVideo.playing = true;
        TRoundVideo.videoEl.muted = false;
        TRoundVideo.videoEl.loop = false;
        TRoundVideo.videoEl.currentTime = 0;
        TRoundVideo.play();
        TRoundVideo.showProgress();
      } else {
        if (TRoundVideo.videoEl.paused) {
          TRoundVideo.play();
          TRoundVideo.showProgress();
        } else {
          TRoundVideo.pause();
        }
      }
    }
  };
  window.TRoundVideo = TRoundVideo;

  var TVoice = {
    init: function(id, is_ogg) {
      TVoice.audioEl = document.getElementById(id);
      if (!TVoice.audioEl) return;
      TVoice.playerEl = document.getElementById(id + '_player');
      TVoice.durationEl = document.getElementById(id + '_duration');
      TVoice.progressEl = document.getElementById(id + '_progress');
      TVoice.progressWrapEl = document.getElementById(id + '_progress_wrap');
      TVoice.notSupportedEl = document.getElementById(id + '_not_supported');

      TVoice.loadSupportLib(function(success) {
        if (!success) return;
        if (is_ogg &&
            OGVCompat.hasWebAudio() &&
            OGVCompat.supported('OGVPlayer')) {
          TVoice.loadLib(function(success) {
            if (!success) return;
            TVoice.player = new OGVPlayer();
            TVoice.player.src = TVoice.audioEl.src;
            TVoice.initPlayer();
          });
        } else {
          TVoice.player = TVoice.audioEl;
          TVoice.initPlayer();
        }
      });
      addEvent(window, 'resize', function(){ TVoice.redrawProgress() });
      TVoice.redrawProgress();
    },
    loadLib: function(callback) {
      var script = document.createElement('script');
      script.type = 'text/javascript';
      script.async = true;
      script.src = TBaseUrl + 'js/ogvjs/ogv.js';
      script.onerror = function() {
        callback(false);
      }
      script.onload = function() {
        callback(true);
      }
      var head = document.getElementsByTagName('head')[0];
      head.appendChild(script);
      return script;
    },
    loadSupportLib: function(callback) {
      var script = document.createElement('script');
      script.type = 'text/javascript';
      script.async = true;
      script.src = TBaseUrl + 'js/ogvjs/ogv-support.js';
      script.onerror = function() {
        callback(false);
      }
      script.onload = function() {
        callback(true);
      }
      var head = document.getElementsByTagName('head')[0];
      head.appendChild(script);
      return script;
    },
    initPlayer: function() {
      TVoice.playerEl.classList.add('ready');
      addEvent(TVoice.player, 'play', function() {
        TVoice.playerEl.classList.add('playing');
      });
      addEvent(TVoice.player, 'pause', function() {
        TVoice.playerEl.classList.remove('playing');
      });
      addEvent(TVoice.player, 'ended', function() {
        if (TVoice.durationEl && TVoice.player.duration) {
          var duration = Math.floor(TVoice.player.duration);
          TVoice.durationEl.innerHTML = formatDuration(duration);
        }
      });
      addEvent(TVoice.playerEl, 'click', TVoice.toggle);
      addEvent(TVoice.progressWrapEl, 'mousedown touchstart', TVoice.seekStart);
    },
    seekStart: function(e) {
      if (TVoice.player &&
          (!TVoice.player.paused || TVoice.player.currentTime > 0 && TVoice.player.currentTime < TVoice.player.duration)) {
        e.preventDefault();
        TVoice.seeking = true;
        TVoice.disableClick = false;
        TVoice.player.pause();
        addEvent(document, 'mousemove touchmove', TVoice.seek);
        addEvent(document, 'mouseup mouseleave touchend touchcancel', TVoice.seekEnd);
        TVoice.seek(e);
      }
    },
    seek: function(e) {
      if (!TVoice.seeking) return;
      var px = e.pageX;
      var op = TVoice.progressWrapEl;
      var x = op.offsetLeft;
      var w = op.offsetWidth;
      while (op = op.offsetParent) {
        x += op.offsetLeft;
      }
      var ct = Math.max(0, Math.min(px - x, w)) / w;
      TVoice.seekTo = ct;
      TVoice.showProgress();
    },
    seekEnd: function(e) {
      if (!TVoice.seeking) return;
      TVoice.seek(e);
      var duration = Math.floor(TVoice.player.duration);
      TVoice.player.currentTime = TVoice.seekTo * duration;
      TVoice.seekTo = null;
      TVoice.seeking = false;
      if (e.type.substr(0, 5) == 'mouse') {
        TVoice.disableClick = true;
      }
      TVoice.player.play();
      TVoice.showProgress();
      removeEvent(document, 'mousemove touchmove', TVoice.seek);
      removeEvent(document, 'mouseup touchend', TVoice.seekEnd);
    },
    showProgress: function() {
      if (!TVoice.player) return;
      if (!TVoice.player.paused) {
        requestAnimationFrame(function(){ TVoice.showProgress(); });
      }
      if (TVoice.player.duration) {
        var duration = Math.floor(TVoice.player.duration);
        if (TVoice.seeking) {
          var currentTime = TVoice.seekTo * duration;
        } else {
          var currentTime = Math.max(0, TVoice.player.currentTime);
        }
        if (TVoice.progressEl) {
          TVoice.progressEl.style.width = (currentTime / duration * 100) + '%';
        }
        if (TVoice.durationEl) {
          TVoice.durationEl.innerHTML = formatDuration(duration - currentTime);
        }
      }
    },
    redrawProgress: function() {
      if (!TVoice.audioEl) return;
      var ss = TVoice.progressWrapEl.getElementsByTagName('S');
      var ss_count = ss.length / 2;
      var waveform_str = TVoice.audioEl.getAttribute('data-waveform') || '';
      var waveform = waveform_str.split(',');
      var lines_cnt = Math.floor((TVoice.progressWrapEl.offsetWidth + 2) / 6);
      var p = waveform.length / lines_cnt;
      var values = [];
      var max_value = 0;
      for (var i = 0; i < lines_cnt; i++) {
        var ws = i * p;
        var we = ws + p;
        var wsi = Math.floor(ws);
        var wei = Math.floor(we);
        if (wsi == wei) {
          var value = waveform[wsi] * (we - ws);
        } else {
          var value = 0;
          for (var j = wsi; j <= wei; j++) {
            var wv = +waveform[j] || 0;
            if (j == wsi) {
              value += wv * (j + 1 - ws);
            } else if (j == wei) {
              value += wv * (we - j);
            } else {
              value += wv;
            }
          }
        }
        value = value / p;
        max_value = Math.max(value, max_value);
        values.push(value);
      }
      for (var i = 0; i < ss.length; i++) {
        var li = i % ss_count;
        if (li < lines_cnt) {
          var height = (values[li] / max_value) * 100;
          ss[i].style.height = height + '%';
          ss[i].style.display = '';
        } else {
          ss[i].style.display = 'none';
        }
      }
    },
    toggle: function(e) {
      e.stopPropagation();
      if (!TVoice.disableClick) {
        if (!TVoice.player) return true;
        if (TVoice.player.paused) {
          TVoice.player.play();
          TVoice.showProgress();
        } else {
          TVoice.player.pause();
          TVoice.showProgress();
        }
      } else {
        TVoice.disableClick = false;
      }
      e.preventDefault();
      return false;
    }
  };
  window.TVoice = TVoice;

  var TSticker = {
    init: proccessWebpImage
  };
  window.TSticker = TSticker;

  window.TWidgetPost = {
    init: function(id) {
      var widget = document.getElementById(id);

      initWidgetFrame({
        auto_height: true,
        onVisible: function() {
          if (widget) {
            var view = widget.getAttribute('data-view');
            if (view) {
              var xhr = getXHR();
              xhr.open('get', location.href + '&view=' + encodeURIComponent(view));
              xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
              xhr.send(null);
            }
          }
        }
      });
      if (!doesSupportEmoji()) {
        document.body.classList.remove('emoji_default');
        document.body.classList.add('emoji_image');
      }
      var times = document.getElementsByTagName('time');
      if (times.length) {
        for (var k = 0; k < times.length; k++) {
          var time = times[k];
          var datetime = time.getAttribute('datetime');
          if (datetime) {
            time.innerHTML = formatDate(datetime);
          }
        }
      }
    }
  };

  var TWidgetLogin = {
    init: function(id, bot_id, params, init_auth) {
      if (arguments.length == 5) { // legacy
        params = arguments[3]; init_auth = arguments[4];
      }
      initWidgetFrame({auto_height: true, auto_width: true});
      TWidgetLogin.widgetEl = document.getElementById(id);
      TWidgetLogin.botId = bot_id;
      TWidgetLogin.params = params;
      var params_encoded = '', params_arr = [];
      for (var k in params) {
        params_arr.push(encodeURIComponent(k) + '=' + encodeURIComponent(params[k]));
      }
      TWidgetLogin.paramsEncoded = params_arr.join('&');
      if (init_auth) {
        TWidgetLogin.getAuth(true);
      }
    },
    auth: function() {
      var width = 550;
      var height = 450;
      var left = Math.max(0, (screen.width - width) / 2) + (screen.availLeft | 0),
          top = Math.max(0, (screen.height - height) / 2) + (screen.availTop | 0);
      function checkClose() {
        if (!TWidgetLogin.activePopup || TWidgetLogin.activePopup.closed) {
          return TWidgetLogin.onClose();
        }
        setTimeout(checkClose, 100);
      }
      TWidgetLogin.activePopup = window.open('https://oauth.telegram.org/auth?bot_id=' + TWidgetLogin.botId + (TWidgetLogin.paramsEncoded ? '&' + TWidgetLogin.paramsEncoded : ''), 'telegram_oauth', 'width=' + width + ',height=' + height + ',left=' + left + ',top=' + top + ',status=0,location=0,menubar=0,toolbar=0');
      if (TWidgetLogin.activePopup) {
        TWidgetLogin.activePopup.focus();
        checkClose();
      }
    },
    getAuth: function(init) {
      var xhr = getXHR();
      xhr.open('POST', 'https://oauth.telegram.org/auth/get?bot_id=' + TWidgetLogin.botId);
      xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      
      xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) {
          if (typeof xhr.responseBody == 'undefined' && xhr.responseText) {
            try {
              var result = JSON.parse(xhr.responseText);
            } catch(e) {
              var result = {};
            }
            if (result.html && TWidgetLogin.widgetEl.innerHTML != result.html) {
              TWidgetLogin.widgetEl.innerHTML = result.html;
            }
            if (result.user) {
              TWidgetLogin.onAuth(result.origin, result.user, init);
            } else {
              TWidgetLogin.onAuth(result.origin, false, init);
            }
          } else {
            TWidgetLogin.onAuth(result.origin, false, init);
          }
        }
      };
      xhr.onerror = function() {
        TWidgetLogin.onAuth(result.origin, false, init);
      };
      xhr.send(TWidgetLogin.paramsEncoded);
    },
    onAuth: function(origin, authData, init) {
      if (authData) {
        var data = {event: 'auth_user', auth_data: authData};
      } else {
        var data = {event: 'unauthorized'};
      }
      if (init) {
        data.init = true;
      }
      sendPostMessage(data, origin);
    },
    onClose: function() {
      TWidgetLogin.getAuth();
    }
  };
  window.TWidgetLogin = TWidgetLogin;

})();