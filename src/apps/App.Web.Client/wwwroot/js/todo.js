$(document).ready(function () {
  // Initialize datepickers
  initializeDatepickers();

  // Initialize Select2 for tag selection
  initializeSelect2();

  // Handle task title click to open edit modal
  $(document).on("click", ".task-title", function (e) {
    e.preventDefault();
    var taskId = $(this).data("task-id");
    openEditTaskModal(taskId);
  });

  // Handle edit button click
  $(document).on("click", ".edit-task", function (e) {
    e.preventDefault();
    var taskId = $(this).data("task-id");
    openEditTaskModal(taskId);
  });

  // Handle delete button click
  $(document).on("click", ".delete-task", function (e) {
    e.preventDefault();
    var taskId = $(this).data("task-id");
    var taskTitle = $('tr[data-task-id="' + taskId + '"]')
      .find(".task-title")
      .text();

    // Set the task title in the confirmation modal
    $("#deleteTaskTitle").text(taskTitle);

    // Set the task ID for the confirm button
    $("#confirmDeleteBtn").data("task-id", taskId);

    // Show the delete confirmation modal
    $("#deleteTaskModal").modal("show");
  });

  // Handle confirm delete button click
  $("#confirmDeleteBtn").on("click", function () {
    var taskId = $(this).data("task-id");
    deleteTask(taskId);
  });

  // Handle task completion checkbox
  $(document).on("change", ".task-complete-checkbox", function () {
    var taskId = $(this).data("task-id");
    toggleTaskCompletion(taskId);
  });

  // Handle add task form submission
  $("#addTaskForm").on("submit", function (e) {
    e.preventDefault();
    createTask();
  });

  // Handle edit task form submission
  $("#editTaskForm").on("submit", function (e) {
    e.preventDefault();
    updateTask();
  });

  // Initialize plugins when modals are shown
  $("#addTaskModal").on("shown.bs.modal", function () {
    initializeDatepickers();
    initializeSelect2();
  });

  $("#editTaskModal").on("shown.bs.modal", function () {
    initializeDatepickers();
    initializeSelect2();
  });
});

// Initialize datepickers
function initializeDatepickers() {
  $(".datepicker").datepicker({
    format: "mm/dd/yyyy",
    autoclose: true,
    todayHighlight: true,
  });
}

// Initialize Select2 for tag selection
function initializeSelect2() {
  $(".select2-tags").select2({
    tags: true,
    placeholder: "Select or add tags",
    width: "100%",
  });
}

// Open edit task modal and load task data
function openEditTaskModal(taskId) {
  // Reset form
  $("#editTaskForm")[0].reset();

  // Get task data via AJAX
  $.ajax({
    url: "/Todo/GetDetail",
    type: "GET",
    data: { taskId: taskId },
    success: function (task) {
      // Populate form fields with task data
      $("#editTaskId").val(task.id);
      $("#editTaskTitle").val(task.title);
      $("#editTaskDescription").val(task.description);
      $("#editTaskDueDate").datepicker("setDate", new Date(task.dueDate));
      $("#editTaskAssignee").val(task.assignedTo);
      $("#editTaskStatus").prop("checked", task.isCompleted);

      // Format dates for display
      var createdDate = new Date(task.createdAt);
      var updatedDate = new Date(task.updatedAt);
      $("#editTaskCreatedAt").text(formatDate(createdDate));
      $("#editTaskUpdatedAt").text(formatDate(updatedDate));

      // Clear and set tags
      $("#editTaskTags").val(null).trigger("change");
      if (task.tags && task.tags.length > 0) {
        // Add any missing tag options
        task.tags.forEach(function (tag) {
          if ($('#editTaskTags option[value="' + tag + '"]').length === 0) {
            var newOption = new Option(tag, tag, true, true);
            $("#editTaskTags").append(newOption);
          }
        });

        // Set the selected values
        $("#editTaskTags").val(task.tags).trigger("change");
      }

      // Show the modal
      $("#editTaskModal").modal("show");
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to load task details: " + error);
    },
  });
}

// Create a new task
function createTask() {
  var formData = {
    title: $("#newTaskTitle").val(),
    description: $("#newTaskDescription").val(),
    dueDate: $("#newTaskDueDate").val(),
    assignedTo: $("#newTaskAssignee").val(),
    tags: $("#newTaskTags").val() || [],
  };

  $.ajax({
    url: "/Todo/Create",
    type: "POST",
    data: formData,
    success: function (response) {
      if (response.success) {
        // Close the modal
        $("#addTaskModal").modal("hide");

        // Show success message
        toastr.success("Task created successfully");

        // Reload the task list
        loadTodoList();
      } else {
        toastr.error("Failed to create task: " + response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to create task: " + error);
    },
  });
}

// Update an existing task
function updateTask() {
  var formData = {
    id: $("#editTaskId").val(),
    title: $("#editTaskTitle").val(),
    description: $("#editTaskDescription").val(),
    dueDate: $("#editTaskDueDate").val(),
    isCompleted: $("#editTaskStatus").is(":checked"),
    assignedTo: $("#editTaskAssignee").val(),
    tags: $("#editTaskTags").val() || [],
  };

  $.ajax({
    url: "/Todo/Update",
    type: "POST",
    data: formData,
    success: function (response) {
      if (response.success) {
        // Close the modal
        $("#editTaskModal").modal("hide");

        // Show success message
        toastr.success("Task updated successfully");

        // Reload the task list
        loadTodoList();
      } else {
        toastr.error("Failed to update task: " + response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to update task: " + error);
    },
  });
}

// Delete a task
function deleteTask(taskId) {
  $.ajax({
    url: "/Todo/Delete",
    type: "POST",
    data: { id: taskId },
    success: function (response) {
      if (response.success) {
        // Close the modal
        $("#deleteTaskModal").modal("hide");

        // Show success message
        toastr.success("Task deleted successfully");

        // Reload the task list
        loadTodoList();
      } else {
        toastr.error("Failed to delete task: " + response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to delete task: " + error);
    },
  });
}

// Toggle task completion status
function toggleTaskCompletion(taskId) {
  $.ajax({
    url: "/Todo/ToggleComplete",
    type: "POST",
    data: { id: taskId },
    success: function (response) {
      if (response.success) {
        // Show success message
        toastr.success("Task status updated");

        // Reload the task list to reflect changes
        loadTodoList();
      } else {
        toastr.error("Failed to update task status: " + response.message);

        // Reset the checkbox to its previous state
        var $checkbox = $("#taskComplete_" + taskId);
        $checkbox.prop("checked", !$checkbox.prop("checked"));
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to update task status: " + error);

      // Reset the checkbox to its previous state
      var $checkbox = $("#taskComplete_" + taskId);
      $checkbox.prop("checked", !$checkbox.prop("checked"));
    },
  });
}

// Format date for display
function formatDate(date) {
  if (!date) return "N/A";

  var options = {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  };

  return date.toLocaleDateString("en-US", options);
}

// Generic content loader function
function loadContent(options) {
  var defaults = {
    url: "",
    container: "content",
    loader: "contentLoader",
    data: {},
    updateUrl: null,
    errorMessage: "Error loading content",
  };

  var settings = $.extend({}, defaults, options);

  $("#" + settings.loader).show();
  $("#" + settings.container).hide();

  $.ajax({
    url: settings.url,
    type: "GET",
    data: settings.data,
    success: function (data) {
      $("#" + settings.container).html(data);
      $("#" + settings.loader).hide();
      $("#" + settings.container).fadeIn();

      // Update URL if provided
      if (settings.updateUrl) {
        history.pushState(null, null, settings.updateUrl);
      }
    },
    error: function (xhr, status, error) {
      $("#" + settings.loader).hide();
      $("#" + settings.container)
        .html(
          '<div class="alert alert-danger">' +
            settings.errorMessage +
            ": " +
            error +
            "</div>"
        )
        .fadeIn();
    },
  });
}

// Load todo list content with optional filter
function loadTodoList(filter) {
  var updateUrl = filter ? "/Todo/Index?filter=" + filter : null;

  loadContent({
    url: "/Todo/TodoListPartial",
    container: "todoListContent",
    loader: "todoListLoader",
    data: { filter: filter },
    updateUrl: updateUrl,
    errorMessage: "Error loading tasks",
  });
}

// Load dashboard content
function loadDashboard() {
  loadContent({
    url: "/Todo/DashboardPartial",
    container: "dashboardContent",
    loader: "dashboardLoader",
    updateUrl: "/Todo/Dashboard",
    errorMessage: "Error loading dashboard",
  });
}

// Set up navigation
function setupNavigation() {
  // Handle sidebar navigation
  $(".nav-link").on("click", function (e) {
    var href = $(this).attr("href");

    // If it's a Todo section link, handle it
    if (href === "/Todo/Dashboard") {
      e.preventDefault();
      loadDashboard();
      history.pushState(null, null, href);
    } else if (href === "/Todo/Index") {
      e.preventDefault();
      loadTodoList();
      history.pushState(null, null, href);
    }
  });
}
