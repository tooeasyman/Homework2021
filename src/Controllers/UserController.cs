using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rickie.Homework.ShowcaseApp.Command;
using Rickie.Homework.ShowcaseApp.Queries;

namespace Rickie.Homework.ShowcaseApp.Controllers
{
    /// <summary>
    ///     Controller for user accounts related APIs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IMediator _mediator;

        private IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();


        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(GetAllUsersQuery command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("GetPayments")]
        public async Task<IActionResult> GetPayments()
        {
            // userID should have been set by JWTMiddleware
            var userId = (Guid) HttpContext.Items["UserId"];
            var command = new GetPaymentsQuery {UserId = userId};

            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentCommand command)
        {
            // userID should have been set by JWTMiddleware
            var userId = (Guid)HttpContext.Items["UserId"];
            command.UserId = userId;

            return Ok(await Mediator.Send(command));
        }
    }
}