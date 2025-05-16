using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.services.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class notificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public notificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost]
        public async Task<ActionResult> PushNotification(string receiverId, string content, string type)
        {
            var notification = await _notificationService.PushNotification(receiverId, content, type);
            return Ok(new { Code = 0, message = "ok", data = notification });
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifications = await _notificationService.GetNotifications(userId!);
            return Ok(new { Code = 0, message = "ok", data = notifications });
        }
        [HttpPost("mark-as-read")]
        [Authorize]
        public async Task<ActionResult> MarkAsRead([FromBody] List<string> notificationListId)
        {
            await _notificationService.MarkAsRead(notificationListId);
            return Ok(new { Code = 0, message = "ok" });
        }

    }
}