
document.addEventListener('DOMContentLoaded', function () {

    const notifications = document.querySelectorAll('.notification-item');
    notifications.forEach(function (notification) {
        notification.addEventListener('click', function () {
            notification.classList.add('read');
            notification.classList.remove('unread');
        });
    });


    const tabToggles = document.querySelectorAll('.tab-toggle');
    tabToggles.forEach(toggle => {
        toggle.addEventListener('change', function() {
            const tabContainer = this.closest('.box');
            const tabContents = tabContainer.querySelectorAll('.tab-content');
            tabContents.forEach(content => content.style.display = 'none');
            
            const activeContent = tabContainer.querySelector(`.tab-content:nth-child(${this.id.replace('tab', '')})`);
            if (activeContent) {
                activeContent.style.display = 'block';
            }
        });
    });


    const firstTabs = document.querySelectorAll('.tab-toggle:first-child');
    firstTabs.forEach(tab => {
        const tabContainer = tab.closest('.box');
        const firstContent = tabContainer.querySelector('.tab-content:first-child');
        if (firstContent) {
            firstContent.style.display = 'block';
        }
    });
});

$(document).ready(function() {

    $('#notificationButton').click(function(e) {
        e.stopPropagation();
        $('#notificationContent').toggle();
        loadNotifications();
    });


    $(document).click(function() {
        $('#notificationContent').hide();
    });


    function loadNotifications() {
        $.get('/TimeSheets/GetNotifications', function(data) {
            var notificationList = $('#notificationList');
            notificationList.empty();
            
            if (data.length === 0) {
                notificationList.append('<li>No notifications</li>');
                return;
            }

            var unreadCount = 0;
            
            data.forEach(function(notification) {
                var itemClass = notification.read ? 'read' : 'unread';
                if (!notification.read) unreadCount++;
                
                var item = $(`
                    <li class="notification-item ${itemClass}" data-id="${notification.notification_ID}">
                        ${notification.message}
                        <span class="time">${new Date(notification.time).toLocaleString()}</span>
                    </li>
                `);
                
                item.click(function() {
                    if (!notification.read) {
                        markAsRead(notification.notification_ID);
                        $(this).removeClass('unread').addClass('read');
                        updateBadge(unreadCount - 1);
                    }
                });
                
                notificationList.append(item);
            });
            
            updateBadge(unreadCount);
        });
    }

    function markAsRead(notificationId) {
        $.post('/TimeSheets/MarkAsRead', { notificationId: notificationId });
    }

    function updateBadge(count) {
        var badge = $('#notificationBadge');
        if (count > 0) {
            badge.text(count).show();
        } else {
            badge.hide();
        }
    }


    setInterval(loadNotifications, 30000);
    

    loadNotifications();
});
