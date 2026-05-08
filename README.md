# izzie_sfn — 3D viewer

Interactive 3D viewer for merfish / SFN spatial-transcriptomics data, built on
top of [Potree](https://github.com/potree/potree) (a WebGL point-cloud viewer
by Markus Schütz).

Live: <https://acycliq.github.io/izzie_sfn/>

Gene-spot mRNA detections are rendered as a Potree point cloud (millions of
points). Cells are rendered on top as instanced ellipsoid meshes coloured by
their top classification. Hovering over a cell pulls per-cell spot data from
Google Cloud Storage and draws lines from each spot to the cell centroid; a
linked DataTables grid shows gene counts and a donut chart shows class
probabilities.

## Stack

- [Potree](https://github.com/potree/potree) v1.8.0 (vendored under `src/`,
  `libs/`, `build/`) for the point-cloud renderer.
- [three.js](https://threejs.org/) (`libs/three.js/`) for cells / lines / lights.
- d3 v4, jQuery, DataTables, jsTree, jQuery-UI for UI.
- `streaming-tsv-parser.js` (web worker) for the cell-data TSV stream.
- A mix of ES modules (`*.module.js`) and classic `<script>` tags. The
  ES-module entry point is the inline `<script type="module">` at the bottom
  of `index.html`.

## Layout

```
index.html               entry point — inline module script orchestrates the viewer
*.module.js              ES modules: config, dataLoader, initScene, lights, stage_cells
donut.js, dt.js          d3 donut chart and DataTables grid (classic scripts)
classConfig.js           class → colour mapping
glyphConfig.js, glyphPaths.js   per-gene glyph styling
73_gene_colour_scheme.js gene colour scheme
my_utils.js              instanced-mesh helpers
streaming-tsv-parser.js  web worker that streams a TSV in chunks
data/                    (gitignored) local copy of the merfish TSVs
pointclouds/             (gitignored) Potree octree binaries — too big for git
src/, libs/, build/      vendored Potree (don't hand-edit)
resources/               Potree resources (icons, textures, fonts)
py/, *.ipynb             Python helpers and notebooks for preparing the data
```

## Running locally

It's a static site. Any local web server works; `file://` will not because
the app fetches TSVs and JSON over `fetch()`.

```bash
python3 -m http.server 8000
# then open http://localhost:8000
```

For local development you need the dataset on disk. The `data/` and
`pointclouds/` directories are git-ignored on purpose — the files are large
and the deployed site fetches them from a Google Cloud bucket. To run the
viewer locally, obtain the dataset separately and drop it under `data/` and
`pointclouds/`.

## Data

- `data/cellData_rgb.tsv` — segmented cells with 3D position, ellipsoid
  shape, RGB tint, gene counts, and class probabilities. Streamed at startup.
- `pointclouds/sfn/octree/` — Potree octree of the gene spots
  (`metadata.json` + `octree.bin` + chunks). Loaded by `Potree.loadPointCloud`.
- Hover-time per-cell spot data comes from
  `https://storage.googleapis.com/merfish_data/izzie_sfn/cellData/<label>.json`,
  with the colour scheme at
  `https://storage.googleapis.com/merfish_data/izzie_sfn/colour_scheme/colour_scheme.csv`.

## Building Potree

The Potree library itself has its own build step (gulp + rollup). This is
only needed if you change the Potree internals under `src/`.

```bash
npm install   # postinstall runs `npm run build`
npm run build # gulp build pack
npm start     # gulp watch — local dev with rebuild
```

The application code at the repo root is **not** bundled — it's served raw
as ES modules and classic scripts. No build step needed for normal app work.

## Deployment

Static hosting on GitHub Pages. All asset paths in `index.html` are relative.
At runtime, dataset files come from `./data/` and `./pointclouds/` locally,
or from Google Cloud Storage in production.