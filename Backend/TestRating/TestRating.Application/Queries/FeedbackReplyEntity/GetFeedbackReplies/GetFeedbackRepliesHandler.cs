using Application.Shared.Exceptions;
using MediatR;
using System.Data;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies
{
    public class GetFeedbackRepliesHandler :
        IRequestHandler<GetFeedbackRepliesQuery, BasePaginationResponse<FeedbackReplyWithOwner>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetFeedbackRepliesHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<BasePaginationResponse<FeedbackReplyWithOwner>> Handle(
            GetFeedbackRepliesQuery request, 
            CancellationToken cancellationToken)
        {
            var feedBack = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.FeedbackId, cancellationToken);

            if(feedBack is null)
            {
                throw new NotFoundException("Feedback doesnt exist");
            }

            var paginatedFeedbackReplies = await unitOfWork.ReplyRepository
                .GetPaginatedRepliesByCriteria(new RepliesPaginationByFeedbackIdWithOwnerSpecification(
                    request.FeedbackId,
                    request.Page,
                    request.PageSize),
                cancellationToken);

            return new BasePaginationResponse<FeedbackReplyWithOwner>
            {
                Page = request.Page,
                PageCount = request.PageSize,
                MaxCount = paginatedFeedbackReplies.TotalCount,
                Items = paginatedFeedbackReplies.Items.Select(x => 
                new FeedbackReplyWithOwner
                {
                    Id = x.Id,
                    FeedbackId = x.FeedbackId,
                    SendTime = x.SendTime,
                    UpdateTime = x.UpdateTime,
                    Text = x.Text,
                    Owner = new BaseProfileResponse
                    {
                        Id = x.Owner.Id,
                        Email = x.Owner.Email,
                        Name = x.Owner.Name
                    }
                })
            };
        }
    }
}
