// Entry-point module. Bootstraps the Potree viewer, populates shared state,
// and kicks off data loading. Sibling modules read state from
// `state.module.js`; `window.viewer` / `window.scene` are kept exclusively for
// `controls.js` (a classic script that can't `import`) and console debugging.

import config from "./config.module.js";
import data_loader from "./dataLoader.module.js";
import { instanceShow, hideAll, showAll } from "./my_utils.js";
import { groupBy } from "./initScene.module.js";
import { state } from "./state.module.js";

state.viewer = new Potree.Viewer(document.getElementById("potree_render_area"));
state.scene = state.viewer.scene;

// classic-script + console access
window.viewer = state.viewer;
window.scene = state.scene;
window.instanceShow = instanceShow;
window.hideAll = hideAll;
window.showAll = showAll;
window.groupBy = groupBy;

state.viewer.setEDLEnabled(false); // disable so transparency in the meshes works fine
state.viewer.setFOV(60);
state.viewer.setPointBudget(2_000_000);
state.viewer.loadSettingsFromURL();
state.viewer.setFilterFloatArray([]);

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
    console.log('app starts');
    var configSettings = config();
    configSettings.cellData["name"] = "cellData";
    make_package([configSettings.cellData]);
}

function make_package(result) {
    var workPackage = result.reduce((a, b) => a.concat(b), []);
    workPackage.forEach(d => d.root_name = strip_url(d.name));
    workPackage.forEach(d => d.bytes_streamed = 0);  // bytes streamed so far
    workPackage.forEach(d => d.data = []);            // accumulated rows from the flatfiles
    workPackage.forEach(d => d.data_length = 0);      // number of rows fetched so far
    data_loader(workPackage);

    console.log(result);
}

function strip_url(d) {
    var fName = d.substring(d.lastIndexOf('/') + 1);
    return fName.split('.')[0];
}