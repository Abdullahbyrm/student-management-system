// ============================================================
// STUDENT MANAGEMENT SYSTEM — Premium Interactivity
// ============================================================

document.addEventListener('DOMContentLoaded', () => {

    // ---------- SIDEBAR TOGGLE ----------
    const sidebar    = document.getElementById('sidebar');
    const overlay    = document.getElementById('sidebarOverlay');
    const toggleBtn  = document.getElementById('sidebarToggle');

    if (toggleBtn) {
        toggleBtn.addEventListener('click', () => {
            sidebar.classList.toggle('open');
            overlay.classList.toggle('active');
        });
    }
    if (overlay) {
        overlay.addEventListener('click', () => {
            sidebar.classList.remove('open');
            overlay.classList.remove('active');
        });
    }

    // ---------- DARK / LIGHT THEME TOGGLE ----------
    const themeBtn = document.getElementById('themeToggle');
    const saved = localStorage.getItem('theme');
    if (saved) document.documentElement.setAttribute('data-theme', saved);

    if (themeBtn) {
        updateThemeIcon();
        themeBtn.addEventListener('click', () => {
            const current = document.documentElement.getAttribute('data-theme');
            const next = current === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            localStorage.setItem('theme', next);
            updateThemeIcon();
        });
    }

    function updateThemeIcon() {
        if (!themeBtn) return;
        const icon = themeBtn.querySelector('i');
        if (!icon) return;
        const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        icon.className = isDark ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
    }

    // ---------- TABLE SEARCH / FILTER ----------
    const searchInput = document.getElementById('tableSearch');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const query = this.value.toLowerCase();
            const rows  = document.querySelectorAll('.premium-table tbody tr');
            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(query) ? '' : 'none';
            });
        });
    }

    // ---------- NAVBAR LIVE SEARCH (AJAX) ----------
    const navSearch = document.getElementById('navbarSearch');
    const dropdown  = document.getElementById('searchDropdown');
    let searchTimer = null;

    if (navSearch && dropdown) {
        navSearch.addEventListener('input', function () {
            clearTimeout(searchTimer);
            const q = this.value.trim();
            if (q.length < 2) {
                dropdown.classList.remove('show');
                dropdown.innerHTML = '';
                return;
            }
            searchTimer = setTimeout(() => {
                fetch('/Students/Search?q=' + encodeURIComponent(q))
                    .then(res => res.json())
                    .then(data => {
                        if (data.length === 0) {
                            dropdown.innerHTML = '<div class="search-no-result"><i class="bi bi-search me-1"></i>Sonuç bulunamadı</div>';
                        } else {
                            dropdown.innerHTML = data.map(s => {
                                const initials = getInitials(s.name);
                                const avatar = s.photoPath
                                    ? '<img src="' + s.photoPath + '" class="student-photo-search" />'
                                    : '<span class="row-avatar av-blue" style="width:34px;height:34px;font-size:0.75rem;">' + initials + '</span>';
                                return '<a href="/Students/Details/' + s.studentId + '" class="search-result-item">'
                                    + avatar
                                    + '<div><div class="search-result-name">' + s.name + '</div>'
                                    + '<div class="search-result-sub">' + s.email + ' · ' + s.course + '</div></div></a>';
                            }).join('');
                        }
                        dropdown.classList.add('show');
                    })
                    .catch(() => {
                        dropdown.classList.remove('show');
                    });
            }, 300);
        });

        // Dışarı tıklayınca kapat
        document.addEventListener('click', (e) => {
            if (!navSearch.contains(e.target) && !dropdown.contains(e.target)) {
                dropdown.classList.remove('show');
            }
        });

        navSearch.addEventListener('focus', () => {
            if (dropdown.innerHTML.trim() !== '' && navSearch.value.trim().length >= 2) {
                dropdown.classList.add('show');
            }
        });
    }

    function getInitials(name) {
        const parts = name.split(' ');
        if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase();
        return name.substring(0, 2).toUpperCase();
    }

    // ---------- PHOTO UPLOAD PREVIEW ----------
    const photoInput = document.getElementById('photoInput');
    const previewBox = document.getElementById('photoPreview');
    if (photoInput && previewBox) {
        photoInput.addEventListener('change', function () {
            previewBox.innerHTML = '';
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    previewBox.innerHTML = '<img src="' + e.target.result + '" /><span style="font-size:0.82rem;color:var(--text-secondary);">' + photoInput.files[0].name + '</span>';
                };
                reader.readAsDataURL(this.files[0]);
            }
        });
    }

    // ---------- TOAST NOTIFICATION SYSTEM ----------
    window.showToast = function (message, type = 'success') {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        const toast = document.createElement('div');
        toast.className = 'toast-msg' + (type === 'error' ? ' error' : '');
        const icon = type === 'error'
            ? '<i class="bi bi-exclamation-circle-fill" style="color:var(--danger)"></i>'
            : '<i class="bi bi-check-circle-fill" style="color:var(--accent)"></i>';
        toast.innerHTML = icon + '<span>' + message + '</span>';
        container.appendChild(toast);
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(80px)';
            toast.style.transition = 'all 0.3s ease';
            setTimeout(() => toast.remove(), 350);
        }, 3500);
    };

    // ---------- FADE-IN ANIMATIONS ON LOAD ----------
    const animatedEls = document.querySelectorAll('.fade-in-up');
    animatedEls.forEach((el, i) => {
        el.style.animationDelay = (i * 0.07) + 's';
    });

    // ---------- ACTIVE SIDEBAR LINK HIGHLIGHT ----------
    const currentPath = window.location.pathname.toLowerCase();
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    sidebarLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (!href) return;
        const linkPath = href.toLowerCase();
        if (currentPath === linkPath || (linkPath !== '/' && currentPath.startsWith(linkPath))) {
            link.classList.add('active');
        }
    });
    if (currentPath === '/' || currentPath === '') {
        const dashLink = document.querySelector('.sidebar-link[href="/"]');
        if (dashLink) dashLink.classList.add('active');
    }
});
