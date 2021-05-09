using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;

namespace Rickie.Homework.ShowcaseApp.Queries
{
    /// <summary>
    ///     Represents the query for getting all payments for a specific user
    /// </summary>
    public class GetPaymentsQuery : IRequest<ApiResponse<UserPaymentsPayload>>
    {
        public Guid UserId { get; set; }
    }


    /// <summary>
    ///     Query handler for getting all payments for a specific user
    /// </summary>
    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, ApiResponse<UserPaymentsPayload>>
    {
        private readonly IMapper _mapper;
        private readonly IPaymentRepositoryAsync _paymentRepository;
        private readonly IUserBalanceRepositoryAsync _userBalanceRepository;
        private readonly IUserRepositoryAsync _userRepository;

        public GetPaymentsQueryHandler(IUserRepositoryAsync userRepository,
            IUserBalanceRepositoryAsync userBalanceRepository, IPaymentRepositoryAsync paymentRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBalanceRepository = userBalanceRepository;
            _paymentRepository = paymentRepository;
        }

        /// <summary>
        ///     Get balance and payments of a specific user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<UserPaymentsPayload>> Handle(GetPaymentsQuery request,
            CancellationToken cancellationToken)
        {
            var user =
                (await _userRepository.FindByCondition(x => x.UserId.Equals(request.UserId)).ConfigureAwait(false))
                .AsQueryable().FirstOrDefault();
            var userBalance = (await _userBalanceRepository.FindByCondition(x => x.UserId.Equals(request.UserId))
                    .ConfigureAwait(false))
                .AsQueryable().FirstOrDefault();

            // TODO: Add paging if there are too many payments
            var paymentsList =
                (await _paymentRepository.FindByCondition(x => x.UserId.Equals(request.UserId)).ConfigureAwait(false))
                .AsQueryable().ToList()
                // Business requirement - sort payments by latest date
                .OrderByDescending(t => t.Date);

            var result = _mapper.Map<UserPaymentsPayload>(user);
            result.Balance = userBalance?.Balance ?? 0;
            result.Payments = _mapper.Map<IEnumerable<PaymentPayload>>(paymentsList);
            return new ApiResponse<UserPaymentsPayload>(result);
        }
    }
}