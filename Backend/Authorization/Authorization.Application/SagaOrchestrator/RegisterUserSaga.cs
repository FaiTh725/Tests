using Authorization.Application.SagaOrchestrator.States;
using Authorization.Contracts.Events.Profile;
using Authorization.Contracts.Events.User;
using MassTransit;
using Test.Contracts.Profile;
using TestRating.Contracts.Profile;

namespace Authorization.Application.SagaOrchestrator
{
    public class RegisterUserSaga : MassTransitStateMachine<RegisterUserSagaState>
    {
        public State UserCreated { get; private set; }
        public State FeedbackProfileCreated { get; private set; }
        public State TestProfileCreated { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }

        public Event<IUserCreated> UserCreatedEvent { get; private set; }
        public Event<IFeedbackProfileCreated> FeedbackProfileCreatedEvent { get; private set; }
        public Event<ITestProfileCreated> TestProfileCreatedEvent { get; private set; }
        public Event<IFeedbackProfileCreateFailed> FeedbackProfileFailedEvent { get; private set; }
        public Event<ITestProfileCreateFailed> TestProfileFailedEvent { get; private set; }

        public RegisterUserSaga()
        {
            InstanceState(x => x.CurrentState);
            Event(() => UserCreatedEvent, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.SelectId(context => context.Message.CorrelationId);
            });
            Event(() => FeedbackProfileCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TestProfileCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => FeedbackProfileFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TestProfileFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Email = context.Message.Email;
                        context.Saga.Name = context.Message.Name;
                    })
                    .TransitionTo(UserCreated)
                    .Publish(context => new CreateTestProfile
                    {
                        Email = context.Saga.Email,
                        Name = context.Saga.Name,
                        CorrelationId = context.Saga.CorrelationId
                    })
                    .Publish(context => new CreateFeedbackProfile
                    {
                        Email = context.Saga.Email,
                        Name = context.Saga.Name,
                        CorrelationId = context.Saga.CorrelationId
                    }));

            During(UserCreated,
                When(FeedbackProfileCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.IsFeedbackProfileCreated = true;
                        context.Saga.FeedbackProfileId = context.Message.FeedbackProfileId;
                    })
                    .If(context => context.Saga.IsTestProfileCreated,
                        binder => binder.Finalize()),

                When(TestProfileCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.IsTestProfileCreated = true;
                        context.Saga.TestProfileId = context.Message.TestProfileId;
                    })
                    .If(context => context.Saga.IsFeedbackProfileCreated,
                        binder => binder.Finalize()),

                When(FeedbackProfileFailedEvent)
                    .Publish(context => new DeleteFeedbackProfile
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Id = context.Saga.FeedbackProfileId!.Value
                    })
                    .TransitionTo(Failed)
                    .Finalize(),

                When(TestProfileFailedEvent)
                    .Publish(context => new DeleteTestProfile
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Id = context.Saga.TestProfileId!.Value
                    })
                    .TransitionTo(Failed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
