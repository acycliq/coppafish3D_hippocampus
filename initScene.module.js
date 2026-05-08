import * as THREE from "./libs/three.js/build/three.module.js";
import make_cells from "./stage_cells.module.js";
import {tree, myjsTree} from "./stage_cells.module.js";
import initLights from "./lights.module.js";
import { state } from "./state.module.js";

var last_visited = 0
function initScene(cellData){
    state.cells = make_cells(cellData)
    state.viewer.scene.scene.add(state.cells.front_face.instancedMesh);

    initLights()

    // state.viewer.renderer.domElement.addEventListener('mousemove', onMouseMove, false)
    // add a tiny throttle. The callback onMouseMove wont be called until after 5milliseconds
    // have passed.
    state.viewer.renderer.domElement.addEventListener('mousemove', throttle(onMouseMove, 5));


    attachTreeControl(cellData)

    // all done, remove the preloader
    removePreloader()

    clearScreen()

}

function attachTreeControl(cellData){
    var classNames = cellData.map(d => d.top_class).filter(d=>d >= 0)
    classNames = [...new Set(classNames)];
    classNames = classNames.sort()
    var treeData = tree(classNames)
    myjsTree(treeData)
}

function throttle(callback, interval) {
    // From https://programmingwithmosh.com/javascript/javascript-throttle-and-debounce-patterns/
    // similar to: https://stackoverflow.com/questions/23181243/throttling-a-mousemove-event-to-fire-no-more-than-5-times-a-second
  let enableCall = true;

  return function(...args) {
    if (!enableCall) return;

    enableCall = false;
    callback.apply(this, args);
    setTimeout(() => enableCall = true, interval);
  }
}

export function groupBy(array, key){
    // from https://learnwithparam.com/blog/how-to-group-by-array-of-objects-using-a-key/
    // Return the end result
    return array.reduce((result, currentValue) => {
        // If an array already present for key, push it to the array. Else create an array and push the object
        (result[currentValue[key]] = result[currentValue[key]] || []).push(
            currentValue
        );
        // Return the current iteration `result` value, this will be taken as next iteration `result` value and accumulate
        return result;
    }, {}); // empty object is the initial value for result object
};

function onMouseMove(event) {
    const mouse = {
        x: (event.clientX / state.viewer.renderer.domElement.clientWidth) * 2 - 1,
        y: -(event.clientY / state.viewer.renderer.domElement.clientHeight) * 2 + 1,
    }

    // console.log(mouse)
    const raycaster = new THREE.Raycaster()

    raycaster.setFromCamera(mouse, state.scene.getActiveCamera())

    const intersects = raycaster.intersectObjects([state.cells.front_face.instancedMesh])

    if (intersects.length > 0) {
        if (intersects[0].distance < 2000){
            var instanceId = state.cellData[intersects[0].instanceId];
            if (last_visited !== instanceId.label){
                // remove the lines from the last visited cell and draw the ones over the new cell
                // I am removing the lines twice. First all the lines (no matter the cell) and then the ones specific to the last cell
                // sometimes I may end up with two cell having lines, hence I am doing this twice. There must be a better way,
                // maybe to throttle the mouse event?
                removeLines()
                // removeLine(last_visited) // I am throttling now, hence this is now redundant
                // $('html,body').css('cursor', 'pointer');
                cellMouseHover(instanceId.label)
                last_visited = instanceId.label
            }
        }
        else {
            $('html,body').css('cursor', 'default');
            clearScreen()
        }
    }
    else {
        // if you are now hovering over any cell, remove any lines you have drawn already
        // and map the last_visited variable to 0 (ie the label for the background)
        removeLines()
        // $('html,body').css('cursor', 'default');
        last_visited = 0
    }
}


function cellMouseHover(label) {
    console.log('Hovering over cell: ' + label)
    // "https://storage.googleapis.com/merfish_data/cellData/"
    // d3.json("./py/cellData/" + label + ".json", outer(label));
    d3.queue()
        // .defer(d3.json, "https://storage.googleapis.com/izzie_sfn/cellData/"+ label + ".json")
        .defer(d3.json, "https://storage.googleapis.com/merfish_data/izzie_sfn/cellData/"+ label + ".json")
        // .defer(d3.json, "./py/cellData/" + label + ".json")
        .defer(d3.csv, "https://storage.googleapis.com/merfish_data/izzie_sfn/colour_scheme/colour_scheme.csv")
        .await(splitArgs(label))
}

function splitArgs(label) {
    return (err, ...args) => {

        var data = args[0];
        var geneColors = args[1];
        var targetCell = state.cellData.filter(d => d.label === label)[0]
        var lines = makeLine(data, targetCell, geneColors)
        lines.map(d => state.viewer.scene.scene.add(d));
        var spots = groupBy(data, 'gene');
        showControls()
        renderDataTable(spots, targetCell)
        donutchart(targetCell)

    }
}

// function outer(label){
//     return function onCellMouseHover(data) {
//         var targetCell = cellData.filter(d => d.label === label)[0]
//         var lines = makeLine(data, targetCell)
//         lines.map(d => state.viewer.scene.scene.add(d));
//         var spots = groupBy(data, 'gene');
//         // $('#dataTableControl').show();
//         // $('#cellCoordsControl').show();
//         renderDataTable(spots, targetCell)
//         donutchart(targetCell)
//     }
// }

function getColor(gene, geneColors){
    var specs = geneColors.filter(d => d.gene == gene)
    if (specs){
        return {'r': +specs[0].r, 'g': +specs[0].g, 'b': +specs[0].b}
    }
    else{
        return {'r': [], 'g': [], 'b': []}
    }
}

function makeLine(obj, targetCell, geneColors){
    var arr = Object.entries(obj).map(d => d[1]).flat()
    arr.forEach(d => d['r'] = getColor(d.gene, geneColors).r)
    arr.forEach(d => d['g'] = getColor(d.gene, geneColors).g)
    arr.forEach(d => d['b'] = getColor(d.gene, geneColors).b)
    var out = arr.map(d => {
        return makeLineHelper(d, targetCell)
    });
    return out
}

function removeLine(label){
    var scene = state.viewer.scene.scene
    scene.children.filter(d => (d.type === "Line") && (d.name === label)).forEach(el => scene.remove(el))
}

function makeLineHelper(spotData, targetCell) {
    var points = [];
    points.push(
        new THREE.Vector3(spotData.x, spotData.y, spotData.z),
        new THREE.Vector3(targetCell.x, targetCell.y, targetCell.z)
    )
    var geometry = new THREE.BufferGeometry().setFromPoints(points);
    // CREATE THE LINE
    var line = new THREE.Line(
        geometry,
        new THREE.LineBasicMaterial({
            color:  new THREE.Color( spotData.r/255.0, spotData.g/255.0, spotData.b/255.0)
        }),
    );
    line.name = targetCell.label
    return line
}

export default initScene