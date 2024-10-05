document.addEventListener('DOMContentLoaded', function () {
    // 處理父選單的點擊
    var toggleLinks = document.querySelectorAll('.toggle-submenu');

    toggleLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            var submenu = this.nextElementSibling.querySelector('.submenu');

            // 隱藏所有其他子選單
            document.querySelectorAll('.submenu').forEach(function (sub) {
                if (sub !== submenu) {
                    sub.style.display = 'none';
                }
            });

            // 切換當前子選單的可見性
            submenu.style.display = (submenu.style.display === "block") ? "none" : "block";
        });
    });

    // 設定 active 的功能選項
    const sidebarItems = document.querySelectorAll('.nav-link');

    sidebarItems.forEach(item => {
        item.addEventListener('click', function (event) {
            // 防止點擊事件傳遞到子選項
            event.stopPropagation();

            // 移除所有選項的 'active' 類別
            sidebarItems.forEach(el => el.classList.remove('active'));
            document.querySelectorAll('.nav-item').forEach(navItem => {
                navItem.classList.remove('active');
                const submenuWrapper = navItem.querySelector('.submenu-wrapper');
                if (submenuWrapper) {
                    submenuWrapper.style.display = 'none';
                }
            });

            // 為當前點擊的選項和父選項添加 'active' 類別
            this.classList.add('active');
            const parentNavItem = this.closest('.nav-item');
            if (parentNavItem) {
                parentNavItem.classList.add('active');
                const submenuWrapper = parentNavItem.querySelector('.submenu-wrapper');
                if (submenuWrapper) {
                    submenuWrapper.style.display = 'block'; // 展開父選單
                }
            }
        });
    });

    // 根據當前 URL 設置預設的 active 項目，並展開父選單
    const currentUrl = window.location.pathname;
    sidebarItems.forEach(item => {
        const linkUrl = item.getAttribute('href');
        if (currentUrl === linkUrl) {
            item.classList.add('active');
            const parentNavItem = item.closest('.nav-item');
            if (parentNavItem) {
                parentNavItem.classList.add('active');
                const submenuWrapper = parentNavItem.querySelector('.submenu-wrapper');
                if (submenuWrapper) {
                    submenuWrapper.style.display = 'block'; // 展開父選單
                }
            }

            // 如果這是子選單，展開父選單
            const grandParentNavItem = parentNavItem.closest('.nav-item');
            if (grandParentNavItem) {
                const grandParentWrapper = grandParentNavItem.querySelector('.submenu-wrapper');
                if (grandParentWrapper) {
                    grandParentWrapper.style.display = 'block'; // 確保祖父選單也展開
                }
            }
        }
    });
});

// 側邊欄元素
const sidebar = document.getElementById('sidebar');
const content = document.getElementById('main-content-wrapper');
const topBar = document.getElementById('top-bar');
let submenuOpen = false;  // 是否有子選單打開

// 當鼠標移動到側邊欄時展開，移開時縮回
sidebar.addEventListener('mouseenter', function () {
    sidebar.classList.add('expanded');
    content.classList.add('expanded');
    topBar.classList.add('expanded');
});

sidebar.addEventListener('mouseleave', function () {
    // 檢查子選單是否打開，若子選單未打開則收起側邊欄
    if (!submenuOpen) {
        sidebar.classList.remove('expanded');
        content.classList.remove('expanded');
        topBar.classList.remove('expanded');
    }
});

// 監聽子選單的點擊事件
document.querySelectorAll('.toggle-submenu').forEach(item => {
    item.addEventListener('click', function () {
        // 切換子選單的顯示
        const submenu = this.nextElementSibling;
        submenu.classList.toggle('expanded');

        // 如果有子選單展開，設置 `submenuOpen = true`
        submenuOpen = submenu.classList.contains('expanded');
    });
});
