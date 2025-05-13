$(document).ready(function () {

    $('.task-title').on('click', function (e) {
        e.preventDefault();
        var taskId = $(this).data('task-id');

        // Load partial, then init modal and plugins
        $('#detailContent').load('/Todo/GetDetail?taskId=' + taskId, function (response, status, xhr) {
            if (status === "error") {
                alert("Failed to load details: " + xhr.status + " " + xhr.statusText);
            } else {
                $('#editTaskModal').modal('show');
            }
        });
    });

     //When modal is fully visible
     //This must be outside load, and always attached
    $('#editTaskModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        setTimeout(() => {
            initializeEditModalPlugins();
        }, 100); // Delay allows layout/render to settle
    });

    function initializeEditModalPlugins() {
        const $notes = $('#notes');

        // initialize
        $('#notes').summernote({
            height: 200,   // set editor height
            placeholder: 'Write your notes here...',
            toolbar: [
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['insert', ['link']],
                ['view', ['codeview']]
            ]
        });

        $('#todoTags').select2({
            tags: true,
            placeholder: "Select or add tags",
            width: '100%'
        });

        // Datepicker
        $('input[id="dueDate"]').datepicker({
            format: 'dd/mm/yyyy',   // Match your date format
            autoclose: true,
            todayHighlight: true
        }).on('changeDate', function () {
            console.log("Date selected: " + $(this).val());
        });

        // Add new step
        document.getElementById('add-step').addEventListener('click', function () {
            const stepContainer = document.getElementById('steps-container');
            const newStep = document.createElement('div');
            newStep.classList.add('input-group', 'mb-3');
            newStep.innerHTML = `
          <input type="text" class="form-control" placeholder="New Step" name="step[]">
          <div class="input-group-append">
            <button class="btn btn-danger remove-step" type="button">Remove</button>
          </div>
        `;
            stepContainer.appendChild(newStep);

            // Add event listener to remove button for each new step
            newStep.querySelector('.remove-step').addEventListener('click', function () {
                stepContainer.removeChild(newStep);
            });
        });
    }
});