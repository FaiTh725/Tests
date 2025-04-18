﻿using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Intrefaces;

namespace Test.Application.Commands.Test.DeleteTest
{
    public class DeleteTestHandler :
        IRequestHandler<DeleteTestCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public DeleteTestHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteTestCommand request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.Id, cancellationToken);

            if(test is null)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            await unitOfWork.TestRepository
                .DeleteTest(request.Id, cancellationToken);

            test.Delete();
        }
    }
}
