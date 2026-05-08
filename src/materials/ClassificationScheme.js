
import * as THREE from "../../libs/three.js/build/three.module.js";

export const ClassificationScheme = {

// 	DEFAULT: {
// 		0: {visible: true, name: 'never classified', color: [0.5, 0.5, 0.5, 1.0]},
// 		1: {visible: true, name: 'unclassified', color: [0.5, 0.5, 0.5, 1.0]},
// 		2: {visible: true, name: 'ground', color: [0.63, 0.32, 0.18, 1.0]},
// 		3: {visible: true, name: 'low vegetation', color: [0.0, 1.0, 0.0, 1.0]},
// 		4: {visible: true, name: 'medium vegetation', color: [0.0, 0.8, 0.0, 1.0]},
// 		5: {visible: true, name: 'high vegetation', color: [0.0, 0.6, 0.0, 1.0]},
// 		6: {visible: true, name: 'building', color: [1.0, 0.66, 0.0, 1.0]},
// 		7: {visible: true, name: 'low point(noise)', color: [1.0, 0.0, 1.0, 1.0]},
// 		8: {visible: true, name: 'key-point', color: [1.0, 0.0, 0.0, 1.0]},
// 		9: {visible: true, name: 'water', color: [0.0, 0.0, 1.0, 1.0]},
// 		12: {visible: true, name: 'overlap', color: [1.0, 1.0, 0.0, 1.0]},
// 		DEFAULT: {visible: true, name: 'default', color: [0.3, 0.6, 0.6, 0.5]},
// 	}
// };
	DEFAULT: {
		0: {visible: true, name: 'asterisk', color: [0.3, 0.6, 0.6, 0.5]},
		1: {visible: true, name: 'circle', color: [0.3, 0.6, 0.6, 0.5]},
		2: {visible: true, name: 'cross', color: [0.3, 0.6, 0.6, 0.5]},
		3: {visible: true, name: 'diamond', color: [0.3, 0.6, 0.6, 0.5]},
		4: {visible: true, name: 'plus', color: [0.3, 0.6, 0.6, 0.5]},
		5: {visible: true, name: 'point', color: [0.3, 0.6, 0.6, 0.5]},
		6: {visible: true, name: 'square', color: [0.3, 0.6, 0.6, 0.5]},
		7: {visible: true, name: 'star5', color: [0.3, 0.6, 0.6, 0.5]},
		8: {visible: true, name: 'star6', color: [0.3, 0.6, 0.6, 0.5]},
		9: {visible: true, name: 'triangleDown', color: [0.3, 0.6, 0.6, 0.5]},
		10: {visible: true, name: 'triangleLeft', color: [0.3, 0.6, 0.6, 0.5]},
		11: {visible: true, name: 'triangleRight', color: [0.3, 0.6, 0.6, 0.5]},
		12: {visible: true, name: 'triangleUp', color: [0.3, 0.6, 0.6, 0.5]},
		DEFAULT: {visible: true, name: 'default', color: [0.3, 0.6, 0.6, 0.5]}
	}
};

Object.defineProperty(ClassificationScheme, 'RANDOM', {
	get: function() { 

		let scheme = {};

		for(let i = 0; i <= 255; i++){
			scheme[i] = new THREE.Vector4(Math.random(), Math.random(), Math.random());
		}

		scheme["DEFAULT"] = new THREE.Vector4(Math.random(), Math.random(), Math.random());

		return scheme;
	}
});