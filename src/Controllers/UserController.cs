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


        /// <summary>
        ///     Get all user accounts in the system
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(GetAllUsersQuery command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        ///     Get all payments for the current user represented by the JWT token in current HTTP context
        /// </summary>
        /// <remarks>Payments are sorted by newest date.</remarks>
        /// <returns>User account summary including balance and payments</returns>
        [HttpPost("GetPayments")]
        public async Task<IActionResult> GetPayments()
        {
            // userID should have been set by JWTMiddleware
            var userId = (Guid) HttpContext.Items["UserId"];
            var command = new GetPaymentsQuery {UserId = userId};

            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        ///     Make a payment to another user account
        /// </summary>
        /// <param name="command">Inputs</param>
        /// <returns>Payment record created with status either Paid or Declined</returns>
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentCommand command)
        {
            // userID should have been set by JWTMiddleware
            var userId = (Guid) HttpContext.Items["UserId"];
            command.UserId = userId;

            return Ok(await Mediator.Send(command));
        }
    }
}