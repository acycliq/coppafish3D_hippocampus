// UI helpers shared across modules and inline event handlers.
// Loaded as a classic script so these are global, matching the way
// initScene.module.js and other modules call them by bare name.
//
// All of them assume `viewer` has been initialised (set in main.module.js)
// before they are called.

function removePreloader() {
    $('#preloader').delay(350).fadeOut(250); // fade out the white div that covers the website
    $('body').delay(350).css({'overflow': 'visible'});
}

function removeLines() {
    var scene = viewer.scene.scene;
    scene.children.filter(d => d.type === "Line").forEach(el => scene.remove(el));
}

function clearScreen() {
    removeLines();
    hideControls();
}

function hideControls() {
    $('#dataTableControl').hide();
    $('#donutChartControl').hide();
    $('#cellCoordsControl').hide();
}

function showControls() {
    $('#dataTableControl').show();
    $('#donutChartControl').show();
    $('#cellCoordsControl').show();
}