using MassTransit;

namespace Authorization.Application.SagaOrchestrator.States
{
    public class RegisterUserSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public long? FeedbackProfileId { get; set; }

        public long? TestProfileId { get; set; }

        public bool IsFeedbackProfileCreated { get; set; }

        public bool IsTestProfileCreated { get; set; }
    }
}
