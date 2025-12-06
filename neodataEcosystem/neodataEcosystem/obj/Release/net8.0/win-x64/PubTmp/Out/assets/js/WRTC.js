var _WRTC = {
	debug: false,
	local: null,
	remote:null,
	localStream: null,
	_chatChannel: null,
	pc: null,
	localOffer: null,
	remoteOffer:null,
	_TMR_ANSWER: 0,
	_last_optionsWrtc:null,
	conf: {
		'iceServers': [
			{ 'urls': 'stun:stun.l.google.com:19302' },
			{ 'urls': 'stun:stun1.l.google.com:19302' },
			{ 'urls': 'stun:stun2.l.google.com:19302' },
			{ 'urls': 'stun:stun3.l.google.com:19302' },
			{ 'urls': 'stun:stun4.l.google.com:19302' }
		]
	},
	onLog: function (ctx, msg) {
		if (_WRTC.debug) { console.log(ctx, msg); }
	},
	initialize: function (_optionsWrtc) {
		_WRTC._last_optionsWrtc = _optionsWrtc;
		_WRTC.pc = new RTCPeerConnection(_WRTC.conf);
		_WRTC.local = document.getElementById(_optionsWrtc.localId);
		_WRTC.remote = document.getElementById(_optionsWrtc.remoteId);
		_WRTC.remote.style.display = "block";
		_WRTC.local.style.display = "block";
		_WRTC.pc.ondatachannel = function (e) {
			if (e.channel.label == "chatChannel") {
				_WRTC._chatChannel = e.channel;
				_WRTC.onChatChannel(e.channel, ("#" + _optionsWrtc._charArea));
			}
		};
		_WRTC.pc.onicecandidate = function (e) {
			var cand = e.candidate;
			if (!cand) {
				_WRTC.localOffer = JSON.stringify(_WRTC.pc.localDescription);
			} else {
				_WRTC.onLog(cand.candidate);
			}
		}
		_WRTC.pc.oniceconnectionstatechange = function () {
			_WRTC.onLog('iceconnectionstatechange: ', _WRTC.pc.iceConnectionState);
		}
		_WRTC.pc.onaddstream = function (e) {_WRTC.remote.srcObject = e.stream;}
		_WRTC.pc.onconnection = function (e) {
			_WRTC.onLog('onconnection ', e);
		}
		navigator.mediaDevices.getUserMedia({ audio: true, video: true }).then(stream => {
			_WRTC.localStream = stream;
			_WRTC.pc.addStream(stream);
			_WRTC.local.srcObject = stream;
			_WRTC.local.muted = true;
		}).catch(_WRTC.onErrorHandler);

	},
	onErrorHandler: function (err) {
		_WRTC.onLog("Error - ", err);
	},
	onChatChannel: function (e, _charArea) {
		_WRTC._chatChannel.onopen = function (e) {
			_WRTC.onLog('chat channel is open', e);
		}
		_WRTC._chatChannel.onmessage = function (e) {
			$(_charArea).append("<pre>Recibido " + today.toDateString() + ":<br/> " + text + "</pre>")
		}
		_WRTC._chatChannel.onclose = function () {
			_WRTC.onLog('chat channel closed');
		}
	},
	onSendMessage: function (_this) {
		var today = new Date();
		var _source = _this.attr("data-source");
		var _target = _this.attr("data-target");

		var text = $(_source).val();
		$(_target).append("<pre>Enviado " + today.toDateString() + ":<br/> " + text + "</pre>")
		_WRTC._chatChannel.send(text);
		$(_source).val("");
	},
	onCreateNewVideoRoom: function () {
		return new Promise(
			function (resolve, reject) {
				try {
					clearInterval(_WRTC._TMR_ANSWER);
					_WRTC._chatChannel = _WRTC.pc.createDataChannel('chatChannel');
					_WRTC.onChatChannel(_WRTC._chatChannel);
					_WRTC.pc.createOffer().then(des => {
						_WRTC.pc.setLocalDescription(des).then(() => {
							setTimeout(function () {
								if (_WRTC.pc.iceGatheringState == "complete") {
									return;
								} else {
									_WRTC.localOffer = JSON.stringify(_WRTC.pc.localDescription);
									_NEOVIDEO.onAuthenticate().then(function (_auth) {
										var _config = { "raw_data": _WRTC.localOffer };
										_NEOVIDEO.UiCreateNewVideoRoom(_config).then(function (data) {
											_NEOVIDEO._rol = "host";
											_NEOVIDEO._id_transaction = data.records[0].id;
											_WRTC._TMR_ANSWER = setInterval(function () { _WRTC.onCheckRemoteResponse(); }, 1500);
											resolve(_NEOVIDEO._id_transaction);
										}).catch(function (err) {
											reject(err);
										});
									});
								}
							}, 2000);
						}).catch();
					}).catch();
				} catch (err) {
					reject(err);
				}
			});
	},
	onJoinOpenSession: function () {
		return new Promise(
			function (resolve, reject) {
				try {
					/*FORCED*/
					var _id = prompt("ID?");
					/*FORCED*/

					_NEOVIDEO.onAuthenticate().then(function (_auth) {
						_NEOVIDEO.UiJoinOpenSession({ "id": _id }).then(function (data) {
							if (data.status != "OK") { reject(data); }
							var _id = data.records[0].id;
							_WRTC.remoteOffer = data.records[0].transaction.raw_offer;
							var _remoteOffer = new RTCSessionDescription(JSON.parse(_WRTC.remoteOffer));
							_WRTC.pc.setRemoteDescription(_remoteOffer).then(function () {
								if (_remoteOffer.type == "offer") {
									_NEOVIDEO._rol = "guest";
									_NEOVIDEO._id_transaction = _id;
									_WRTC.pc.createAnswer().then(function (description) {
										_WRTC.pc.setLocalDescription(description).then(function () {
											_WRTC.localOffer = JSON.stringify(_WRTC.pc.localDescription);
											_NEOVIDEO.onAuthenticate().then(function (_auth) {
												_NEOVIDEO.UiUpdateWrtcAnswer({ "id": _id, "raw_answer": _WRTC.localOffer }).then(function (data) { })
											});
										}).catch();
									}).catch();
								}
							}).catch();
						}).catch(function (err) {
							reject(err);
						});
					});
				} catch (err) {
					reject(err);
				}
			});
	},

	onCheckRemoteResponse: function () {
		_NEOVIDEO.onAuthenticate().then(function (_auth) {
			_NEOVIDEO.UiGetWrtcOfferAnswer({ "id": _NEOVIDEO._id_transaction }).then(function (answer) {
				if (answer.status != "OK") { reject(answer); }
				if (answer.data[0].raw_answer.length > 10) {
					clearInterval(_WRTC._TMR_ANSWER);
					_WRTC.remoteOffer = answer.data[0].raw_answer;
					var _remoteOffer = new RTCSessionDescription(JSON.parse(_WRTC.remoteOffer));
					_WRTC.pc.setRemoteDescription(_remoteOffer).then(function () {
						if (_remoteOffer.type == "offer") {
							_WRTC.pc.createAnswer().then(function (description) {
								_WRTC.pc.setLocalDescription(description).then(function () {

								}).catch();
							}).catch();
						}
					}).catch();
				}
			}).catch(function (err) {
				reject(err);
			});
		});
	},
}

