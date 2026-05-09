// Entry-point module. Bootstraps the Potree viewer, populates shared state,
// and kicks off data loading. Sibling modules read state from
// `state.module.js`. The few `window.*` assignments below are for console
// debugging only — they expose user-callable helpers, not internal state.

import config from "./config.module.js";
import dataLoader from "./dataLoader.module.js";
import { instanceShow, hideAll, showAll } from "./my_utils.js";
import { groupBy } from "./initScene.module.js";
import { state } from "./state.module.js";

state.viewer = new Potree.Viewer(document.getElementById("potree_render_area"));
state.scene = state.viewer.scene;

// console-debug helpers
window.instanceShow = instanceShow;
window.hideAll = hideAll;
window.showAll = showAll;
window.groupBy = groupBy;

state.viewer.setEDLEnabled(false); // disable so transparency in the meshes works fine
state.viewer.setFOV(60);
state.viewer.setPointBudget(2_000_000);
state.viewer.loadSettingsFromURL();
state.viewer.setFilterFloatArray([]);

// Load the gene-panel mapping (point source id → gene name) for the spot tooltip.
// Exposed on window so the Potree-bundled InputHandler.getHoveredSpot() can read it.
fetch('./data/gene_dict.json').then(r => r.json()).then(d => { window.genePanel = d; });

const url = './pointclouds/sfn/octree/metadata.json';
Potree.loadPointCloud(url, 'merfish', onloaded);

function onloaded(e) {
    let scene = state.viewer.scene;
    let pointcloud = e.pointcloud;
    let material = pointcloud.material;

    material.size = 0.02;
    material.pointSizeType = Potree.PointSizeType.ADAPTIVE;
    material.shape = Potree.PointShape.CIRCLE;
    material.activeAttributeName = "rgba";

    scene.addPointCloud(pointcloud);
    scene.view.position.set(0.0, 330, 17000);
    scene.view.lookAt(0, 0, 0);

    run();
}

function run() {
    var configSettings = config();
    configSettings.cellData["name"] = "cellData";
    makePackage([configSettings.cellData]);
}

function makePackage(result) {
    var workPackage = result.reduce((a, b) => a.concat(b), []);
    workPackage.forEach(d => d.root_name = stripUrl(d.name));
    workPackage.forEach(d => d.bytes_streamed = 0);  // bytes streamed so far
    workPackage.forEach(d => d.data = []);            // accumulated rows from the flatfiles
    workPackage.forEach(d => d.data_length = 0);      // number of rows fetched so far
    dataLoader(workPackage);
}

function stripUrl(d) {
    var fName = d.substring(d.lastIndexOf('/') + 1);
    return fName.split('.')[0];
}