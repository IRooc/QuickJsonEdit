// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', () => {
    const saveFile = document.getElementById('save-file');

    saveFile.addEventListener('click', (event) => {
        // Download it
        var filename = document.querySelector('h1').innerText;
        var body = document.getElementById('new-json-document').value;
        var link = document.createElement('a');
        link.style.display = 'none';
        link.setAttribute('target', '_blank');
        link.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(body));
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });

    if (document.location.search.endsWith('&Saved=True')) {
        const newUrl = document.location.href.replace('&Saved=True', '')
        window.history.pushState(null, "Updated", newUrl);
        document.querySelector('.saved').classList.add('show');
        setTimeout(() => {
            document.querySelector('.saved').classList.remove('show');
        }, 3000);
    }
});