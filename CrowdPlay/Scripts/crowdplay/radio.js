$(function () {
    var audioElement = $('audio#player');
    audioElement.on('ended', function () {
        audioElement.load();
        audioElement.play();
    });
});