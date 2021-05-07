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
    ///     Represents the query for <see cref="User" />
    /// </summary>
    public class GetAllUsersQuery : IRequest<ApiResponse<IEnumerable<UserPayload>>>
    {
    }

    /// <summary>
    ///     Query handler for getting all <see cref="User" />s
    /// </summary>
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<IEnumerable<UserPayload>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepositoryAsync _userRepository;

        public GetAllUsersQueryHandler(IUserRepositoryAsync userRepository, IMapper mapper = null)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<UserPayload>>> Handle(GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            var usersList = (await _userRepository.GetAllAsync().ConfigureAwait(false)).AsQueryable().ToList();
            return new ApiResponse<IEnumerable<UserPayload>>(_mapper.Map<IEnumerable<UserPayload>>(usersList));
        }
    }
}