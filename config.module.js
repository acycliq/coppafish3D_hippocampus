var config = function () {
    return {
            // === aang dataset (active) ===
            cellData: {mediaLink: './data/cellData_rgb_aang.tsv', size: "36243376"},
            // Spike-lines data: one binary blob + a tiny index, fetched lazily via HTTP Range.
            // The blob lives on GCS (Range + CORS verified); index stays local because it's small.
            spotsBin:   'https://storage.googleapis.com/aang_data/potree_viewer/cell_spots_aang.bin',
            // spotsBin: './data/cell_spots_aang.bin',  // local fallback
            spotsIndex: './data/cell_spots_aang_index.json',
            roi: {"x0": 0, "x1": 6431, "y0": 0, "y1": 8544 },
            zoomLevels: 10,
         // Multiplier applied to pciSeq's `sphere_scale` (= 3 × sqrt(eigvals) of
            // the cell's covariance — i.e. 3σ radii). Use this to visually shrink
            // or grow the cell ellipsoids without re-running pciSeq.
            //   1     → 3σ (default, ~99.7% coverage of the fitted gaussian)
            //   1/3   → 1σ (radius = stdev along each principal axis)
            //   0.5   → 1.5σ
            //   2     → 6σ (visibly fatter, useful for debug)
            sphereScale: 1/3,
            // background tiles for aang not yet available (would come from output.mbtiles).
            // Leave tiles undefined so the viewer doesn't try to fetch the wrong image.
            // tiles: '',

            // === hippocampus dataset (commented out — flip with the block above to revert) ===
            // cellData: {mediaLink: './data/cellData_rgb.tsv', size: "11476992"},
            // roi: {"x0": 0, "x1": 7602, "y0": 0, "y1": 5471 },
            // zoomLevels: 10,
            // tiles: 'https://storage.googleapis.com/ca1-data/img/262144px/{z}/{y}/{x}.jpg',
        }
};
export default config;

