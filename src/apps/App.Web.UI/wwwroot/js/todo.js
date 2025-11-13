$(document).ready(function () {
  const cfg = window.todoConfig;
  console.log(cfg);
});

// function deleteTodo(i) {
//   $.ajax({
//     url: "Todo/Delete",
//     type: "POST",
//     data: {
//       id: i,
//     },
//     success: function () {
//       window.location.reload();
//     },
//   });
// }

// function populateForm(i) {
//   $.ajax({
//     url: "Todo/PopulateForm",
//     type: "GET",
//     data: {
//       id: i,
//     },
//     dataType: "json",
//     success: function (response) {
//       $("#ItemName").val(response.itemName);
//       $("#RowKey").val(response.rowKey);
//       $("#form-button").val("Update Todo");
//       $("#form-action").attr("action", "/Todo/Update");
//     },
//   });
// }

// $("input[type='checkbox']").change(function () {
//   let i = $(this).attr("id");
//   $.ajax({
//     url: "Todo/CompleteItem",
//     type: "POST",
//     data: {
//       id: i,
//     },
//     success: function () {
//       window.location.reload();
//     },
//   });
// });
