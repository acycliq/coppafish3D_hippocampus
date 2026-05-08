// Single mutable wrapper for shared application state.
// Modules import `state` and read/write `state.viewer`, `state.cells`, etc.
// instead of relying on `window.*` globals.
//
// `state` itself is a const binding — what's mutable is its properties.
// That means you can do `state.cells = newCells` from anywhere, and every
// importer sees the new value (live binding through the wrapper object).
export const state = {
    viewer: null,
    scene: null,
    cells: {},
    cellData: [],
};