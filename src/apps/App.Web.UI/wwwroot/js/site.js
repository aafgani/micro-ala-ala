// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// This ensures Pace restarts on every AJAX request
$(document).ajaxStart(function () {
  if (window.Pace) {
    Pace.restart();
  }
});

// Show the overlay
function showCardOverlay(card) {
  $(card).find(".overlay").show();
}
// Hide the overlay
function hideCardOverlay(card) {
  $(card).find(".overlay").fadeOut();
}
