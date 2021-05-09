using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;

namespace Rickie.Homework.ShowcaseApp.Command
{
    /// <summary>
    ///     Input payload for making a payment
    /// </summary>
    public class CreatePaymentCommand : IRequest<ApiResponse<PaymentPayload>>
    {
        /// <summary>
        ///     Id of the paying user
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        ///     How much to pay
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        ///     Who to pay to
        /// </summary>
        public virtual Guid PayTo { get; set; }
    }

    /// <summary>
    ///     Handler to process payment command
    /// </summary>
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentPayload>>
    {
        private readonly IMapper _mapper;
        private readonly IPaymentRepositoryAsync _paymentRepository;

        public CreatePaymentCommandHandler(IPaymentRepositoryAsync paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Do the payment and return payment as result
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<PaymentPayload>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentId = await _paymentRepository.MakePaymentAsync(new Payment
            {
                UserId = request.UserId,
                Amount = request.Amount,
                PayTo = request.PayTo
            }).ConfigureAwait(false);
            var payment = (await _paymentRepository.FindByCondition(x => x.PaymentId.Equals(paymentId.PaymentId))
                .ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            return new ApiResponse<PaymentPayload>(_mapper.Map<PaymentPayload>(payment));
        }
    }
}