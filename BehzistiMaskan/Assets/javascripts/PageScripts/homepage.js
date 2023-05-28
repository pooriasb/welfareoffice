var effects;
function activeVantaBirds() {
    if (effects != null) destroyEffect();
    effects = VANTA.BIRDS({
        el: "#main",
        colorMode: "lerp"
    });
}
function activeVantaNet() {
    if (effects != null) destroyEffect();
    effects = VANTA.NET({
        el: "#main"
    });
}
function activeVantaWaves() {
    if (effects != null) destroyEffect();
    effects = VANTA.WAVES({
        el: "#main"
    });
}
function destroyEffect() {
    effects.destroy();
}
window.onload = function () {
    activeVantaWaves();
};