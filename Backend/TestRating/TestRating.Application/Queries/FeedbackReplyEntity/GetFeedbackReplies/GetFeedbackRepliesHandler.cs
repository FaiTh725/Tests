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

            await unitOfWork.BeginTransactionAsync(
                IsolationLevel.RepeatableRead, 
                cancellationToken);
            try
            {
                var allReplies = await unitOfWork.ReplyRepository
                    .GetRepliesByCriteria(new RepliesByFeedbackIdWithOwnerSpecification(
                        request.FeedbackId),
                        cancellationToken);

                var feedbackReplies = await unitOfWork.ReplyRepository
                    .GetRepliesByCriteria(new RepliesByFeedbackIdWithOwnerSpecification(
                        request.FeedbackId),
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                return new BasePaginationResponse<FeedbackReplyWithOwner>
                {
                    Page = request.Page,
                    PageCount = request.PageSize,
                    MaxCount = allReplies.Count(),
                    Items = feedbackReplies.Select(x => new FeedbackReplyWithOwner 
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
            catch
            {
                await unitOfWork.RollBackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
