// UI helpers for showing/hiding the side controls and clearing the scene.
// Imported by initScene.module.js; loaded automatically through the module graph
// (no <script> tag in index.html).

import { state } from "./state.module.js";

export function removePreloader() {
    $('#preloader').delay(350).fadeOut(250); // fade out the white div that covers the website
    $('body').delay(350).css({'overflow': 'visible'});
}

export function removeLines() {
    var scene = state.viewer.scene.scene;
    scene.children.filter(d => d.type === "Line").forEach(el => scene.remove(el));
}

export function clearScreen() {
    removeLines();
    hideControls();
}

function hideControls() {
    $('#dataTableControl').hide();
    $('#donutChartControl').hide();
    $('#cellCoordsControl').hide();
}

export function showControls() {
    $('#dataTableControl').show();
    $('#donutChartControl').show();
    $('#cellCoordsControl').show();
}