$(function () {
    $('#searchBox').on('input', function () {
        var keyword = $(this).val();
        $.get('/Employees/Index?handler=Search', { keyword: keyword }, function (data) {
            var tbody = $('#employeesTable tbody');
            tbody.empty();
            data.forEach(function (emp) {
                var row = `<tr>
                    <td>${emp.firstName}</td>
                    <td>${emp.lastName}</td>
                    <td>${emp.firstName} ${emp.lastName}</td>
                    <td>${emp.salary}</td>
                    <td>${emp.managerName || ''}</td>
                    <td>${emp.imageUrl ? `<img src="${emp.imageUrl}" width="50" />` : ''}</td>
                    <td>
                        <a href="/Employees/Edit/${emp.id}">Edit</a> |
                        <a href="/Employees/Delete/${emp.id}">Delete</a>
                    </td>
                </tr>`;
                tbody.append(row);
            });
        });
    });
});
