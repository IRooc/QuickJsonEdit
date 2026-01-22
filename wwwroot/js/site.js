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

    if (document.location.search.endsWith('Saved=True')) {
        const newUrl = document.location.href.replace(/[\?&]Saved=True/, '')
        window.history.pushState(null, "Updated", newUrl);
        document.querySelector('.saved').classList.add('show');
        setTimeout(() => {
            document.querySelector('.saved').classList.remove('show');
        }, 3000);
    }


    if (document.location.search.startsWith('?key=')) {
        const textarea = document.querySelector('textarea[name="newValue"]');
        if (textarea) {
            textarea.focus();
            // Set cursor to the end of the content
            const length = textarea.value.length;
            textarea.setSelectionRange(length, length);
        }
    }

	setupCustomDropdown();
});


// Custom dropdown for key choice
function setupCustomDropdown() {
    const input = document.getElementById('key-choice');
    const dropdown = document.getElementById('key-list');

    function showDropdown() {
        dropdown.classList.add('show');
    }

    function hideDropdown() {
        dropdown.classList.remove('show');
    }

    function setInputValue(value) {
        input.value = value;
        hideDropdown();
        // Submit form immediately to enter edit mode
        document.getElementById('key-form').submit();
    }

    // Show dropdown when input is focused or clicked
    input.addEventListener('focus', () => {
        showDropdown();
    });
    input.addEventListener('click', (e) => {
        e.stopPropagation();
        showDropdown();
    });

    // Hide dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (!dropdown.contains(e.target) && e.target !== input) {
            hideDropdown();
        }
    });

    // Handle clicking on dropdown items
    dropdown.querySelectorAll('.dropdown-item').forEach((item) => {
        item.addEventListener('click', () => {
            setInputValue(item.textContent);
        });
    });

    // Keyboard navigation for input and dropdown items
    let focusedItemIndex = -1;
    const items = dropdown.querySelectorAll('.dropdown-item');

    input.addEventListener('keydown', (e) => {
        if (e.key === 'ArrowDown') {
            e.preventDefault();
            if (items.length > 0) {
                focusedItemIndex = 0;
                items[0].focus();
                showDropdown();
            }
        } else if (e.key === 'Escape') {
            hideDropdown();
        }
    });

    items.forEach((item, index) => {
        item.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                setInputValue(item.textContent);
            } else if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (index < items.length - 1) {
                    items[index + 1].focus();
                }
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (index > 0) {
                    items[index - 1].focus();
                } else {
                    input.focus();
                }
            } else if (e.key === 'Escape') {
                hideDropdown();
                input.focus();
            }
        });
    });
}