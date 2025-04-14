window.registerViewportChangeCallback = (dotnetHelper) => {
	window.addEventListener('resize', () => {
		let trackedElement = document.getElementById('mainCanvasPositioner');
		dotnetHelper.invokeMethodAsync('OnResize', trackedElement.getBoundingClientRect().width, trackedElement.getBoundingClientRect().height);
	});

	let trackedElement = document.getElementById('mainCanvasPositioner');
	dotnetHelper.invokeMethodAsync('OnResize', trackedElement.getBoundingClientRect().width, trackedElement.getBoundingClientRect().height);
}
