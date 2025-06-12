using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackReplyEntity.GetReplyWithOwnerById
{
    public class GetReplyWithOwnerByIdHandler :
        IRequestHandler<GetReplyWithOwnerByIdQuery, FeedbackReplyWithOwner>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetReplyWithOwnerByIdHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<FeedbackReplyWithOwner> Handle(
            GetReplyWithOwnerByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var reply = await unitOfWork.ReplyRepository
                .GetReplyByCriteria(new ReplyByIdWithOwnerSpecification(
                    request.Id),
                    cancellationToken);

            if(reply is null)
            {
                throw new NotFoundException("Feedback reply doesnt exist");
            }

            return new FeedbackReplyWithOwner 
            { 
                Id = reply.Id,
                FeedbackId = reply.FeedbackId,
                Text = reply.Text,
                SendTime = reply.SendTime,
                UpdateTime = reply.UpdateTime,
                Owner = new BaseProfileResponse
                {
                    Id = reply.Owner.Id,
                    Email = reply.Owner.Email,
                    Name = reply.Owner.Name
                } 
            };

        }
    }
}
