const AjaxHandler = {
  
    submitForm: function (form, successCallback, errorCallback) {
        const formData = new FormData(form);
        const url = form.action;
        const method = form.method;

        fetch(url, {
            method: method,
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }

                 const contentType = response.headers.get("content-type");
                if (contentType && contentType.includes("application/json")) {
                    return response.json();
                } else {
                    return response.text();
                }
            })
            .then(data => {
                if (typeof successCallback === 'function') {
                    successCallback(data);
                }
            })
            .catch(error => {
                console.error("AJAX Error:", error);
                if (typeof errorCallback === 'function') {
                    errorCallback(error);
                }
            });
    },

    loadContent: function (url, targetSelector, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const fullUrl = queryString ? `${url}?${queryString}` : url;

        fetch(fullUrl, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => response.text())
            .then(html => {
                const targetElement = document.querySelector(targetSelector);
                if (targetElement) {
                    targetElement.innerHTML = html;
                     const scripts = targetElement.querySelectorAll('script');
                    scripts.forEach(script => {
                        const newScript = document.createElement('script');
                        newScript.textContent = script.textContent;
                        document.body.appendChild(newScript);
                        document.body.removeChild(newScript);
                    });
                }
                 history.pushState({}, '', fullUrl);
            })
            .catch(error => console.error("Content loading error:", error));
    },


    initPagination: function (containerSelector, paginationSelector) {
        document.addEventListener('click', function (e) {
            const paginationLink = e.target.closest(`${paginationSelector} a`);
            if (paginationLink) {
                e.preventDefault();
                const url = paginationLink.getAttribute('href');
                AjaxHandler.loadContent(url, containerSelector);
            }
        });
    },

    initFormSubmission: function (formSelector, resultContainerSelector) {
        document.addEventListener('submit', function (e) {
            const form = e.target.closest(formSelector);
            if (form) {
                e.preventDefault();

                 const resultContainer = document.querySelector(resultContainerSelector);
                if (resultContainer) {
                    resultContainer.innerHTML = '<div class="text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
                }

                AjaxHandler.submitForm(form,
                    function (response) {
                         if (typeof response === 'string') {
                             if (resultContainer) {
                                resultContainer.innerHTML = response;
                            }
                        } else {
                             if (response.redirectUrl) {
                                AjaxHandler.loadContent(response.redirectUrl, resultContainerSelector);
                            } else if (response.success) {
                                if (resultContainer) {
                                    resultContainer.innerHTML = `<div class="alert alert-success">${response.message || 'Operation completed successfully!'}</div>`;
                                }
                            } else {
                                if (resultContainer) {
                                    resultContainer.innerHTML = `<div class="alert alert-danger">${response.message || 'An error occurred.'}</div>`;
                                }
                            }
                        }
                    },
                    function (error) {
                         if (resultContainer) {
                            resultContainer.innerHTML = `<div class="alert alert-danger">An error occurred: ${error.message}</div>`;
                        }
                    }
                );
            }
        });
    }


};

document.addEventListener('DOMContentLoaded', function () {
     AjaxHandler.initFormSubmission('form[data-ajax="true"]', '#main-content');

     AjaxHandler.initPagination('#main-content', '.pagination');

     document.addEventListener('click', function (e) {
        const ajaxLink = e.target.closest('a[data-ajax="true"]');
        if (ajaxLink) {
            e.preventDefault();
            const url = ajaxLink.getAttribute('href');
            const target = ajaxLink.getAttribute('data-target') || '#main-content';
            AjaxHandler.loadContent(url, target);
        }
    });

     document.addEventListener('click', function (e) {
        const deleteBtn = e.target.closest('.department-delete-btn');
        if (deleteBtn) {
            e.preventDefault();
            const id = deleteBtn.dataset.id;
            const name = deleteBtn.dataset.name;

             if (typeof window.deleteDepartment === 'function' &&
                document.getElementById('deleteModal')) {
                 window.deleteDepartment(id, name);
            } else {
                 directDeleteDepartment(id, name);
            }
        }
    });
});
function deleteEmployee(id, fullName) {
    console.log("Attempting to delete employee with ID:", id);

    if (confirm(`Are you sure you want to delete ${fullName}?`)) {
         fetch(`/Employee/Delete/${id}`, {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                console.log("Response status:", response.status);
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("Delete response:", data);
                if (data.success) {
                     const mainContent = document.querySelector('#main-content');
                    if (mainContent) {
                        const alertDiv = document.createElement('div');
                        alertDiv.className = 'alert alert-success alert-dismissible fade show';
                        alertDiv.innerHTML = `
                        ${data.message}
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    `;
                        mainContent.prepend(alertDiv);

                         AjaxHandler.loadContent('/Employee/Index', '#main-content');
                    }
                } else {
                    alert(`Error: ${data.message}`);
                }
            })
            .catch(error => {
                console.error('Delete error:', error);
                alert('An error occurred while trying to delete the employee.');
            });
    }
}

function directDeleteDepartment(id, name) {
    console.log("Attempting to delete department with ID:", id);

    if (confirm(`Are you sure you want to delete department "${name}"?`)) {
        fetch(`/Department/Delete/${id}`, {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                console.log("Response status:", response.status);
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("Delete response:", data);
                if (data.success) {
                     const mainContent = document.querySelector('#main-content');
                    if (mainContent) {
                        const alertDiv = document.createElement('div');
                        alertDiv.className = 'alert alert-success alert-dismissible fade show';
                        alertDiv.innerHTML = `
                        ${data.message}
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    `;
                        mainContent.prepend(alertDiv);

                         AjaxHandler.loadContent('/Department/Index', '#main-content');
                    }
                } else {
                    alert(`Error: ${data.message}`);
                }
            })
            .catch(error => {
                console.error('Delete error:', error);
                alert('An error occurred while trying to delete the department.');
            });
    }
}
 