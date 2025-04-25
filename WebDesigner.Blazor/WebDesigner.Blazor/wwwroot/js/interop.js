window.initJSInterop = (dotnet) => {

	document.addEventListener('keydown', function (event) {
		switch (event.code) {
			case 'F9':
				event.preventDefault();
				dotnet.invokeMethodAsync('OnClickBackButton');
				break;
			case 'F10':
				event.preventDefault();
				dotnet.invokeMethodAsync('OnClickNextButton');
				break;
			case 'F11':
				event.preventDefault();
				dotnet.invokeMethodAsync('OnClickEnterButton');
				break;
			case 'F5':
				if (event.shiftKey) {
					event.preventDefault();
					dotnet.invokeMethodAsync('OnClickRestartButton');
				} else {
					event.preventDefault();
					dotnet.invokeMethodAsync('OnClickFastForwardButton');
				}
				break;
		}
	});

	// window or document ?
	//window.addEventListener('resize', () => {
	//	let trackedElement = document.getElementById('mainCanvasPositioner');
	//	dotnetHelper.invokeMethodAsync('OnResize', trackedElement.getBoundingClientRect().width, trackedElement.getBoundingClientRect().height);
	//});

	//let trackedElement = document.getElementById('mainCanvasPositioner');
	//dotnetHelper.invokeMethodAsync('OnResize', trackedElement.getBoundingClientRect().width, trackedElement.getBoundingClientRect().height);
}

