using DeepOrangeTelegramBot.Bot;
using DeepOrangeTelegramBot.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace DeepOrangeTelegramBot.Controllers
{
    [ApiController]
    [Route("/[action]")]
    public class AuthController : Controller
    {
        private readonly OIdcService _oIdcService;

        public AuthController(OIdcService oIdcService)
        {
            _oIdcService = oIdcService;
        }

        [HttpGet]
        public async Task<IActionResult> Auth(string code, string state)
        {
            var userInfo = await _oIdcService.CompleteAuthAsync(state, code);
            
            if(userInfo == null)
            {
                return NotFound();
            }
            
            /*tg://resolve?domain=DeepOrange_bot*/
            return Redirect($"https://t.me/DeepOrange_bot?start=auth");
        }

        [HttpGet]
        public async Task Logout()
        {
            
        }
    }
}
