using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;
using Rickie.Homework.ShowcaseApp.Services;

namespace Rickie.Homework.ShowcaseApp.Command
{
    public class CreatePaymentCommand : IRequest<ApiResponse<PaymentPayload>>
    {
        public virtual Guid UserId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual Guid PayTo { get; set; }
    }

    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentPayload>>
    {
        private readonly IPaymentRepositoryAsync _paymentRepository;
        private readonly IMapper _mapper;
        public CreatePaymentCommandHandler(IPaymentRepositoryAsync paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<PaymentPayload>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentId = (await _paymentRepository.MakePaymentAsync(new Payment()
            {
                UserId = request.UserId,
                Amount = request.Amount,
                PayTo = request.PayTo
            }).ConfigureAwait(false));
            var payment = (await _paymentRepository.FindByCondition(x => x.PaymentId.Equals(paymentId.PaymentId))
                    .ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            // return new ApiResponse<PaymentPayload>(_mapper.Map<PaymentPayload>(payload));
            return new ApiResponse<PaymentPayload>(_mapper.Map<PaymentPayload>(payment));
        }
    }
}
